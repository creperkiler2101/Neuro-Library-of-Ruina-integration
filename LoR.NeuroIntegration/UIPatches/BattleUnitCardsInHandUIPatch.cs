using HarmonyLib;
using LoR.NeuroIntegration.Controllers;

namespace LoR.NeuroIntegration.UIPatches;


[HarmonyPatch(typeof(BattleUnitCardsInHandUI))]
[HarmonyPatch("FixedUpdate")]
public static class BattleUnitCardsInHandUIArrowPatch
{
    public static bool Prefix(BattleUnitCardsInHandUI __instance)
    {
        __instance.mouseFollowingTransform.position = CursorPositionController.Instance.CursorPosition;

        return false;
    }
}

[HarmonyPatch(typeof(BattleUnitCardsInHandUI))]
[HarmonyPatch("ApplySelectedCard")]
public static class BattleUnitCardsInHandUIPatch
{
    public static void Postfix(BattleUnitCardsInHandUI __instance, BattleUnitModel target, int targetSlot)
    {
        NeuroIntegration.Instance.OnCardPlayed();
    }
}
