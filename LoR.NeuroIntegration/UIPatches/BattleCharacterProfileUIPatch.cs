using BattleCharacterProfile;
using HarmonyLib;
using NeuroSdk.Messages.Outgoing;
using System.Collections;
using UnityEngine;

namespace LoR.NeuroIntegration.UIPatches;

[HarmonyPatch(typeof(BattleCharacterProfileUI))]
[HarmonyPatch("DisplayDlg")]
public static class BattleCharacterProfileUIPatch
{
    public static void Postfix(BattleCharacterProfileUI __instance, string str)
    {
        var logger= BepInEx.Logging.Logger.CreateLogSource("Dialog");
        logger.LogInfo($"{__instance.UnitModel.UnitData.unitData.name} says: {str}");
        Context.Send($"{__instance.UnitModel.UnitData.unitData.name} says: {str}");
        __instance.StartCoroutine(HideDialogCoroutine(__instance));
    }

    private static IEnumerator HideDialogCoroutine(BattleCharacterProfileUI instance)
    {
        yield return new WaitForSeconds(3);
        instance.CloseDlgWithDelay();
    }
}
