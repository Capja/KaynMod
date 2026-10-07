using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    // Se ejecuta una sola vez al arrancar el juego
    [StaticConstructorOnStartup]
    public static class KaynBiotechIntegration
    {
        static KaynBiotechIntegration()
        {
            // Si el jugador no tiene Biotech, no hace nada
            if (!ModLister.BiotechInstalled) return;

            var awakenedDef = DefDatabase<HediffDef>.GetNamedSilentFail(CompKayn.AwakenedHediffDefName);
            if (awakenedDef?.stages == null) return;

            var bandwidthStat = DefDatabase<StatDef>.GetNamedSilentFail("MechBandwidth");
            var controlGroupsStat = DefDatabase<StatDef>.GetNamedSilentFail("MechControlGroups");
            var repairSpeedStat = DefDatabase<StatDef>.GetNamedSilentFail("MechRepairSpeed");
            var gestationSpeedStat = DefDatabase<StatDef>.GetNamedSilentFail("MechGestationSpeed");

            if (bandwidthStat == null) return;

            // Inyecta el ancho de banda y bonos en TODAS las fases actuales y futuras de Kayn
            foreach (var stage in awakenedDef.stages)
            {
                if (stage.statOffsets == null)
                {
                    stage.statOffsets = new List<StatModifier>();
                }

                // Añade los modificadores globales de mecanizador
                stage.statOffsets.Add(new StatModifier { stat = bandwidthStat, value = 1000f });
                if (controlGroupsStat != null) stage.statOffsets.Add(new StatModifier { stat = controlGroupsStat, value = 8f });
                if (repairSpeedStat != null) stage.statOffsets.Add(new StatModifier { stat = repairSpeedStat, value = 5f });
                if (gestationSpeedStat != null) stage.statOffsets.Add(new StatModifier { stat = gestationSpeedStat, value = 5f });
            }
        }
    }
}