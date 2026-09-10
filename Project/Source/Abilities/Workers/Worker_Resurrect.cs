using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class Worker_Resurrect : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            // Solo permite hacer clic sobre cadáveres con un ser biológico dentro
            if (target.Thing is Corpse corpse)
            {
                return corpse.InnerPawn != null && !corpse.InnerPawn.RaceProps.IsMechanoid;
            }
            return false;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Corpse corpse) || corpse.InnerPawn == null) return;

            Pawn deadPawn = corpse.InnerPawn;
            Map map = caster.Map;
            IntVec3 pos = corpse.Position;

            // 1. Si el cadáver no tiene cabeza o corazón, se los reconstruimos antes de revivirlo
            var missingVital = deadPawn.health.hediffSet.GetMissingPartsCommonAncestors().ToList();
            foreach (var missing in missingVital)
            {
                deadPawn.health.RestorePart(missing.Part);
            }

            // 2. Resurrección
            bool success = ResurrectionUtility.TryResurrect(deadPawn);

            if (success)
            {
                // 3. Limpieza de secuelas negativas (resurrection psychosis, ceguera, etc.)
                var badHediffs = new List<Hediff>();
                foreach (var h in deadPawn.health.hediffSet.hediffs)
                {
                    if (h.def == HediffDefOf.ResurrectionSickness || 
                        h.def.defName == "ResurrectionPsychosis" ||
                        h is Hediff_Injury)
                    {
                        badHediffs.Add(h);
                    }
                }
                foreach (var h in badHediffs)
                {
                    deadPawn.health.RemoveHediff(h);
                }

                // 4. Refresco gráfico completo
                deadPawn.Drawer?.renderer?.SetAllGraphicsDirty();
                PortraitsCache.SetDirty(deadPawn);
                GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(deadPawn);

                // 5. Efectos de sombras
                FleckMaker.ThrowDustPuff(pos.ToVector3Shifted(), map, 3.0f);
                MoteMaker.ThrowText(pos.ToVector3Shifted(), map, "¡RESUCITADO!", Color.magenta);
                Messages.Message($"{deadPawn.LabelShort} ha sido arrancado de la muerte por Kayn.", deadPawn, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message($"No se pudo resucitar a {deadPawn.LabelShort}.", MessageTypeDefOf.RejectInput, false);
            }
        }
    }
}