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
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace TheTailor.Powers
{
    public sealed class InspirationPower : CustomPowerModel
    {
        public override string? CustomPackedIconPath => "res://TheTailor/images/powers/inspirationSmall.png";
        public override string? CustomBigIconPath => "res://TheTailor/images/powers/inspiration.png";
        public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/inspiration.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Cards.Token.Patch>(), HoverTipFactory.FromEnchantment<Sharp>(3).First()];
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.IsClone || card is not Cards.Token.Patch || card.Owner != Owner.Player)
            {
                return;
            }

            Flash();
            CardCmd.Enchant<Sharp>(card, Amount);
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side == CombatSide.Player)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}