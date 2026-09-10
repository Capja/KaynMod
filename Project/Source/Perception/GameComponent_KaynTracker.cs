using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    // Datos serializables de un testigo individual
    public class KaynWitnessData : IExposable
    {
        public Pawn witness;
        public bool canPerceiveKayn;
        public bool canPerceiveEffect;
        public bool correlatesToKayn;

        public KaynWitnessData() { } // Constructor vacío obligatorio para RimWorld

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

    // Registro serializable del evento guardado
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
        }
    }

    // Componente central que vive en la partida guardada
    public class GameComponent_KaynTracker : GameComponent
    {
        public List<KaynSavedEvent> recordedEvents = new List<KaynSavedEvent>();

        public GameComponent_KaynTracker(Game game) : base() { }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref recordedEvents, "kaynRecordedEvents", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (recordedEvents == null) recordedEvents = new List<KaynSavedEvent>();
                CleanupDeadWitnesses();
            }
        }

        public void RecordEvent(KaynAbilityEvent ev)
        {
            var usefulWitnesses = new List<KaynWitnessData>();

            foreach (var w in ev.Witnesses)
            {
                // 1. Descartar nulos, muertos y animales
                if (w.Pawn == null || w.Pawn.Dead || !w.Pawn.RaceProps.Humanlike) continue;

                // 2. Descartar a quienes no hayan percibido NADA útil
                if (!w.CanPerceiveKayn && !w.CanPerceiveEffect) continue;

                usefulWitnesses.Add(new KaynWitnessData(w));
            }

            // Si nadie vio nada útil, el evento se ignora por completo (no gasta memoria)
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

        // Se ejecuta automáticamente cada tick en segundo plano
        public override void GameComponentTick()
        {
            base.GameComponentTick();

            // Limpieza periódica: una vez cada ~16 segundos (1000 ticks)
            if (Find.TickManager.TicksGame % 1000 == 0)
            {
                CleanupDeadWitnesses();
            }
        }

        /// <summary>
        /// Purga a los testigos muertos o desaparecidos.
        /// Si un evento se queda sin testigos vivos, el evento se borra del juego.
        /// </summary>
        public void CleanupDeadWitnesses()
        {
            if (recordedEvents == null) return;

            for (int i = recordedEvents.Count - 1; i >= 0; i--)
            {
                var ev = recordedEvents[i];

                // Quita a los testigos muertos o destruidos
                ev.witnesses.RemoveAll(w => w.witness == null || w.witness.Dead || w.witness.Destroyed);

                // Si no quedan testigos vivos, el secreto murió con ellos: se elimina el evento
                if (ev.witnesses.Count == 0)
                {
                    recordedEvents.RemoveAt(i);
                }
            }
        }
    }
}