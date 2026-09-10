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
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            // Comprobación periódica cada 30 ticks (medio segundo)
            if (Pawn != null && Pawn.IsHashIntervalTick(30))
            {
                // 1. Enlace psíquico vanilla nivel 6
                if (ModLister.RoyaltyInstalled && Pawn.GetPsylinkLevel() < 6)
                {
                    Pawn.ChangePsylinkLevel(6, false);
                }

                // 2. Control de calor a 0 y psicofoco al 100%
                if (Pawn.psychicEntropy != null)
                {
                    Pawn.psychicEntropy.RemoveAllEntropy();
                    Pawn.psychicEntropy.OffsetPsyfocusDirectly(1.0f);
                }

                // 3. COMPATIBILIDAD CON VANILLA PSYCASTS EXPANDED (VPE)
                // Otorga 100 puntos de habilidad psiónica para desbloquear los árboles completos
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
    }
}