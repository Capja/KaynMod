using RimWorld;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class Worker_ShadowScythe : KaynAbilityWorker
    {
        public const string ScytheDefName = "Kayn_ShadowScythe";

        public override bool CanTarget(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            // Solo se puede invocar en uno mismo
            return target.Thing == caster;
        }

        public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
        {
            if (!(target.Thing is Pawn p) || p.equipment == null) return;

            var scytheDef = ThingDef.Named(ScytheDefName);
            ThingWithComps currentWeapon = p.equipment.Primary;

            // 1. CASO DISIPAR: Si ya tiene la guadaña en la mano, SE DISUELVE
            if (currentWeapon != null && currentWeapon.def == scytheDef)
            {
                p.equipment.DestroyEquipment(currentWeapon);

                FleckMaker.ThrowDustPuff(p.Position.ToVector3Shifted(), p.Map, 2.0f);
                MoteMaker.ThrowText(p.DrawPos, p.Map, "Guadaña Disipada", Color.gray);
                Messages.Message($"{p.LabelShort} disuelve la Guadaña de Sombras y vuelve a estar desarmado.", p, MessageTypeDefOf.NeutralEvent);
                return;
            }

            // 2. Si tenía otra arma normal empuñada, la suelta al suelo
            if (currentWeapon != null)
            {
                p.equipment.TryDropEquipment(currentWeapon, out _, p.Position, false);
            }

            // 3. CASO MANIFESTAR: Crear y equipar la guadaña
            ThingWithComps scythe = (ThingWithComps)ThingMaker.MakeThing(scytheDef);
            p.equipment.AddEquipment(scythe);

            FleckMaker.ThrowDustPuff(p.Position.ToVector3Shifted(), p.Map, 2.5f);
            MoteMaker.ThrowText(p.DrawPos, p.Map, "¡Guadaña de Sombras!", new Color(0.6f, 0.2f, 0.9f));
            Messages.Message($"{p.LabelShort} ha manifestado su Guadaña de Sombras.", p, MessageTypeDefOf.PositiveEvent);
        }
    }
}