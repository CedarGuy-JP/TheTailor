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

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class MedicalKit() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/medicalKitBeta.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Token.Patch>(), HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate)];
        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            await Token.Patch.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);

            List<CardModel> list = PileType.Hand.GetPile(Owner).Cards.Where((CardModel c) => c != null && c.IsTransformable && c.Type == CardType.Status).ToList();
            List<CardTransformation> list2 = new List<CardTransformation>();
            foreach (CardModel item in list)
            {
                CardModel cardModel = CombatState.CreateCard<Token.Patch>(Owner);
                list2.Add(new CardTransformation(item, cardModel));
            }
            await CardCmd.Transform(list2, null);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}