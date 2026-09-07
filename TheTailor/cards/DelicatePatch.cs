
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
using HarmonyLib;
using TheTailor.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MinionLib.Utilities.BetterExtraArgs;
using Godot;

namespace TheTailor.Cards
{
    /// <summary>
    ///     Decrements Delicate on card play and Exhausts cards when it reaches 0
    /// </summary>
    [HarmonyPatch]
    internal static class DelicatePatch
    {
        [HarmonyPatch(typeof(AbstractModel), "AfterCardPlayed")]
        internal static async void Postfix(PlayerChoiceContext choiceContext, CardPlay cardPlay, AbstractModel __instance)
        {
            if (__instance is CardModel && cardPlay.Card == __instance && cardPlay.Card.DynamicVars.ContainsKey("Delicate"))
            {
                int extraDelicacies = 0;
                extraDelicacies += cardPlay.Card.Owner.Creature.GetPowerAmount<SteadyHandPower>();

                cardPlay.Card.DynamicVars["Delicate"].BaseValue -= 1;
                if (cardPlay.Card.DynamicVars["Delicate"].IntValue + extraDelicacies <= 1)
                {
                    cardPlay.Card.AddKeyword(CardKeyword.Exhaust);
                }
            }
        }
    }

    /// <summary>
    ///     Hides the 'Exhaust' keyword when a card has Delicate
    /// </summary>
    [HarmonyPatch]
    internal static class DelicateHideExhaustPatch
    {
        [HarmonyPatch(typeof(CardModel), "GetDescriptionForPile", argumentTypes: [typeof(PileType), typeof(CardModel.DescriptionPreviewType), typeof(Creature)])]
        internal static string Postfix(string __result, PileType pileType, DescriptionPreviewType previewType, Creature? target, CardModel __instance)
        {
            if (__instance.DynamicVars.ContainsKey("Delicate") && __instance.DynamicVars["Delicate"].BaseValue > -999 && __instance.Keywords.Contains(CardKeyword.Exhaust))
            {
                string exhaustString = "\n" + CardKeyword.Exhaust.GetCardText();
                int exhaustStringIndex = __result.Find(exhaustString);
                if (exhaustStringIndex > -1)
                {
                    __result = __result.Remove(exhaustStringIndex, exhaustString.Length);
                }
            }

            return __result;
        }
    }
}