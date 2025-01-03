using Cysharp.Threading.Tasks;
using LoR.NeuroIntegration.Controllers;
using LoR.NeuroIntegration.Utils;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace LoR.NeuroIntegration.NeuroActions;

public class SelectEgoCardAction : NeuroAction<SelectEgoCardData>
{
    public override string Name => "select_ego_card";
    protected override string Description => "Select one of the available EGO cards";
    protected override JsonSchema Schema { get; }

    public SelectEgoCardAction(ActionWindow actionWindow) : base(actionWindow)
    {
        Schema = new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Required = ["card"],
            Properties =
            {
                { "card", QJS.Enum(AbnormalityPageSelectionUtils.GetAvailableEgoCards().Select(x => x.GetName())) },
            }
        };
    }

    protected override UniTask ExecuteAsync(SelectEgoCardData parsedData)
    {
        AbnormalityPageSelectionUIController.Instance.SelectEgoCard(parsedData.Card);
        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData, out SelectEgoCardData parsedData)
    {
        var cardName = actionData.Data?["card"]?.Value<string>();
        var card = AbnormalityPageSelectionUtils.GetAvailableEgoCards().FirstOrDefault(x => x.GetName() == cardName);
        if (card == null)
        {
            parsedData = new() { };
            return ExecutionResult.Failure($"EGO card '{cardName}' not found");
        }

        parsedData = new() { Card = card };
        return ExecutionResult.Success();
    }
}

public class SelectEgoCardData
{
    public BattleDiceCardModel Card { get; set; }
}