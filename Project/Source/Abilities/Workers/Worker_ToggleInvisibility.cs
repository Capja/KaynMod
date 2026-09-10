using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_ToggleInvisibility : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (target.Thing is Pawn p)
            {
                if (p == caster) return true;
                return p.Faction != null && p.Faction == caster.Faction;
            }
            return false;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Pawn targetPawn)) return;

            var invisDef = HediffDef.Named("Kayn_Invisibility");
            Hediff current = targetPawn.health.hediffSet.GetFirstHediffOfDef(invisDef);

            if (current != null)
            {
                targetPawn.health.RemoveHediff(current);
                MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "Velo Disipado", UnityEngine.Color.gray);
                Messages.Message($"{targetPawn.LabelShort} vuelve a ser visible.", targetPawn, MessageTypeDefOf.NeutralEvent);
            }
            else
            {
                targetPawn.health.AddHediff(invisDef);
                MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "Velo Activado", UnityEngine.Color.cyan);
                Messages.Message($"{targetPawn.LabelShort} ahora es invisible.", targetPawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}