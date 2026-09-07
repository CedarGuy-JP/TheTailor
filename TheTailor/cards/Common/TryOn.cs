using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTailor;
using TheTailor.Extensions;
using TheTailor.Cards;
using TheTailor.Minions;
using MinionLib.Commands;
using MinionLib.Minion;
using TheTailor.Character;
using MinionLib.Utilities;
using MegaCrit.Sts2.Core.Extensions;

namespace TheTailor.Cards.Common
{
    [Pool(typeof(TheTailorCardPool))]
    public class TryOn() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override bool GainsBlock => true;
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/tryOnBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/tryOnBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/tryOnBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move), new CardsVar(2)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            int num = Math.Min(DynamicVars.Cards.IntValue, CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count);
            IEnumerable<CardModel> enumerable = PileType.Discard.GetPile(Owner).Cards.TakeRandom(DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardSelection);
            foreach (CardModel item in enumerable)
            {
                if (num <= 0) { break; }
                await CardPileCmd.Add(item, PileType.Hand.GetPile(Owner), CardPilePosition.Bottom);
                num--;
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}