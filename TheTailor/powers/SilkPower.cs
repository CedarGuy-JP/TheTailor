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
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheTailor.Powers
{
    public sealed class SilkPower : CustomPowerModel
    {
        public override string? CustomPackedIconPath => "res://TheTailor/images/powers/silkSmall.png";
        public override string? CustomBigIconPath => "res://TheTailor/images/powers/silk.png";
        public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/silk.png";
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];

        public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
        {
            if (Owner.IsPet && Owner.PetOwner == player)
            {
                Flash();
                await PowerCmd.Apply<SilkDexterityPower>(choiceContext, player.Creature, Amount, Owner, null);
            }
        }
    }
}