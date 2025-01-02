using HarmonyLib;
using LoR.NeuroIntegration.Controllers;

namespace LoR.NeuroIntegration.UIPatches;


[HarmonyPatch(typeof(BattleUnitCardsInHandUI))]
[HarmonyPatch("FixedUpdate")]
public static class BattleUnitCardsInHandUIPatch
{
    public static bool Prefix(BattleUnitCardsInHandUI __instance)
    {
        __instance.mouseFollowingTransform.position = CursorPositionController.Instance.CursorPosition;

        return false;
    }
}
