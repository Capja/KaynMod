using System.Collections.Generic;
using RimWorld;
using Verse;

namespace KaynMod
{
    public class CompKayn : ThingComp
    {
        public Pawn Pawn => (Pawn)parent;
        public const string AwakenedHediffDefName = "Kayn_Awakened";

        public bool IsAwakened => Pawn?.health?.hediffSet?.HasHediff(HediffDef.Named(AwakenedHediffDefName)) == true;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!IsAwakened) yield break;

            // 1. Selector de Estado dinámico (auto-descubre todos los KaynStateDef)
            yield return new Gizmo_KaynStateSelector(Pawn);

            // 2. Habilidades de Kayn (auto-descubre todos los KaynAbilityDef)
            foreach (var def in DefDatabase<KaynAbilityDef>.AllDefs)
            {
                yield return new Command_KaynAbility(Pawn, def);
            }
        }
    }
}