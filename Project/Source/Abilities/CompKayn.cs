using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class Command_KaynAbility : Command_Action
    {
        public Pawn caster;
        public KaynAbilityDef abilityDef;

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();
            if (caster != null && abilityDef != null)
            {
                abilityDef.Worker.DrawHighlight(abilityDef, caster, LocalTargetInfo.Invalid);
            }
        }
    }

    public class CompKayn : ThingComp
    {
        public Pawn Pawn => (Pawn)parent;
        public const string AwakenedHediffDefName = "Kayn_Awakened";

        public bool IsAwakened => Pawn?.health?.hediffSet != null && 
                                  Pawn.health.hediffSet.HasHediff(HediffDef.Named(AwakenedHediffDefName));

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!IsAwakened) yield break;

            foreach (var def in DefDatabase<KaynAbilityDef>.AllDefs)
            {
                yield return new Command_KaynAbility
                {
                    caster = Pawn,
                    abilityDef = def,
                    defaultLabel = def.label,
                    defaultDesc = def.description,
                    icon = def.UIIcon,
                    action = () => StartTargeting(def)
                };
            }
        }

        private void StartTargeting(KaynAbilityDef ability)
        {
            var targetParams = new TargetingParameters
            {
                canTargetPawns = true,
                canTargetLocations = !ability.targetMustBePawn,
                canTargetBuildings = false,
                canTargetSelf = true,
                validator = (TargetInfo t) =>
                {
                    if (t.Cell.DistanceTo(Pawn.Position) > ability.range) return false;

                    // Si el objetivo es un colono/objeto toma su Thing, si no, toma la celda del suelo
                    LocalTargetInfo localTarget = t.HasThing ? new LocalTargetInfo(t.Thing) : new LocalTargetInfo(t.Cell);
                    return ability.Worker.CanTarget(ability, Pawn, localTarget);
                }
            };

            Action<LocalTargetInfo> onHighlight = (LocalTargetInfo t) =>
            {
                ability.Worker.DrawHighlight(ability, Pawn, t);
            };

            Find.Targeter.BeginTargeting(
                targetParams,
                (LocalTargetInfo target) => ExecuteAbility(ability, target),
                onHighlight,
                null,
                Pawn
            );
        }

        public void ExecuteAbility(KaynAbilityDef ability, LocalTargetInfo target)
        {
            var ev = new KaynAbilityEvent(Pawn, ability, target);

            // Ejecuta el worker individual de esa habilidad
            ability.Worker.Apply(ability, Pawn, target);

            // Registro de percepción
            foreach (Pawn potentialWitness in Pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (potentialWitness == Pawn) continue;
                if (!potentialWitness.RaceProps.Humanlike) continue;

                var record = PerceptionEvaluator.EvaluatePawn(potentialWitness, ev);
                ev.Witnesses.Add(record);
            }

            Current.Game?.GetComponent<GameComponent_KaynTracker>()?.RecordEvent(ev);
            KaynDebugger.PrintEventLog(ev);
        }
    }
}