using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_Teleport : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            return target.Cell.Standable(caster.Map) && !target.Cell.Fogged(caster.Map);
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            IntVec3 origin = caster.Position;
            Map map = caster.Map;

            FleckMaker.ThrowDustPuff(origin.ToVector3Shifted(), map, 2.0f);

            caster.Position = target.Cell;
            caster.pather?.StopDead();
            caster.Notify_Teleported(true, true);
            caster.Drawer?.tweener?.ResetTweenedPosToRoot();

            FleckMaker.ThrowDustPuff(target.Cell.ToVector3Shifted(), map, 2.0f);
        }
    }
}