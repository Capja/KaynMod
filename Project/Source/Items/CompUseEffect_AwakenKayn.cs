using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class CompUseEffect_AwakenKayn : CompUseEffect
    {
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            if (user == null || user.health?.hediffSet == null) return;

            var hediffDef = HediffDef.Named(CompKayn.AwakenedHediffDefName);
            if (!user.health.hediffSet.HasHediff(hediffDef))
            {
                user.health.AddHediff(hediffDef);
                MoteMaker.ThrowText(user.DrawPos, user.Map, "¡ESENCIA DESPERTADA!", new Color(0.6f, 0.2f, 0.9f));
                FleckMaker.ThrowDustPuff(user.Position.ToVector3Shifted(), user.Map, 2.5f);
                Messages.Message($"{user.LabelShort} ha absorbido el Núcleo de Sombras. La Esencia de Kayn despierta en su interior.", user, MessageTypeDefOf.PositiveEvent);
            }
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (p == null) return false;
            var hediffDef = HediffDef.Named(CompKayn.AwakenedHediffDefName);
            if (p.health?.hediffSet?.HasHediff(hediffDef) == true)
            {
                return "Este ser ya posee la Esencia de Kayn.";
            }
            if (p.RaceProps.IsMechanoid)
            {
                return "Un mecanoide no puede albergar la Esencia.";
            }
            return true;
        }
    }
}