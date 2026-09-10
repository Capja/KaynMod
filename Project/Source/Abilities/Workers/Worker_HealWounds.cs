using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_HealWounds : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            return target.Thing is Pawn;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Pawn healTarget) || healTarget.health?.hediffSet == null) return;

            var injuries = new List<Hediff_Injury>();
            foreach (var h in healTarget.health.hediffSet.hediffs)
            {
                if (h is Hediff_Injury injury) injuries.Add(injury);
            }

            foreach (var injury in injuries)
            {
                healTarget.health.RemoveHediff(injury);
            }

            var bloodLoss = healTarget.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLoss != null && bloodLoss.Severity > 0.05f)
            {
                bloodLoss.Severity = 0.05f;
            }

            MoteMaker.ThrowText(healTarget.DrawPos, healTarget.Map, "Heridas Cerradas", UnityEngine.Color.green);
            Messages.Message($"{healTarget.LabelShort} ha cerrado todas sus heridas ({injuries.Count} sanadas).", healTarget, MessageTypeDefOf.PositiveEvent);
        }
    }
}