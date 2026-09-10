using Verse;

namespace KaynMod
{
    public class Command_KaynAbility : Command_Action
    {
        public Pawn caster;
        public KaynAbilityDef abilityDef;

        public Command_KaynAbility(Pawn caster, KaynAbilityDef def)
        {
            this.caster = caster;
            this.abilityDef = def;
            defaultLabel = def.label;
            defaultDesc = def.description;
            icon = def.UIIcon;
            action = () => KaynTargeting.Begin(caster, def);
        }

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();
            if (caster != null && abilityDef != null)
            {
                abilityDef.Worker.DrawHighlight(abilityDef, caster, LocalTargetInfo.Invalid);
            }
        }
    }
}