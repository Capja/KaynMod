using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    [StaticConstructorOnStartup]
    public static class KaynBiotechIntegration
    {
        static KaynBiotechIntegration()
        {
            var awakenedDef = DefDatabase<HediffDef>.GetNamedSilentFail(CompKayn.AwakenedHediffDefName);

            // =========================================================================
            // 1. INMUNIDAD AL ZUMBIDO PSÍQUICO NATIVA (SIN HARMONY)
            // =========================================================================
            if (awakenedDef != null)
            {
                // Lista de pensamientos psíquicos del mapa a anular
                string[] thoughtsToNullify = new string[] { "PsychicDrone", "PsychicSuppression" };

                foreach (var thoughtName in thoughtsToNullify)
                {
                    var thought = DefDatabase<ThoughtDef>.GetNamedSilentFail(thoughtName);
                    if (thought != null)
                    {
                        if (thought.nullifyingHediffs == null)
                        {
                            thought.nullifyingHediffs = new List<HediffDef>();
                        }
                        // Le decimos al motor de RimWorld que Kayn_Awakened anula este pensamiento
                        if (!thought.nullifyingHediffs.Contains(awakenedDef))
                        {
                            thought.nullifyingHediffs.Add(awakenedDef);
                        }
                    }
                }
            }

            // =========================================================================
            // 2. INYECCIÓN GLOBAL DE BIOTECH (ANCHO DE BANDA DE MECANIZADOR)
            // =========================================================================
            if (!ModLister.BiotechInstalled || awakenedDef?.stages == null) return;

            var bandwidthStat = DefDatabase<StatDef>.GetNamedSilentFail("MechBandwidth");
            var controlGroupsStat = DefDatabase<StatDef>.GetNamedSilentFail("MechControlGroups");
            var repairSpeedStat = DefDatabase<StatDef>.GetNamedSilentFail("MechRepairSpeed");
            var gestationSpeedStat = DefDatabase<StatDef>.GetNamedSilentFail("MechGestationSpeed");

            if (bandwidthStat == null) return;

            foreach (var stage in awakenedDef.stages)
            {
                if (stage.statOffsets == null)
                {
                    stage.statOffsets = new List<StatModifier>();
                }

                stage.statOffsets.Add(new StatModifier { stat = bandwidthStat, value = 1000f });
                if (controlGroupsStat != null) stage.statOffsets.Add(new StatModifier { stat = controlGroupsStat, value = 8f });
                if (repairSpeedStat != null) stage.statOffsets.Add(new StatModifier { stat = repairSpeedStat, value = 5f });
                if (gestationSpeedStat != null) stage.statOffsets.Add(new StatModifier { stat = gestationSpeedStat, value = 5f });
            }
        }
    }
}