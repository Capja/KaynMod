using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public static class PerceptionEvaluator
    {
        /// <summary>
        /// Comprueba matemáticamente si un punto está dentro del cono frontal del peón (180 grados).
        /// </summary>
        public static bool IsInFieldOfView(Pawn observer, IntVec3 targetCell)
        {
            // Si están en la misma celda, se asume dentro del campo
            if (observer.Position == targetCell) return true;

            // Dirección hacia la que mira el observador (Norte, Sur, Este, Oeste)
            Vector2 facing = observer.Rotation.AsVector2;

            // Vector que va desde el observador hasta el objetivo
            Vector2 toTarget = new Vector2(targetCell.x - observer.Position.x, targetCell.z - observer.Position.z).normalized;

            // Producto escalar (Dot product):
            // > 0 significa que está en los 180° de su frente (lo ve).
            // <= 0 significa que está a su espalda / punto ciego (no lo ve).
            return Vector2.Dot(facing, toTarget) > 0f;
        }

        public static WitnessRecord EvaluatePawn(Pawn witness, KaynAbilityEvent ev)
        {
            var record = new WitnessRecord { Pawn = witness };

            // 1. Estados que impiden totalmente percibir (muerto, incapacitado, durmiendo)
            if (witness.Dead || witness.Downed || !witness.Awake())
            {
                record.CanPerceiveKayn = false;
                record.CanPerceiveEffect = false;
                record.CorrelatesToKayn = false;
                record.PerceptionReason = "Inconsciente/Durmiendo";
                return record;
            }

            float sightLevel = witness.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
            float hearingLevel = witness.health.capacities.GetLevel(PawnCapacityDefOf.Hearing);

            // --- EVALUAR PERCEPCIÓN DE KAYN ---
            bool isKaynInvisible = InvisibilityUtility.IsPsychologicallyInvisible(ev.Kayn);
            float distToKayn = witness.Position.DistanceTo(ev.Kayn.Position);
            bool losToKayn = GenSight.LineOfSight(witness.Position, ev.Kayn.Position, ev.Map, true);
            bool fovKayn = IsInFieldOfView(witness, ev.Kayn.Position); // ¿Está en su frente o a su espalda?

            // Kayn es visto si: la acción es visible, no es invisible, hay vista, distancia, línea de visión Y cono de visión
            if (!isKaynInvisible && ev.Ability.isKaynActionVisible && sightLevel > 0.1f && distToKayn <= ev.Ability.visualPerceptionRadius && losToKayn && fovKayn)
            {
                record.CanPerceiveKayn = true;
            }
            else
            {
                record.CanPerceiveKayn = false;

                // Anotación en debug para saber por qué falló
                if (!fovKayn && losToKayn && !isKaynInvisible)
                {
                    record.PerceptionReason = "De espaldas a Kayn (Punto ciego)";
                }
            }

            // --- EVALUAR PERCEPCIÓN DEL EFECTO ---
            float distToEffect = witness.Position.DistanceTo(ev.EffectPosition);
            bool losToEffect = GenSight.LineOfSight(witness.Position, ev.EffectPosition, ev.Map, true);
            bool fovEffect = IsInFieldOfView(witness, ev.EffectPosition);

            bool sawEffect = false;
            bool heardEffect = false;

            // Percepción visual del efecto: requiere estar en su cono de visión
            if (ev.Ability.hasVisualEffect && sightLevel > 0.1f && distToEffect <= ev.Ability.visualPerceptionRadius && losToEffect && fovEffect)
            {
                sawEffect = true;
            }

            // Percepción acústica (Oído): El sonido NO depende de la espalda ni de los ojos
            if (ev.Ability.acousticRadius > 0f && hearingLevel > 0.1f && distToEffect <= ev.Ability.acousticRadius)
            {
                heardEffect = true;
            }

            // Si el testigo es el propio objetivo directo, siente el impacto en su cuerpo
            bool isDirectTarget = (ev.Target.Thing == witness);
            if (isDirectTarget)
            {
                sawEffect = true;
            }

            record.CanPerceiveEffect = sawEffect || heardEffect;

            // --- EVALUAR CORRELACIÓN ---
            if (record.CanPerceiveKayn && record.CanPerceiveEffect)
            {
                record.CorrelatesToKayn = true;
            }
            else if (isDirectTarget && distToKayn <= 1.5f)
            {
                record.CorrelatesToKayn = true;
                record.PerceptionReason = "Contacto cuerpo a cuerpo";
            }
            else
            {
                record.CorrelatesToKayn = false;
            }

            return record;
        }
    }
}