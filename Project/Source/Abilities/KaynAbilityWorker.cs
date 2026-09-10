using RimWorld; // <-- AÑADE ESTA LÍNEA
using Verse;

namespace KaynMod
{
    public class KaynAbilityWorker
    {
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
            GenDraw.DrawRadiusRing(caster.Position, def.range);
            if (def.explosionRadius > 0f && target.IsValid)
            {
                GenDraw.DrawRadiusRing(target.Cell, def.explosionRadius);
            }
        }
    }
}