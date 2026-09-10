using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_Cataclysm : KaynAbilityWorker
    {
        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            float radius = def.explosionRadius > 0f ? def.explosionRadius : 4.9f;
            GenExplosion.DoExplosion(target.Cell, caster.Map, radius, DamageDefOf.Bomb, caster);
        }
    }
}