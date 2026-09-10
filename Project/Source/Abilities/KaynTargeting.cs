using System;
using RimWorld;
using Verse;

namespace KaynMod
{
    public static class KaynTargeting
    {
        public static void Begin(Pawn caster, KaynAbilityDef ability)
        {
            var targetParams = new TargetingParameters
            {
                canTargetPawns = true,
                canTargetLocations = !ability.targetMustBePawn,
                canTargetBuildings = false,
                canTargetSelf = true,
                validator = (TargetInfo t) =>
                {
                    if (t.Cell.DistanceTo(caster.Position) > ability.range) return false;
                    LocalTargetInfo localTarget = t.HasThing ? new LocalTargetInfo(t.Thing) : new LocalTargetInfo(t.Cell);
                    return ability.Worker.CanTarget(ability, caster, localTarget);
                }
            };

            Action<LocalTargetInfo> onHighlight = (LocalTargetInfo t) =>
            {
                ability.Worker.DrawHighlight(ability, caster, t);
            };

            Find.Targeter.BeginTargeting(
                targetParams,
                (LocalTargetInfo target) => Execute(caster, ability, target),
                onHighlight,
                null,
                caster
            );
        }

        public static void Execute(Pawn caster, KaynAbilityDef ability, LocalTargetInfo target)
        {
            var ev = new KaynAbilityEvent(caster, ability, target);

            // 1. Efecto de la habilidad
            ability.Worker.Apply(ability, caster, target);

            // 2. Escaneo de percepción (solo humanos)
            foreach (Pawn witness in caster.Map.mapPawns.AllPawnsSpawned)
            {
                if (witness == caster || !witness.RaceProps.Humanlike) continue;
                var record = PerceptionEvaluator.EvaluatePawn(witness, ev);
                ev.Witnesses.Add(record);
            }

            // 3. Guardado en la memoria y consola
            Current.Game?.GetComponent<GameComponent_KaynTracker>()?.RecordEvent(ev);
            KaynDebugger.PrintEventLog(ev);
        }
    }
}