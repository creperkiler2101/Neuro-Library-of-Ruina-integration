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

public class SelectAbnormalityPageTargetAction : NeuroAction<SelectAbnormalityPageTargetData>
{
    public override string Name => "abnormality_page_select_librarian";
    protected override string Description => "Select librarian to whom abnormality page will be applied";
    protected override JsonSchema Schema { get; }

    public SelectAbnormalityPageTargetAction(ActionWindow actionWindow) : base(actionWindow)
    {
        Schema = new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Required = ["librarian"],
            Properties =
            {
                { "librarian", QJS.Enum(BattleUnitUtils.GetAliveLibrarians().Select(x => x.GetUniqueName())) },
            }
        };
    }

    protected override UniTask ExecuteAsync(SelectAbnormalityPageTargetData parsedData)
    {
        AbnormalityPageSelectionUIController.Instance.SelectAbnormalityPageTarget(parsedData.Librarian);
        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData, out SelectAbnormalityPageTargetData parsedData)
    {
        var name = actionData.Data?["librarian"]?.Value<string>();
        var librarian = BattleUnitUtils.GetLibrarian(name);
        if (librarian == null)
        {
            parsedData = null;
            return ExecutionResult.Failure($"Librarian '{name}' not found");
        }

        parsedData = new() { Librarian = librarian };
        return ExecutionResult.Success();
    }
}

public class SelectAbnormalityPageTargetData
{
    public BattleUnitModel Librarian { get; set; }
}