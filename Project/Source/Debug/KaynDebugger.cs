using System.Text;
using RimWorld;  
using Verse;

namespace KaynMod
{
    public static class KaynDebugger
    {
        public static void PrintEventLog(KaynAbilityEvent ev)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("================ [KAYN DEBUG] ================");
            sb.AppendLine($"Ability Used: {ev.Ability.defName}");
            sb.AppendLine($"Kayn Position: {ev.Kayn.Position} | Target Position: {ev.EffectPosition}");
            sb.AppendLine($"Kayn Invisible Hediff: {InvisibilityUtility.IsPsychologicallyInvisible(ev.Kayn)}");
            sb.AppendLine($"Action Visually Obvious: {ev.Ability.isKaynActionVisible}");
            sb.AppendLine($"Effect Visual: {ev.Ability.hasVisualEffect} | Acoustic Radius: {ev.Ability.acousticRadius}");
            sb.AppendLine("----------------------------------------------");

            int correlatedCount = 0;
            int sawEffectCount = 0;

            foreach (var w in ev.Witnesses)
            {
                sb.AppendLine($"Pawn: {w.Pawn.LabelShort}");
                sb.AppendLine($"  - Can perceive Kayn:   {w.CanPerceiveKayn}");
                sb.AppendLine($"  - Can perceive effect: {w.CanPerceiveEffect}");
                sb.AppendLine($"  - Correlates to Kayn:  {w.CorrelatesToKayn}");
                if (!string.IsNullOrEmpty(w.PerceptionReason))
                {
                    sb.AppendLine($"  - Note: {w.PerceptionReason}");
                }

                if (w.CanPerceiveEffect) sawEffectCount++;
                if (w.CorrelatesToKayn) correlatedCount++;
            }

            sb.AppendLine("----------------------------------------------");
            sb.AppendLine($"Total Witnesses (Saw Effect): {sawEffectCount}");
            sb.AppendLine($"Direct Accusers (Know it was Kayn): {correlatedCount}");
            sb.AppendLine("==============================================");

            var tracker = Current.Game.GetComponent<GameComponent_KaynTracker>();
            int totalActiveEvents = tracker != null ? tracker.recordedEvents.Count : 0;
            sb.AppendLine($"[MEMORIA PERSISTENTE] Eventos activos en el mundo: {totalActiveEvents}");

            Log.Message(sb.ToString());
        }
    }
}