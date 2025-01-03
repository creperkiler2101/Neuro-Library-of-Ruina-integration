using Cysharp.Threading.Tasks;
using LoR.NeuroIntegration.Controllers;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using System;

namespace LoR.NeuroIntegration.NeuroActions;

public class RemoveCardAction : NeuroAction
{
    public override string Name => $"remove_card_{_librarian.UnitData.unitData.name.ToLower()}_{_speedDice}";
    protected override string Description => $"Remove card played by {_librarian.UnitData.unitData.name} (Speed dice: {_speedDice})";
    protected override JsonSchema Schema { get; }

    private readonly BattleUnitModel _librarian;
    private readonly int _speedDice;

    public RemoveCardAction(
        BattleUnitModel librarian,
        int speedDice,
        ActionWindow actionWindow) : base(actionWindow)
    {
        _librarian = librarian;
        _speedDice = speedDice;
    }

    protected override UniTask ExecuteAsync()
    {
        SpeedDiceUIController.Instance.RemoveCard(_librarian, _speedDice);

        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData)
    {
        return ExecutionResult.Success();
    }
}
