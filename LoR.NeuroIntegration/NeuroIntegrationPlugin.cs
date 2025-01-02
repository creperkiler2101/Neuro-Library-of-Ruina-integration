using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.Mono;
using HarmonyLib;
using LoR.NeuroIntegration.Controllers;
using NeuroSdk;
using UnityEngine;

namespace LoR.NeuroIntegration;

[BepInPlugin("neuro.integration", "Neuro integration", "1.0.0.0")]
public class NeuroIntegrationPlugin : BaseUnityPlugin
{
    private ConfigEntry<bool> _playCardAction;
    private ConfigEntry<bool> _removeCardAction;
    private ConfigEntry<bool> _endTurnAction;
    private ConfigEntry<bool> _skipOpening;
    private ConfigEntry<bool> _autoContinue;

    public bool PlayCardAction => _playCardAction.Value;
    public bool RemoveCardAction => _removeCardAction.Value;
    public bool EndTurnAction => _endTurnAction.Value;
    public bool SkipOpening => _skipOpening.Value;
    public bool AutoContinue => _autoContinue.Value;

    public static NeuroIntegrationPlugin Instance;

    public void Awake()
    {
        Instance = this;

        _playCardAction = Config.Bind("Actions", "PlayCardAction", true, "Allow neuro to play cards");
        _removeCardAction = Config.Bind("Actions", "RemoveCardAction", true, "Allow neuro to remove already played cards");
        _endTurnAction = Config.Bind("Actions", "EndTurnAction", true, "Allow neuro to end turn");
        _skipOpening = Config.Bind("Automation", "SkipOpening", false, "Skip the opening");
        _autoContinue = Config.Bind("Automation", "AutoContinue", false, "Press continue automatically. Enable in case neuro will be able to play game fully automatically");

        var harmony = new Harmony("neuro.integration");
        harmony.PatchAll();

        var gameObject = new GameObject();
        gameObject.AddComponent<NeuroIntegration>();
        gameObject.AddComponent<SpeedDiceUIController>();
        gameObject.AddComponent<CursorPositionController>();
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        NeuroSdkSetup.Initialize("Library of Ruina");
    }
}