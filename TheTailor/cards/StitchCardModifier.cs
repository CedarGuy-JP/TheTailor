using System.Collections.ObjectModel;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Localization;
using BaseLib.Patches.Saves;
using BaseLib.Utils;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheTailor;
using TheTailor.Cards;
using TheTailor.Cards.Rare;

namespace TheTailor.Cards
{
    public class StitchCardModifier : CardModifier
    {
        private CardModel? _stitchedCard;
        public CardModel? StitchedCard
        {
            get
            {
                return _stitchedCard;
            }
            set
            {
                if (_stitchedCard != null)
                {
                    Log.Error("Card cannot be stitched twice");
                    return;
                }
                if (value == null)
                {
                    Log.Error("Attempt to stitch null card");
                    return;
                }

                _stitchedCard = value;
                _stitchedCard.AddKeyword(Keywords.Stitched);
            }
        }

        public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card == Owner && !StitchTrackAutoplaySingleton.BlockedFromAutoplay.Contains(Owner) && Owner != null && StitchedCard != null && cardPlay.PlayIndex == 0)
            {
                TailorStitchQueueSingleton.BeingPlayed.Add(Owner);
                TailorStitchQueueSingleton.BeingPlayed.Add(StitchedCard);

                Creature? target = GetTarget(StitchedCard, StitchedCard.CombatState);
                if (cardPlay.Target != null && cardPlay.Target.IsAlive && target != null && cardPlay.Card.TargetType == StitchedCard.TargetType)
                {
                    target = cardPlay.Target;
                }
                await CardCmd.AutoPlay(choiceContext, StitchedCard, target, StitchedAutoPlayType.Stitched);
            }
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            TailorStitchQueueSingleton.BeingPlayed.Remove(cardPlay.Card);

            if (cardPlay.Card != Owner)
            {
                return;
            }

            if (StitchedCard == null || StitchedCard.Pile == null || !StitchedCard.IsInCombat)
            {
                if (!TailorStitchQueueSingleton.BeingPlayed.Contains(Owner))
                {
                    await StitchCmd.UnstitchCard(Owner);
                }
            }
            else if (Owner.Type == CardType.Power || Owner.Pile.Type == PileType.Exhaust || StitchedCard.Pile.Type == PileType.Exhaust || Owner.Pile.Type == PileType.None || StitchedCard.Pile.Type == PileType.None)
            {
                if (!TailorStitchQueueSingleton.BeingPlayed.Contains(StitchedCard))
                {
                    await StitchCmd.UnstitchCard(StitchedCard);
                }
                if (!TailorStitchQueueSingleton.BeingPlayed.Contains(Owner))
                {
                    await StitchCmd.UnstitchCard(Owner);
                }
            }
        }

        public static Creature? GetTarget(CardModel card, ICombatState combatState)
        {
            Rng combatTargets = card.Owner.RunState.Rng.CombatTargets;
            return card.TargetType switch
            {
                TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(),
                TargetType.AnyAlly => combatTargets.NextItem(combatState.Allies.Where((Creature c) => c != null && c.IsAlive && c.IsPlayer && c != card.Owner.Creature)),
                TargetType.AnyPlayer => card.Owner.Creature,
                TargetType.Self => card.Owner.Creature,
                _ => null,
            };
        }
    }

    [HarmonyPatch]
    internal static class StitchHovertipPatch
    {
        [HarmonyPatch(typeof(CardModel), "HoverTips", MethodType.Getter)]
        internal static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
        {
            StitchCardModifier? cardStitch = __instance.GetModifier<StitchCardModifier>();
            if (cardStitch != null && cardStitch.StitchedCard != null && __instance.IsMutable)
            {
                __result = [.. __result, .. new IHoverTip[1] { new CardHoverTip(cardStitch.StitchedCard) }];
            }

            return __result;
        }
    }

    [HarmonyPatch]
    internal static class StitchRemovePatch
    {
        [HarmonyPatch(typeof(AbstractModel), "AfterCardChangedPiles")]
        internal static async void Postfix(CardModel card, PileType oldPileType, AbstractModel? clonedBy, AbstractModel __instance)
        {
            if (card == null || card.Pile == null)
            {
                return;
            }

            if (card != __instance)
            {
                return;
            }

            StitchCardModifier? cardStitch = card.GetModifier<StitchCardModifier>();
            if (cardStitch != null)
            {
                if (cardStitch.StitchedCard == null || cardStitch.StitchedCard.Pile == null || !cardStitch.StitchedCard.IsInCombat)
                {
                    if (!TailorStitchQueueSingleton.BeingPlayed.Contains(card))
                    {
                        await StitchCmd.UnstitchCard(card);
                    }
                }
                else if (card.Pile.Type == PileType.Exhaust || cardStitch.StitchedCard.Pile.Type == PileType.Exhaust || card.Pile.Type == PileType.None || cardStitch.StitchedCard.Pile.Type == PileType.None)
                {
                    if (!TailorStitchQueueSingleton.BeingPlayed.Contains(cardStitch.StitchedCard))
                    {
                        await StitchCmd.UnstitchCard(cardStitch.StitchedCard);
                    }
                    if (!TailorStitchQueueSingleton.BeingPlayed.Contains(card))
                    {
                        await StitchCmd.UnstitchCard(card);
                    }
                }
            }
        }
    }

    public class StitchOverlayAdd
    {
        private static readonly string _scenePath = "res://TheTailor/scenes/cards/overlays/stitch.tscn";
        public static AddedNode<NCard, StitchOverlay> StitchOverlay = new(_scenePath, (card, display) =>
        {
            Node cardContainer = card.GetChild(0);
            cardContainer.AddChild(display);
            display.Visible = card.Model?.GetModifier<StitchCardModifier>() != null;
        });
    }

    [HarmonyPatch]
    internal static class StitchOverlayPatch
    {
        [HarmonyPatch(typeof(NCard), "ReloadOverlay")]
        internal static void Postfix(NCard __instance)
        {
            if (__instance.Model == null)
            { 
                return;
            }

            foreach (Node node in __instance.GetChild(0).GetChildren())
            {
                if (node is StitchOverlay)
                {
                    StitchOverlay stitchOverlay = node as StitchOverlay;
                    stitchOverlay.Visible = __instance.Model.GetModifier<StitchCardModifier>() != null;
                }
            }
        }
    }

    [HarmonyPatch]
    internal static class RemoveStitchOnCopiesPatch
    {
        [HarmonyPatch(typeof(CardModel), "CreateClone")]
        internal static CardModel Postfix(CardModel __result, CardModel __instance)
        {
            StitchCardModifier? cardStitch = __result.GetModifier<StitchCardModifier>();
            if (cardStitch != null && __instance.IsMutable)
            {
                __result.RemoveKeyword(Keywords.Stitched);
                CardModifier.RemoveModifier(__result, cardStitch);
                NCard.FindOnTable(__result)?.ReloadOverlay();
            }

            return __result;
        }
    }
}