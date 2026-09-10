using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class WitnessRecord
    {
        public Pawn Pawn;
        public bool CanPerceiveKayn;
        public bool CanPerceiveEffect;
        public bool CorrelatesToKayn; // ¿Atribuye el efecto a Kayn?
        public string PerceptionReason; // Para depuración (ej: "Línea de visión bloqueada", "Kayn invisible", etc.)
    }

    public class KaynAbilityEvent
    {
        public Pawn Kayn;
        public KaynAbilityDef Ability;
        public LocalTargetInfo Target;
        public IntVec3 EffectPosition;
        public Map Map;
        public int Tick;
        public List<WitnessRecord> Witnesses = new List<WitnessRecord>();

        public KaynAbilityEvent(Pawn kayn, KaynAbilityDef ability, LocalTargetInfo target)
        {
            Kayn = kayn;
            Ability = ability;
            Target = target;
            EffectPosition = target.Cell;
            Map = kayn.Map;
            Tick = Find.TickManager.TicksGame;
        }
    }
}