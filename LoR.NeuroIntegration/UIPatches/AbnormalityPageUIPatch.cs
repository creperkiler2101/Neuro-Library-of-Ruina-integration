using HarmonyLib;
using LoR.NeuroIntegration.Utils;

namespace LoR.NeuroIntegration.UIPatches;

[HarmonyPatch(typeof(LevelUpUI))]
[HarmonyPatch("OnSelectShow")]
public static class AbnormalityPageUIOnSelectShowPatch
{
    public static void Postfix(LevelUpUI __instance)
    {
        NeuroIntegration.Instance.OnSelectAbnormalityPage();
    }
}

[HarmonyPatch(typeof(LevelUpUI))]
[HarmonyPatch("OnSelectPassive")]
public static class AbnormalityPageUIOnSelectPassivePatch
{
    public static void Prefix(LevelUpUI __instance, EmotionPassiveCardUI picked)
    {
        NeuroIntegration.Instance.OnAbnormalityPageSelected();
    }

    public static void Postfix(LevelUpUI __instance, EmotionPassiveCardUI picked)
    {
        if (AbnormalityPageSelectionUtils.IsNeedToSelectLibrarian())
        {
            NeuroIntegration.Instance.OnSelectAbnormalityPageTarget();
        }
    }
}

