using Cysharp.Threading.Tasks;
using LoR.NeuroIntegration.Controllers;
using LoR.NeuroIntegration.Extensions;
using LoR.NeuroIntegration.Utils;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace LoR.NeuroIntegration.NeuroActions;

public class PlayCardAction : NeuroAction<PlayCardActionData>
{
    public override string Name => $"play_card_{_librarian.UnitData.unitData.name}_{_speedDice}";
    protected override string Description => $"Select a card to play by {_librarian.UnitData.unitData.name} (Speed dice: {_speedDice}) with speed {GetSpeedDiceSpeed()} against enemy and its speed dice";
    protected override JsonSchema Schema { get; } 

    private readonly BattleUnitModel _librarian;
    private readonly int _speedDice;
    private readonly Action _callback;

    public PlayCardAction(
        BattleUnitModel librarian,
        int speedDice,
        Action callback,
        ActionWindow actionWindow) : base(actionWindow)
    {
        _librarian = librarian;
        _speedDice = speedDice;
        _callback = callback;

        var librarianLight = _librarian.cardSlotDetail.PlayPoint - _librarian.cardSlotDetail.ReservedPlayPoint;
        Schema = new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Required = ["card", "enemy", "speed_dice_index"],
            Properties =
            {
                { "card", QJS.Enum(_librarian.GetPlayableCards().Select(x => x.GetName())) },
                { "enemy", QJS.Enum(BattleUnitNameMap.ListEnemyNames()) },
                { "speed_dice_index", new JsonSchema { Type = JsonSchemaType.Integer, Minimum = 0 } }
            }
        };
    }

    protected override UniTask ExecuteAsync(PlayCardActionData parsedData)
    {
        SpeedDiceUIController.Instance.PlayCard(_librarian, _speedDice, parsedData.Target, parsedData.TargetSpeedDice, parsedData.Card, _callback);

        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData, out PlayCardActionData parsedData)
    {
        var cardName = actionData.Data?["card"]?.Value<string>();
        if (string.IsNullOrWhiteSpace(cardName))
        {
            parsedData = new();
            return ExecutionResult.Success();
        }
        
        var card = GetCard(cardName);
        if (card == null)
        {
            parsedData = new();
            return ExecutionResult.Failure($"Card '{cardName}' not found in hand or its not usable");
        }

        var enemyName = actionData.Data?["enemy"]?.Value<string>();
        var enemy = GetEnemy(enemyName);
        if (enemy == null)
        {
            parsedData = new();
            return ExecutionResult.Failure($"Enemy '{enemyName}' not found");
        }

        var speedDiceIndex = actionData.Data?["speed_dice_index"]?.Value<int>();
        if (!speedDiceIndex.HasValue)
        {
            parsedData = new();
            return ExecutionResult.Failure($"Speed value is invalid");
        }

        if (enemy.speedDiceCount <= speedDiceIndex)
        {
            parsedData = new();
            return ExecutionResult.Failure($"{enemyName} has only {enemy.speedDiceCount} speed dices");
        }

        parsedData = new()
        {
            Card = card,
            Target = enemy,
            TargetSpeedDice = speedDiceIndex.Value,
        };

        return ExecutionResult.Success();
    }

    private BattleUnitModel GetEnemy(string name)
    {
        return BattleUnitNameMap.GetBattleUnitModel(name);
    }

    private BattleDiceCardModel GetCard(string name)
    {
        var cards = _librarian.GetPlayableCards();
        return cards.FirstOrDefault(x => x.GetName() == name);
    }

    private int GetSpeedDiceSpeed()
    {
        return _librarian.GetSpeed(_speedDice);
    }
}

public class PlayCardActionData
{
    public BattleDiceCardModel Card { get; set; }
    public BattleUnitModel Target { get; set; }
    public bool Redirect { get; set; }
    public int TargetSpeedDice { get; set; }
}
