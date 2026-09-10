using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class KaynWitnessData : IExposable
    {
        public Pawn witness;
        public bool canPerceiveKayn;
        public bool canPerceiveEffect;
        public bool correlatesToKayn;

        public KaynWitnessData() { }

        public KaynWitnessData(WitnessRecord record)
        {
            witness = record.Pawn;
            canPerceiveKayn = record.CanPerceiveKayn;
            canPerceiveEffect = record.CanPerceiveEffect;
            correlatesToKayn = record.CorrelatesToKayn;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref witness, "witness");
            Scribe_Values.Look(ref canPerceiveKayn, "canPerceiveKayn");
            Scribe_Values.Look(ref canPerceiveEffect, "canPerceiveEffect");
            Scribe_Values.Look(ref correlatesToKayn, "correlatesToKayn");
        }
    }

    public class KaynSavedEvent : IExposable
    {
        public string abilityDefName;
        public IntVec3 position;
        public int tick;
        public List<KaynWitnessData> witnesses = new List<KaynWitnessData>();

        public KaynSavedEvent() { }

        public void ExposeData()
        {
            Scribe_Values.Look(ref abilityDefName, "abilityDefName");
            Scribe_Values.Look(ref position, "position");
            Scribe_Values.Look(ref tick, "tick");
            Scribe_Collections.Look(ref witnesses, "witnesses", LookMode.Deep);
            if (witnesses == null) witnesses = new List<KaynWitnessData>();
        }
    }

    public class GameComponent_KaynTracker : GameComponent
    {
        public List<KaynSavedEvent> recordedEvents = new List<KaynSavedEvent>();

        // CORRECCIÓN 1: Constructores compatibles con RimWorld moderno (sin base con argumentos)
        public GameComponent_KaynTracker() : base() { }
        public GameComponent_KaynTracker(Game game) : base() { }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref recordedEvents, "kaynRecordedEvents", LookMode.Deep);

            if (recordedEvents == null)
            {
                recordedEvents = new List<KaynSavedEvent>();
            }

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                CleanupDeadWitnesses();
            }
        }

        public void RecordEvent(KaynAbilityEvent ev)
        {
            if (recordedEvents == null) recordedEvents = new List<KaynSavedEvent>();

            var usefulWitnesses = new List<KaynWitnessData>();

            foreach (var w in ev.Witnesses)
            {
                if (w.Pawn == null || w.Pawn.Dead || !w.Pawn.RaceProps.Humanlike) continue;
                if (!w.CanPerceiveKayn && !w.CanPerceiveEffect) continue;

                usefulWitnesses.Add(new KaynWitnessData(w));
            }

            if (usefulWitnesses.Count == 0) return;

            KaynSavedEvent saved = new KaynSavedEvent
            {
                abilityDefName = ev.Ability.defName,
                position = ev.EffectPosition,
                tick = ev.Tick,
                witnesses = usefulWitnesses
            };

            recordedEvents.Add(saved);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            // Revisa cada segundo (60 ticks) si Kayn debe resucitar
            if (Find.TickManager != null && Find.TickManager.TicksGame % 60 == 0)
            {
                CheckAndReviveImmortalKayn();
                CleanupDeadWitnesses();
            }
        }


        private void CheckAndReviveImmortalKayn()
        {
            if (Find.Maps == null) return;

            var awakenedDef = HediffDef.Named(CompKayn.AwakenedHediffDefName);
            if (awakenedDef == null) return;

            // 10 segundos de juego = 600 ticks
            const int TicksBeforeRevival = 600;

            foreach (Map map in Find.Maps)
            {
                // 1. Cadáveres en el suelo
                var corpses = map.listerThings?.ThingsInGroup(ThingRequestGroup.Corpse)?.OfType<Corpse>().ToList();
                if (corpses != null)
                {
                    foreach (Corpse corpse in corpses)
                    {
                        // Espera exactamente a que hayan pasado 10 segundos desde que murió
                        if (corpse.Age >= TicksBeforeRevival)
                        {
                            TryReconstitutePawn(corpse.InnerPawn, corpse.Position, map);
                        }
                    }
                }

                // 2. Cadáveres dentro de tumbas
                var graves = map.listerBuildings?.allBuildingsColonist?.OfType<Building_Grave>().ToList();
                if (graves != null)
                {
                    foreach (var grave in graves)
                    {
                        if (grave.Corpse?.InnerPawn?.health?.hediffSet?.HasHediff(awakenedDef) == true)
                        {
                            if (grave.Corpse.Age >= TicksBeforeRevival)
                            {
                                IntVec3 pos = grave.Position;
                                Pawn deadPawn = grave.Corpse.InnerPawn;
                                grave.EjectContents();
                                TryReconstitutePawn(deadPawn, pos, map);
                            }
                        }
                    }
                }
            }
        }


        
        private void TryReconstitutePawn(Pawn p, IntVec3 pos, Map map)
        {
            var awakenedDef = HediffDef.Named(CompKayn.AwakenedHediffDefName);
            if (p == null || !p.Dead || p.health?.hediffSet?.HasHediff(awakenedDef) != true) return;

            // 1. Reconstruir órganos vitales (Corazón, Cabeza, Cerebro...)
            var missingVital = p.health.hediffSet.GetMissingPartsCommonAncestors().ToList();
            foreach (var part in missingVital)
            {
                p.health.RestorePart(part.Part);
            }

            // 2. Cerrar heridas letales
            var injuries = p.health.hediffSet.hediffs.OfType<Hediff_Injury>().ToList();
            foreach (var inj in injuries)
            {
                p.health.RemoveHediff(inj);
            }

            // 3. Resurrección
            bool success = ResurrectionUtility.TryResurrect(p);
            if (success)
            {
                var sickness = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ResurrectionSickness);
                if (sickness != null) p.health.RemoveHediff(sickness);

                p.Drawer?.renderer?.SetAllGraphicsDirty();
                PortraitsCache.SetDirty(p);
                GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(p);

                FleckMaker.ThrowDustPuff(pos.ToVector3Shifted(), map, 3.0f);
                MoteMaker.ThrowText(pos.ToVector3Shifted(), map, "¡RECONSTITUCIÓN!", Color.magenta);
                Messages.Message($"{p.LabelShort} ha engañado a la muerte. Las sombras han reconstruido su cuerpo.", p, MessageTypeDefOf.PositiveEvent);
            }
        }

        public void CleanupDeadWitnesses()
        {
            if (recordedEvents == null) return;

            for (int i = recordedEvents.Count - 1; i >= 0; i--)
            {
                var ev = recordedEvents[i];
                if (ev == null || ev.witnesses == null)
                {
                    recordedEvents.RemoveAt(i);
                    continue;
                }

                ev.witnesses.RemoveAll(w => w == null || w.witness == null || w.witness.Dead || w.witness.Destroyed);

                if (ev.witnesses.Count == 0)
                {
                    recordedEvents.RemoveAt(i);
                }
            }
        }
    }
}