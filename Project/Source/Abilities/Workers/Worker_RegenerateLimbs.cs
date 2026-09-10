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
                // Permite a uno mismo o a cualquier miembro de la colonia
                return targetPawn == caster || (targetPawn.Faction != null && targetPawn.Faction == caster.Faction);
            }
            return false;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Pawn p) || p.health?.hediffSet == null) return;

            // Busca extremidades u órganos perdidos
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

            MoteMaker.ThrowText(p.DrawPos, p.Map, "¡Miembros Regenerados!", UnityEngine.Color.green);
            FleckMaker.ThrowDustPuff(p.Position.ToVector3Shifted(), p.Map, 2.0f);
            Messages.Message($"{p.LabelShort} ha regenerado {count} parte(s) de su cuerpo.", p, MessageTypeDefOf.PositiveEvent);
        }
    }
}