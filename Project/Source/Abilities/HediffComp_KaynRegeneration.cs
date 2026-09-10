using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class HediffCompProperties_KaynRegeneration : HediffCompProperties
    {
        public HediffCompProperties_KaynRegeneration()
        {
            compClass = typeof(HediffComp_KaynRegeneration);
        }
    }

    public class HediffComp_KaynRegeneration : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            // Se ejecuta cada 20 ticks (1/3 de segundo)
            if (Pawn != null && Pawn.IsHashIntervalTick(20))
            {
                if (Pawn.health?.hediffSet == null) return;

                // 1. Curar heridas normales (cortes, disparos, quemaduras)
                var injuries = new List<Hediff_Injury>();
                foreach (var h in Pawn.health.hediffSet.hediffs)
                {
                    if (h is Hediff_Injury injury) injuries.Add(injury);
                }

                foreach (var injury in injuries)
                {
                    injury.Severity -= 1.0f;
                    if (injury.Severity <= 0f)
                    {
                        Pawn.health.RemoveHediff(injury);
                    }
                }

                // 2. Detener hemorragias y recuperar sangre
                var bloodLoss = Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
                if (bloodLoss != null && bloodLoss.Severity > 0f)
                {
                    bloodLoss.Severity -= 0.05f;
                    if (bloodLoss.Severity <= 0f)
                    {
                        Pawn.health.RemoveHediff(bloodLoss);
                    }
                }
            }
        }
    }
}