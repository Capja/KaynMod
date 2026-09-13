using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class HediffCompProperties_KaynImmunity : HediffCompProperties
    {
        public HediffCompProperties_KaynImmunity()
        {
            compClass = typeof(HediffComp_KaynImmunity);
        }
    }

    public class HediffComp_KaynImmunity : HediffComp
    {
        public override void CompPostMake()
        {
            base.CompPostMake();
            PurgeDiseases();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            // Revisa cada segundo (60 ticks)
            if (Pawn != null && Pawn.health?.hediffSet != null && Pawn.IsHashIntervalTick(60))
            {
                PurgeDiseases();
            }
        }

        private void PurgeDiseases()
        {
            if (Pawn?.health?.hediffSet?.hediffs == null) return;

            var toRemove = new List<Hediff>();

            foreach (var h in Pawn.health.hediffSet.hediffs)
            {
                if (IsDiseaseOrAffliction(h))
                {
                    toRemove.Add(h);
                }
            }

            foreach (var h in toRemove)
            {
                Pawn.health.RemoveHediff(h);
            }
        }

        /// <summary>
        /// Identifica si un estado es una enfermedad, parásito o infección, sea de vanilla o de un mod.
        /// </summary>
        private bool IsDiseaseOrAffliction(Hediff h)
        {
            if (h == null || h.def == null) return false;

            // 1. BLINDAJE DE SEGURIDAD: Nunca tocar prótesis, implantes, poderes ni heridas físicas
            if (h is Hediff_AddedPart || h is Hediff_Implant) return false;
            if (h is Hediff_Injury || h is Hediff_MissingPart) return false;
            if (h.def.defName.StartsWith("Kayn_")) return false;
            if (h.def == HediffDefOf.PsychicAmplifier) return false;

            // Si es un estado neutro o positivo (ej. bufos, efectos de drogas placenteros), no lo borra
            if (!h.def.isBad) return false;

            // 2. Cualquier enfermedad que use inmunidad (Gripe, malaria, plagas de mods, etc.)
            if (h.TryGetComp<HediffComp_Immunizable>() != null) return true;

            // 3. Infecciones bacterianas de heridas
            if (h.def == HediffDefOf.WoundInfection) return true;

            // 4. Parásitos internos y mecanitas (gusanos intestinales, parásitos musculares, etc.)
            if (h.def.makesSickThought) return true;

            // 5. Enfermedades crónicas o degenerativas (carcinoma/cáncer, asma, demencia, cataratas)
            if (h.def.chronic) return true;

            // 6. Intoxicación alimentaria y venenos/gases tóxicos
            if (h.def == HediffDefOf.FoodPoisoning || h.def == HediffDefOf.ToxicBuildup) return true;

            return false;
        }
    }
}