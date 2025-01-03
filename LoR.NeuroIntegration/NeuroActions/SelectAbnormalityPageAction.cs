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

public class SelectAbnormalityPageAction : NeuroAction<SelectAbnormalityPageData>
{
    public override string Name => "select_abnormality_page";
    protected override string Description => "Select one of the available abnormality pages";
    protected override JsonSchema Schema { get; }

    public SelectAbnormalityPageAction(ActionWindow actionWindow) : base(actionWindow)
    {
        Schema = new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Required = ["page"],
            Properties =
            {
                { "page", QJS.Enum(AbnormalityPageSelectionUtils.GetAvailablePages().Select(x => x.GetXmlInfo().cardName)) },
            }
        };
    }

    protected override UniTask ExecuteAsync(SelectAbnormalityPageData parsedData)
    {
        AbnormalityPageSelectionUIController.Instance.SelectAbnormalityPage(parsedData.Page);
        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData, out SelectAbnormalityPageData parsedData)
    {
        var pageName = actionData.Data?["page"]?.Value<string>();
        var page = AbnormalityPageSelectionUtils.GetAvailablePages().FirstOrDefault(x => x.GetXmlInfo().cardName == pageName);
        if (page == null)
        {
            parsedData = new() { };
            return ExecutionResult.Failure($"Abnormality page '{pageName}' not found");
        }

        parsedData = new() { Page = page };
        return ExecutionResult.Success();
    }
}

public class SelectAbnormalityPageData
{
    public EmotionCardXmlInfo Page { get; set; }
}