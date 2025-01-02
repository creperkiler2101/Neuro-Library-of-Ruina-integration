using Cysharp.Threading.Tasks;
using LoR.NeuroIntegration.Controllers;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using System;

namespace LoR.NeuroIntegration.NeuroActions;

public class RemoveCardAction : NeuroAction
{
    public override string Name => $"remove_card_{_librarian.UnitData.unitData.name}_{_speedDice}";
    protected override string Description => $"Remove card played by {_librarian.UnitData.unitData.name} (Speed dice: {_speedDice})";
    protected override JsonSchema Schema { get; }

    private readonly BattleUnitModel _librarian;
    private readonly int _speedDice;
    private readonly Action _callback;

    public RemoveCardAction(
        BattleUnitModel librarian,
        int speedDice,
        Action callback, 
        ActionWindow actionWindow) : base(actionWindow)
    {
        _librarian = librarian;
        _speedDice = speedDice;
        _callback = callback;
    }

    protected override UniTask ExecuteAsync()
    {
        SpeedDiceUIController.Instance.RemoveCard(_librarian, _speedDice, _callback);

        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData)
    {
        return ExecutionResult.Success();
    }
}
