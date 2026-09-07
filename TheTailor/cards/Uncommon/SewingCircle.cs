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
using MinionLib.Utilities;
using MegaCrit.Sts2.Core.Entities.Creatures;
using TheTailor.Minions;

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class SewingCircle() : CustomCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override bool HasEnergyCostX => true;
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Minion };
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/sewingCircleBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/sewingCircleBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/sewingCircleBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(3)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            await TailorMinionCmd.GiveMinionHealth<TailorMinion>(choiceContext, cardPlay.Card.Owner, DynamicVars.Heal.IntValue * ResolveEnergyXValue(), TailorMinionCmd.MinionTriggerType.All);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Heal.UpgradeValueBy(1);
        }
    }
}