using HarmonyLib;
using System.Collections;
using UI.Title;
using UnityEngine;

namespace LoR.NeuroIntegration;

[HarmonyPatch(typeof(UITitleController))]
[HarmonyPatch("Initialize")]
public static class TitleMenuPatch
{
    public static void Postfix(UITitleController __instance)
    {
        if (NeuroIntegrationPlugin.Instance.AutoContinue)
        {
            __instance.StartCoroutine(Continue(__instance));
        }
    }

    private static IEnumerator Continue(UITitleController controller)
    {
        yield return new WaitForSeconds(1);
        controller.OnSelectButton(TitleActionType.Continue);
        yield return new WaitForSeconds(1.5f);
        AccessTools.Method(controller.GetType(), "Continue").Invoke(controller, []);
    }
}
