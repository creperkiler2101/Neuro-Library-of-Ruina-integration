using Cysharp.Threading.Tasks;
using LoR.NeuroIntegration.Controllers;
using LoR.NeuroIntegration.Extensions;
using LoR.NeuroIntegration.Utils;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace LoR.NeuroIntegration.NeuroActions;

public class PlayCardAction : NeuroAction<PlayCardActionData>
{
    public override string Name => $"play_card_{_librarian.UnitData.unitData.name.ToLower()}_{_speedDice}";
    protected override string Description => $"Select a card to play by {_librarian.UnitData.unitData.name} (Speed dice: {_speedDice}) with speed {GetSpeedDiceSpeed()} against enemy and its speed dice";
    protected override JsonSchema Schema { get; } 

    private readonly BattleUnitModel _librarian;
    private readonly int _speedDice;

    public PlayCardAction(
        BattleUnitModel librarian,
        int speedDice,
        ActionWindow actionWindow) : base(actionWindow)
    {
        _librarian = librarian;
        _speedDice = speedDice;

        var librarianLight = _librarian.cardSlotDetail.PlayPoint - _librarian.cardSlotDetail.ReservedPlayPoint;
        Schema = new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Required = ["card", "target", "speed_dice_index"],
            Properties =
            {
                { "card", QJS.Enum(_librarian.GetPlayableCards().Select(x => x.GetName())) },
                { "target", QJS.Enum(BattleUnitNameMap.ListAliveNames()) },
                { "speed_dice_index", new JsonSchema { Type = JsonSchemaType.Integer, Minimum = 0, Maximum = BattleUnitUtils.GetMaxSpeedDiceCount() - 1 } }
            }
        };
    }

    protected override UniTask ExecuteAsync(PlayCardActionData parsedData)
    {
        SpeedDiceUIController.Instance.PlayCard(_librarian, _speedDice, parsedData.Target, parsedData.TargetSpeedDice, parsedData.Card);

        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData, out PlayCardActionData parsedData)
    {
        var cardName = actionData.Data?["card"]?.Value<string>();
        if (string.IsNullOrWhiteSpace(cardName))
        {
            parsedData = null;
            return ExecutionResult.Success();
        }
        
        var card = GetCard(cardName);
        if (card == null)
        {
            parsedData = null;
            return ExecutionResult.Failure($"Card '{cardName}' not found in hand or its not usable");
        }

        var targetName = actionData.Data?["target"]?.Value<string>();
        var target = GetTarget(targetName);
        if (target == null)
        {
            parsedData = null;
            return ExecutionResult.Failure($"Target '{targetName}' not found");
        }

        var speedDiceIndex = actionData.Data?["speed_dice_index"]?.Value<int>();
        if (!speedDiceIndex.HasValue)
        {
            parsedData = null;
            return ExecutionResult.Failure($"Speed value is invalid");
        }

        if (target.speedDiceCount <= speedDiceIndex)
        {
            parsedData = null;
            return ExecutionResult.Failure($"{targetName} has only {target.speedDiceCount} speed dices");
        }

        if (target.IsDeadReal() || !BattleUnitUtils.CanUseCard(_librarian, _speedDice, target, speedDiceIndex.Value, card))
        {
            parsedData = null;
            return ExecutionResult.Failure($"Unable to target '{targetName}' using '{cardName}'");
        }

        parsedData = new()
        {
            Card = card,
            Target = target,
            TargetSpeedDice = speedDiceIndex.Value,
        };

        return ExecutionResult.Success();
    }

    private BattleUnitModel GetTarget(string name)
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
