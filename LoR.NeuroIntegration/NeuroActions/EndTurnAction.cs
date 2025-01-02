using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;

namespace LoR.NeuroIntegration.NeuroActions;

public class EndTurnAction : NeuroAction
{
    public override string Name => "end_turn";
    protected override string Description => "End your turn";
    protected override JsonSchema Schema { get; }

    public EndTurnAction(ActionWindow actionWindow) : base(actionWindow)
    {
    }

    protected override UniTask ExecuteAsync()
    {
        StageController.Instance.CheckInput(true);
        return UniTask.CompletedTask;
    }

    protected override ExecutionResult Validate(ActionJData actionData)
    {
        return ExecutionResult.Success();
    }
}
