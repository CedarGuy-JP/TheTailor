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
using TheTailor.Character;

namespace TheTailor.Cards.Common
{
    [Pool(typeof(TheTailorCardPool))]
    public class Spool() : CustomCardModel(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/spoolBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/spoolBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/spoolBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move), new DynamicVar("Delicate", 2)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.Stitch), HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            IEnumerable<CardModel> cardModel = await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefsExtensions.StitchSelectionPrompt, 2), context: choiceContext, player: Owner, filter: StitchCmd.CanBeStitched, source: this);
            if (cardModel != null && cardModel.Count() == 2)
            {
                await StitchCmd.StitchCards(cardModel.ElementAt(0), cardModel.ElementAt(1));
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3);
            DynamicVars["Delicate"].UpgradeValueBy(1);
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}