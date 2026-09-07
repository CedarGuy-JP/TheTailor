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

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class WhirlingSilk() : CustomCardModel(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Minion };
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/whirlingSilkBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/whirlingSilkBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/whirlingSilkBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Delicate", -999), new DamageVar(3m, ValueProp.Move)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.SilkMinion), IsUpgraded ? HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate) : HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .TargetingAllOpponents(CombatState)
                .WithHitCount(2)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await TailorMinionCmd.AddOrReplaceMinion<MinionSilk>(choiceContext, Owner, true);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
            DynamicVars["Delicate"].BaseValue = 2;
            DynamicVars.Damage.UpgradeValueBy(1m);
        }
    }
}