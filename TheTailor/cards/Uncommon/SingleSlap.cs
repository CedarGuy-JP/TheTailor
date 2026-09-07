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
using MegaCrit.Sts2.Core.Logging;

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class SingleSlap() : CustomCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        public override string Title
        {
            get
            {
                LocString titleLocString = TitleLocString;
                LocString prefix = new LocString("cards", "THETAILOR-SLAPAMOUNT-" + Math.Min(DynamicVars["Slaps"].BaseValue, 31));
                bool nonSpaceLanguage = false;

                switch (LocManager.Instance.Language)
                {
                    case "zhs":
                    case "zht":
                    case "jpn":
                    case "kor":
                        nonSpaceLanguage = true;
                        break;
                    default:
                        break;
                }
                
                string space = nonSpaceLanguage ? "" : " ";

                if (!IsUpgraded)
                {
                    return $"{prefix.GetFormattedText()}{space}{titleLocString.GetFormattedText()}";
                }
                else
                {
                    return $"{prefix.GetFormattedText()}{space}{titleLocString.GetFormattedText()}+";
                }
            }
        }

        public override int MaxUpgradeLevel => 99999;
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/singleSlapBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/singleSlapBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/singleSlapBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new DynamicVar("Slaps", 2)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitCount(DynamicVars["Slaps"].IntValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Slaps"].UpgradeValueBy(1);
        }
    }
}