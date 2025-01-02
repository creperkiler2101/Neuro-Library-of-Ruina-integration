using BepInEx.Logging;
using HarmonyLib;
using LoR.NeuroIntegration.Utils;
using System.Collections;

namespace LoR.NeuroIntegration;

[HarmonyPatch(typeof(StageController))]
[HarmonyPatch("ApplyLibrarianCardPhase")]
public class ApplyLibrarianCardPhasePatch
{
    public static bool IsNeuroTurn { get; private set; }
    private static ManualLogSource _logger = Logger.CreateLogSource("neuro");

    public static void NeuroTurnEnded()
    {
        IsNeuroTurn = false;
        _logger.LogInfo("Neuro turn end");
    }

    private static void Postfix(StageController __instance, float deltaTime)
    {
        if (IsNeuroTurn)
        {
            return;
        }

        IsNeuroTurn = true;

        NeuroIntegration.Instance.StartCoroutine(NeuroTurnCoroutine());
    }

    private static IEnumerator NeuroTurnCoroutine()
    {
        yield return new UnityEngine.WaitForSeconds(1);
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
