using System.Linq;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class Worker_RegenerateLimbs : KaynAbilityWorker
    {
        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (target.Thing is Pawn targetPawn)
            {
                return targetPawn == caster || (targetPawn.Faction != null && targetPawn.Faction == caster.Faction);
            }
            return false;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Pawn p) || p.health?.hediffSet == null) return;

            var missingParts = p.health.hediffSet.GetMissingPartsCommonAncestors().ToList();

            if (missingParts.Count == 0)
            {
                Messages.Message($"{p.LabelShort} no tiene ningún miembro perdido que regenerar.", p, MessageTypeDefOf.NeutralEvent);
                return;
            }

            int count = missingParts.Count;
            foreach (var missing in missingParts)
            {
                p.health.RestorePart(missing.Part);
            }

            // === ARREGLO VISUAL: Fuerza al motor a redibujar cabeza, pelo, ojos y extremidades ===
            p.Drawer?.renderer?.SetAllGraphicsDirty();
            PortraitsCache.SetDirty(p);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(p);

            MoteMaker.ThrowText(p.DrawPos, p.Map, "¡Miembros Regenerados!", UnityEngine.Color.green);
            FleckMaker.ThrowDustPuff(p.Position.ToVector3Shifted(), p.Map, 2.0f);
            Messages.Message($"{p.LabelShort} ha regenerado {count} parte(s) de su cuerpo.", p, MessageTypeDefOf.PositiveEvent);
        }
    }
}