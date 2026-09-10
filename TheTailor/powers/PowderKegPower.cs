using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Minion;
using MinionLib.Powers.Patches;
using HarmonyLib;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Players;
using TheTailor.Minions;
using MegaCrit.Sts2.Core.Rooms;
using BaseLib.Common.Rewards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheTailor.Powers
{
    public sealed class PowderKegPower : CustomPowerModel
    {
        public override string? CustomPackedIconPath => "res://TheTailor/images/powers/powderKegSmall.png";
        public override string? CustomBigIconPath => "res://TheTailor/images/powers/powderKeg.png";
        public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/powderKeg.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(Keywords.LeatherMinion), HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion)];
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
        {
            if (creature.PetOwner != null && Owner == creature.PetOwner.Creature && (creature.Monster is MinionLeather || creature.Monster is MinionBurlap))
            {
                Flash();
                await Cmd.CustomScaledWait(0.2f, 0.4f);
                foreach (Creature hittableEnemy in CombatState.HittableEnemies)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
                }
                await Cmd.CustomScaledWait(0.2f, 0.4f);
                await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, new DamageVar(Amount, ValueProp.Unpowered), Owner);
            }
        }
    }
}