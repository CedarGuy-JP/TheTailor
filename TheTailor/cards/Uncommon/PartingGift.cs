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
using TheTailor.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using TheTailor.Character;

namespace TheTailor.Cards.Uncommonz
{
    [Pool(typeof(TheTailorCardPool))]
    public class PartingGift() : CustomCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        public override bool GainsBlock => false;
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/partingGiftBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/partingGiftBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/partingGiftBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Unpowered)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
            await PowerCmd.Apply<PartingGiftPower>(choiceContext, Owner.Creature, DynamicVars.Block.IntValue, Owner.Creature, this, false);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2m);
        }
    }
}