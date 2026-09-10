using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_Awaken : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            return target.Thing is Pawn p && p.RaceProps.Humanlike;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Pawn targetPawn)) return;

            var hediffDef = HediffDef.Named(CompKayn.AwakenedHediffDefName);
            if (!targetPawn.health.hediffSet.HasHediff(hediffDef))
            {
                targetPawn.health.AddHediff(hediffDef);
                Messages.Message($"{targetPawn.LabelShort} ha sido despertado con la Esencia de Kayn.", targetPawn, MessageTypeDefOf.PositiveEvent);
                MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "¡DESPERTADO!", UnityEngine.Color.magenta);
            }
        }
    }
}