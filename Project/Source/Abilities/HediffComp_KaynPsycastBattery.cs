using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class HediffCompProperties_KaynPsycastBattery : HediffCompProperties
    {
        public HediffCompProperties_KaynPsycastBattery()
        {
            compClass = typeof(HediffComp_KaynPsycastBattery);
        }
    }

    public class HediffComp_KaynPsycastBattery : HediffComp
    {
        public override void CompPostMake()
        {
            base.CompPostMake();
            EnsureBasePsylink();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (Pawn != null && Pawn.IsHashIntervalTick(30))
            {
                // Solo otorga psylink si el peón no tiene absolutamente ningún nivel (nivel 0)
                if (ModLister.RoyaltyInstalled && Pawn.GetPsylinkLevel() == 0)
                {
                    EnsureBasePsylink();
                }

                // 1. Control de calor neuronal a cero y psicofoco permanente al 100%
                if (Pawn.psychicEntropy != null)
                {
                    Pawn.psychicEntropy.RemoveAllEntropy();
                    Pawn.psychicEntropy.OffsetPsyfocusDirectly(1.0f);
                }

                // 2. Mantener puntos de habilidad en Vanilla Psycasts Expanded (VPE) sin spamear niveles
                if (Pawn.health?.hediffSet != null)
                {
                    var vpeHediff = Pawn.health.hediffSet.hediffs
                        .FirstOrDefault(h => h.GetType().Name == "Hediff_PsycastAbilities");

                    if (vpeHediff != null)
                    {
                        FieldInfo pointsField = vpeHediff.GetType().GetField("points");
                        if (pointsField != null)
                        {
                            int currentPoints = (int)pointsField.GetValue(vpeHediff);
                            if (currentPoints < 100)
                            {
                                pointsField.SetValue(vpeHediff, 100);
                            }
                        }
                    }
                }
            }
        }

        private void EnsureBasePsylink()
        {
            // Otorga nivel 6 una sola vez y sin enviar cartas
            if (ModLister.RoyaltyInstalled && Pawn != null && Pawn.GetPsylinkLevel() < 6)
            {
                int needed = 6 - Pawn.GetPsylinkLevel();
                Pawn.ChangePsylinkLevel(needed, false);
            }
        }
    }
}