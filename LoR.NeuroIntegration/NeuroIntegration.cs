using LoR.NeuroIntegration.Extensions;
using LoR.NeuroIntegration.NeuroActions;
using NeuroSdk.Actions;
using NeuroSdk.Messages.Outgoing;
using Newtonsoft.Json;
using UnityEngine;

namespace LoR.NeuroIntegration;

public class NeuroIntegration : MonoBehaviour
{
    public static NeuroIntegration Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OnTurnStart()
    {
        StartNeuroTurnActionWindow();
    }

    public void OnTurnEnd()
    {
        Context.Send("You ended your turn, its now battle stage");
    }

    private void StartNeuroTurnActionWindow()
    {
        var actionWindow = ActionWindow.Create(gameObject);
        if (NeuroIntegrationPlugin.Instance.EndTurnAction)
        {
            actionWindow.AddAction(new EndTurnAction(actionWindow));
        }

        var canPlayCard = false;
        var canRemoveCard = false;
        foreach (var librarian in BattleObjectManager.instance.GetAliveList(Faction.Player))
        {
            for (var i = 0; i < librarian.speedDiceCount; i++)
            {
                var isCardPlayed = librarian.cardSlotDetail.cardAry[i] != null;
                if (!isCardPlayed && librarian.CanPlayAnyCard())
                {
                    if (NeuroIntegrationPlugin.Instance.PlayCardAction)
                    {
                        actionWindow.AddAction(new PlayCardAction(librarian, i, StartNeuroTurnActionWindow, actionWindow));
                        canPlayCard = true;
                    }
                }
                else
                {
                    if (NeuroIntegrationPlugin.Instance.RemoveCardAction)
                    {
                        actionWindow.AddAction(new RemoveCardAction(librarian, i, StartNeuroTurnActionWindow, actionWindow));
                        canRemoveCard = true;
                    }
                }
            }
        }

        var state = BattleState.BattleState.CollectState();
        var context = "Its your turn;";
        if (canPlayCard)
        {
            context += " You can play card;";
        }

        if (canRemoveCard)
        {
            context += " You can remove card;";
        }

        actionWindow.SetForce(
            () => true,
            context,
            JsonConvert.SerializeObject(state),
            true);

        actionWindow.Register();
    }
}
