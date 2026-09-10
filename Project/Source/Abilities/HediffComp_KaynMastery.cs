using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class HediffCompProperties_KaynMastery : HediffCompProperties
    {
        public HediffCompProperties_KaynMastery()
        {
            compClass = typeof(HediffComp_KaynMastery);
        }
    }

    public class HediffComp_KaynMastery : HediffComp
    {
        public override void CompPostMake()
        {
            base.CompPostMake();
            MaximizeSkills();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            // Revisa cada segundo (60 ticks) para anular la degradación de XP del juego
            if (Pawn != null && Pawn.skills != null && Pawn.IsHashIntervalTick(60))
            {
                MaximizeSkills();
            }
        }

        private void MaximizeSkills()
        {
            if (Pawn?.skills?.skills == null) return;

            foreach (SkillRecord skill in Pawn.skills.skills)
            {
                if (skill.Level < 20)
                {
                    skill.Level = 20;
                }
                // Mantiene la barra de XP al máximo para que nunca caiga a nivel 19
                skill.xpSinceLastLevel = Mathf.Max(skill.xpSinceLastLevel, 1000f);
            }
        }
    }
}