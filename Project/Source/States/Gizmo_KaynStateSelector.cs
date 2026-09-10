using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class Gizmo_KaynStateSelector : Command_Action
    {
        private readonly Pawn pawn;

        public Gizmo_KaynStateSelector(Pawn pawn)
        {
            this.pawn = pawn;

            var currentState = GetCurrentState();
            defaultLabel = $"Esencia: {currentState?.label ?? "Desconocido"}";
            defaultDesc = $"{currentState?.description}\n\nClic izquierdo: Avanzar estado.\nClic derecho: Elegir directamente.";
            defaultIconColor = currentState?.iconColor ?? Color.white;
            icon = currentState?.Icon ?? ContentFinder<Texture2D>.Get("UI/Commands/DesirePower", false);

            // Clic izquierdo: pasa al siguiente estado en el ciclo
            action = CycleNextState;
        }

        // Propiedad nativa de RimWorld: menú desplegable con clic derecho
        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
        {
            get
            {
                foreach (var state in DefDatabase<KaynStateDef>.AllDefs.OrderBy(s => s.order))
                {
                    yield return new FloatMenuOption(state.label, () => ApplyState(state));
                }
            }
        }

        private KaynStateDef GetCurrentState()
        {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named(CompKayn.AwakenedHediffDefName));
            float severity = hediff?.Severity ?? 0f;

            return DefDatabase<KaynStateDef>.AllDefs
                .OrderByDescending(s => s.hediffSeverity)
                .FirstOrDefault(s => severity >= s.hediffSeverity) 
                ?? DefDatabase<KaynStateDef>.AllDefs.OrderBy(s => s.order).FirstOrDefault();
        }

        private void CycleNextState()
        {
            var states = DefDatabase<KaynStateDef>.AllDefs.OrderBy(s => s.order).ToList();
            if (states.Count == 0) return;

            var current = GetCurrentState();
            int currentIndex = states.IndexOf(current);
            int nextIndex = (currentIndex + 1) % states.Count;

            ApplyState(states[nextIndex]);
        }

        private void ApplyState(KaynStateDef state)
        {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named(CompKayn.AwakenedHediffDefName));
            if (hediff == null) return;

            hediff.Severity = state.hediffSeverity;

            if (!string.IsNullOrEmpty(state.moteText))
            {
                MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, state.moteText, state.moteColor);
            }
        }
    }
}