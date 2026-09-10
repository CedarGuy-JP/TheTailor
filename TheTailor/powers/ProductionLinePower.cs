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
using MinionLib.Commands;

namespace TheTailor.Powers
{
    public sealed class ProductionLinePower : CustomPowerModel
    {
        public override string? CustomPackedIconPath => "res://TheTailor/images/powers/productionLineSmall.png";
        public override string? CustomBigIconPath => "res://TheTailor/images/powers/productionLine.png";
        public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/productionLine.png";
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion)];

        public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (Owner == player.Creature)
            {
                for (int i = 0; i < Amount; i++)
                {
                    if (await TailorMinionCmd.AddMinion<MinionLinen>(choiceContext, player))
                    {
                        Flash();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}