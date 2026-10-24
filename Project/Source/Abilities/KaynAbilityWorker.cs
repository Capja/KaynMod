using RimWorld;
using Verse;

namespace KaynMod
{
    public class KaynAbilityWorker
    {
        // El radio máximo seguro en RimWorld antes de agotar la lista precalculada radial es de ~56 celdas.
        private const float MaxSafeRadius = 55f;

        public virtual bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            return true;
        }

        public virtual void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            MoteMaker.ThrowText(target.CenterVector3, caster.Map, "Pulso Sutil", UnityEngine.Color.cyan);
        }

        public virtual void DrawHighlight(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            // --- 1. DIBUJAR EL ALCANCE DE LA HABILIDAD ---
            if (def.range > MaxSafeRadius)
            {
                // Si la habilidad tiene rango 60, 100 o 600, dibuja de forma segura el anillo al tamaño máximo permitido
                GenDraw.DrawRadiusRing(caster.Position, MaxSafeRadius);
            }
            else if (def.range > 0f)
            {
                // Si el rango es corto (ej. rango 15), se dibuja a su tamaño real
                GenDraw.DrawRadiusRing(caster.Position, def.range);
            }

            // --- 2. DIBUJAR EL RADIO DE LA EXPLOSIÓN ---
            if (def.explosionRadius > 0f && target.IsValid)
            {
                if (def.explosionRadius > MaxSafeRadius)
                {
                    // Límite de seguridad también para explosiones masivas
                    GenDraw.DrawRadiusRing(target.Cell, MaxSafeRadius);
                }
                else
                {
                    GenDraw.DrawRadiusRing(target.Cell, def.explosionRadius);
                }
            }
        }
    }
}
