using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class Worker_Slumber : KaynAbilityWorker
    {
        public const string SlumberHediffDefName = "Kayn_Slumber";

        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (target.Thing is Pawn targetPawn)
            {
                // PROHIBIDO A SÍ MISMO: El ratón se pondrá en rojo si pasas por encima de Kayn
                if (targetPawn == caster) return false;

                // Solo a otros seres biológicos vivos (enemigos o aliados)
                return !targetPawn.Dead && !targetPawn.RaceProps.IsMechanoid;
            }
            return false;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            // Si por algún motivo intentara aplicarse a Kayn, lo ignora
            if (!(target.Thing is Pawn targetPawn) || targetPawn == caster || targetPawn.health == null) return;

            // 1. Aplicar el trance de sueño
            var slumberDef = HediffDef.Named(SlumberHediffDefName);
            targetPawn.health.AddHediff(slumberDef);

            // 2. Interrumpir cualquier acción actual
            targetPawn.jobs?.StopAll(false);

            // 3. Calmar colapsos mentales
            if (targetPawn.MentalState != null)
            {
                targetPawn.MentalState.RecoverFromState();
            }

            // 4. Rellenar descanso
            if (targetPawn.needs?.rest != null)
            {
                targetPawn.needs.rest.CurLevel = 1.0f;
            }

            FleckMaker.ThrowDustPuff(targetPawn.Position.ToVector3Shifted(), targetPawn.Map, 1.5f);
            MoteMaker.ThrowText(targetPawn.DrawPos, targetPawn.Map, "Zzz...", Color.cyan);
            Messages.Message($"{targetPawn.LabelShort} ha caído en un letargo profundo.", targetPawn, MessageTypeDefOf.NeutralEvent);
        }
    }
}