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
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace TheTailor.Cards.Uncommon
{
    [Pool(typeof(TheTailorCardPool))]
    public class TearThrough() : CustomCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Minion };
        public override string? CustomPortraitPath => "res://TheTailor/images/card_portraits/tearThroughBeta.png";
        public override string? PortraitPath => "res://TheTailor/images/card_portraits/tearThroughBeta.png";
        public override string? BetaPortraitPath => "res://TheTailor/images/card_portraits/tearThroughBeta.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(TheTailor.Keywords.BurlapMinion)];
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.Damage.IntValue)
                .FromCard(this, cardPlay)
                .TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
                
            PetsOrderAccessor accessor = new PetsOrderAccessor(cardPlay.Card.Owner);
            if (accessor != null && accessor.Pets != null)
            {
                for (int i = 0; i < accessor.Pets.Count; i++)
                {
                    if (accessor.Pets[i].Monster is TailorMinion)
                    {
                        await TailorMinionCmd.ReplaceMinion<MinionBurlap>(choiceContext, cardPlay.Card.Owner, i, false, true);
                        break;
                    }
                }
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4);
        }
    }
}