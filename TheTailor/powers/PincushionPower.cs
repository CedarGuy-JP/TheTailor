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
using MegaCrit.Sts2.Core.Entities.Cards;
using TheTailor.Cards.Common;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheTailor.Powers
{
    public sealed class PincushionPower : CustomPowerModel
    {
        public override string? CustomPackedIconPath => "res://TheTailor/images/powers/pincushionSmall.png";
        public override string? CustomBigIconPath => "res://TheTailor/images/powers/pincushion.png";
        public override string? CustomBigBetaIconPath => "res://TheTailor/images/powers/pincushion.png";
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (Owner.IsAlive && Owner == target && result.TotalDamage > 0 && result.Props.HasFlag(ValueProp.Move) && dealer != null && dealer.Side == CombatSide.Player)
            {
                if (dealer.PetOwner != null)
                {
                    await PowerCmd.Apply<PincushionStrengthPower>(choiceContext, dealer.PetOwner.Creature, Amount, dealer, null);
                }
                else
                {
                    await PowerCmd.Apply<PincushionStrengthPower>(choiceContext, dealer, Amount, dealer, null);
                }
            }
        }

        public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
        {
            if (Owner == creature && Applier != null)
            {
                await PowerCmd.Apply<PincushionStrengthPower>(choiceContext, Applier, Amount, Applier, null);
            }
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (participants.Contains(Owner))
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}