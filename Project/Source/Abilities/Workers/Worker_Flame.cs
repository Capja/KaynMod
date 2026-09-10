using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_Flame : KaynAbilityWorker
    {
        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            FireUtility.TryStartFireIn(target.Cell, caster.Map, 0.2f, caster);
        }
    }
}