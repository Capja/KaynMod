using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_Stun : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            return target.Thing is Pawn;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (target.Thing is Pawn p)
            {
                p.TakeDamage(new DamageInfo(DamageDefOf.Stun, 10f, 0, -1, caster));
            }
        }
    }
}