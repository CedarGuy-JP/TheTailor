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
using BaseLib.Extensions;
using TheTailor.Relics.Common;

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class XXL() : CustomCardModel(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override int MaxUpgradeLevel => 99999;
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Minion };
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/xxlBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/xxlBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/xxlBeta.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(12)];
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.LinenMinion), HoverTipFactory.FromKeyword(TheTailor.Keywords.Premium)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int extraMaxHp = 0;
            foreach (RelicModel relicModel in Owner.Relics)
            {
                if (relicModel is HeavyDuty)
                {
                    extraMaxHp++;
                }
            }

            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
            await TailorMinionCmd.AddOrReplaceMinion<MinionLinen>(choiceContext, Owner, true, maxHpOverride: DynamicVars.Heal.IntValue + extraMaxHp);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Heal.UpgradeValueBy(4);
        }
    }
}