using HarmonyLib;
using LoR.NeuroIntegration.Utils;

namespace LoR.NeuroIntegration;

[HarmonyPatch(typeof(StageController))]
[HarmonyPatch("ApplyLibrarianCardPhase")]
public class ApplyLibrarianCardPhasePatch
{
    public static bool IsNeuroTurn { get; private set; }

    public static void NeuroTurnEnded()
    {
        IsNeuroTurn = false;
        NeuroIntegration.Instance.OnTurnEnd();
    }

    private static void Postfix(StageController __instance, float deltaTime)
    {
        if (IsNeuroTurn)
        {
            return;
        }

        IsNeuroTurn = true;

        BattleUnitNameMap.Refresh();
        NeuroIntegration.Instance.OnTurnStart();
    }
}

[HarmonyPatch(typeof(StageController))]
[HarmonyPatch("ArrangeCardsPhase")]
public class ArrangeCardsPhasePatch
{
    private static void Prefix(StageController __instance)
    {
        ApplyLibrarianCardPhasePatch.NeuroTurnEnded();
    }
}

[HarmonyPatch(typeof(StageController))]
[HarmonyPatch("RoundStartPhase_System")]
public class RoundStartPhasePatch
{
    private static void Prefix(StageController __instance)
    {
        __instance.CheckInput(true);
    }
}

[HarmonyPatch(typeof(StageController))]
[HarmonyPatch("StartBattle")]
public class StartBattlePatch
{
    private static void Postfix(StageController __instance)
    {
        NeuroSdk.Messages.Outgoing.Context.Send($"Battle againsts {(__instance.stageType == StageType.Invitation ? "guests" : "abnormality")} started");
    }
}

[HarmonyPatch(typeof(StageController))]
[HarmonyPatch("EndBattle")]
public class EndBattlePatch
{
    private static void Prefix(StageController __instance)
    {
        var battleResult = __instance.GetStageModel().GetFloor(__instance.CurrentFloor).IsUnavailable() ? "lost" : "win";

        NeuroSdk.Messages.Outgoing.Context.Send($"Battle againsts {(__instance.stageType == StageType.Invitation ? "guests" : "abnormality")} finished. You {battleResult}");
    }
}