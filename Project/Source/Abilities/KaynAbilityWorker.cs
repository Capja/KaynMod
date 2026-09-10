using Verse;

namespace KaynMod
{
    public abstract class KaynAbilityWorker
    {
        // Valida si se puede hacer clic sobre ese objetivo
        public virtual bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            return true;
        }

        // Ejecuta el efecto real de la habilidad
        public abstract void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target);

        // Dibuja el alcance y áreas de explosión (comportamiento por defecto)
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