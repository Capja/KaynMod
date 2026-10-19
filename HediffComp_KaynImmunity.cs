using System.Collections.Generic;
using HarmonyLib;
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
            if (Pawn != null && Pawn.health?.hediffSet != null && Pawn.IsHashIntervalTick(300))
            {
                PurgeDiseases();

                // Recuperación instantánea de cualquier colapso mental forzado
                if (Pawn.MentalState != null)
                {
                    Pawn.MentalState.RecoverFromState();
                }
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
        /// Identifica si un estado es una enfermedad, parásito, infección o aflicción psíquica.
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

            // 2. Ataques y aflicciones psíquicas hostiles (Lanzas de choque, supresión mecanoide, resaca)
            if (h.def == HediffDefOf.PsychicShock || 
                h.def == HediffDefOf.PsychicSuppression || 
                h.def == HediffDefOf.PsychicHangover) 
                return true;

            // 3. Cualquier enfermedad que use inmunidad (Gripe, malaria, plagas de mods, etc.)
            if (h.TryGetComp<HediffComp_Immunizable>() != null) return true;

            // 4. Infecciones bacterianas de heridas
            if (h.def == HediffDefOf.WoundInfection) return true;

            // 5. Parásitos internos y mecanitas (gusanos intestinales, parásitos musculares, etc.)
            if (h.def.makesSickThought) return true;

            // 6. Enfermedades crónicas o degenerativas (carcinoma/cáncer, asma, demencia, cataratas)
            if (h.def.chronic) return true;

            // 7. Intoxicación alimentaria y venenos/gases tóxicos
            if (h.def == HediffDefOf.FoodPoisoning || h.def == HediffDefOf.ToxicBuildup) return true;

            return false;
        }
    }

    // Parche integrado: Hace que el zumbido psíquico del mapa sea completamente invisible e inofensivo para Kayn
    [HarmonyPatch(typeof(ThoughtWorker_PsychicDrone), "CurrentStateInternal")]
    public static class Patch_IgnorePsychicDrone
    {
        public static bool Prefix(Pawn p, ref ThoughtState __result)
        {
            if (p?.health?.hediffSet?.HasHediff(DefDatabase<HediffDef>.GetNamedSilentFail("Kayn_Awakened")) == true)
            {
                __result = ThoughtState.Inactive;
                return false;
            }
            return true;
        }
    }
}