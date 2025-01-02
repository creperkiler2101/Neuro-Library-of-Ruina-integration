using HarmonyLib;
using LoR.NeuroIntegration.Extensions;
using NeuroSdk.Messages.Outgoing;

namespace LoR.NeuroIntegration.BattleUnitPatches;

[HarmonyPatch(typeof(BattleUnitModel))]
[HarmonyPatch(nameof(BattleUnitModel.Die))]
public static class BattleUnitDeadPatch
{
    public static void Postfix(BattleUnitModel __instance)
    {
        Context.Send($"{__instance.GetUniqueName()} died");
    }
}
