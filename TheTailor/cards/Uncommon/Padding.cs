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
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class Padding() : CustomCardModel(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override bool GainsBlock => true;
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Minion };
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/paddingBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/paddingBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/paddingBeta.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.Delicate), HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion), HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion)];
        protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move), new DynamicVar("Delicate", 2)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            await TailorMinionCmd.AddOrReplaceMinion<MinionBurlap>(choiceContext, Owner, true);
            await TailorMinionCmd.AddOrReplaceMinion<MinionLinen>(choiceContext, Owner, true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2);
            DynamicVars["Delicate"].UpgradeValueBy(1);
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}