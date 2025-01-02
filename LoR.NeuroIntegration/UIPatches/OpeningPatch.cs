using HarmonyLib;
using Opening;

namespace LoR.NeuroIntegration;

[HarmonyPatch(typeof(GameOpeningController))]
[HarmonyPatch("PlayOpening")]
public static class OpeningPatch
{
    public static void Postfix(GameOpeningController __instance)
    {
        if (NeuroIntegrationPlugin.Instance.SkipOpening)
        {
            __instance.StopOpening();
        }
    }
}
