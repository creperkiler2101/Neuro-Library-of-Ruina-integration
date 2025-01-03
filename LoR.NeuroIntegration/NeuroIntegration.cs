using Cysharp.Threading.Tasks;
using LoR.NeuroIntegration.BattleState;
using LoR.NeuroIntegration.Extensions;
using LoR.NeuroIntegration.NeuroActions;
using LoR.NeuroIntegration.Utils;
using NeuroSdk.Actions;
using NeuroSdk.Messages.Outgoing;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace LoR.NeuroIntegration;

public class NeuroIntegration : MonoBehaviour
{
    public static NeuroIntegration Instance;

    private ActionWindow _actionWindow;

    private void Awake()
    {
        Instance = this;
    }

    public void OnTurnStart()
    {
        NeuroIntegrationPlugin.Instance.Logger.LogInfo("Turn start");
        
        ClearActionWindow();
        StartNeuroTurnActionWindow(true);
    }

    public void OnTurnEnd()
    {
        NeuroIntegrationPlugin.Instance.Logger.LogInfo("Turn end");
       
        ClearActionWindow();
        Context.Send("You ended your turn, its now battle stage");
    }

    public void OnCardPlayed()
    {
        NeuroIntegrationPlugin.Instance.Logger.LogInfo("Card played");
       
        ClearActionWindow();
        StartNeuroTurnActionWindow(false);
    }

    public void OnSelectAbnormalityPage()
    {
        ClearActionWindow();
        SelectAbnormalityPageActionWindow();
    }

    public void OnSelectAbnormalityPageTarget()
    {
        ClearActionWindow();
        SelectAbnormalityPageTargetActionWindow();
    }

    public void OnAbnormalityPageSelected()
    {
        ClearActionWindow();
    }

    public void OnAbnormalityPageTargetSelected()
    {
        ClearActionWindow();
    }

    private void CreateActionWindow()
    {
        if (_actionWindow != null)
        {
            ClearActionWindow();
        }

        _actionWindow = ActionWindow.Create(gameObject);
    }

    private void ClearActionWindow()
    {
        if (_actionWindow != null) 
        {
            DestroyImmediate(_actionWindow);
            _actionWindow = null;
        }
    }

    private void SelectAbnormalityPageActionWindow()
    {
        CreateActionWindow();

        if (AbnormalityPageSelectionUtils.IsSelectingEgo())
        {
            _actionWindow.AddAction(new SelectEgoCardAction(_actionWindow));

            var model = new EgoCardSelectModel
            {
                BattleState = BattleState.BattleState.CollectState(),
                EgoCards = AbnormalityPageSelectionUtils.GetAvailableEgoCards().Select(CardModel.From).ToList()
            };

            _actionWindow.SetForce(1.5f, "Please select an EGO card", JsonConvert.SerializeObject(model), true);
        }
        else
        {
            _actionWindow.AddAction(new SelectAbnormalityPageAction(_actionWindow));

            var model = new AbnormalityPageSelectModel
            {
                BattleState = BattleState.BattleState.CollectState(),
                AbnormalityPages = AbnormalityPageSelectionUtils.GetAvailablePages().Select(AbnormalityPageModel.From).ToList()
            };

            _actionWindow.SetForce(1.5f, "Please select an abnormality page", JsonConvert.SerializeObject(model), true);
        }

        _actionWindow.Register();
    }

    private void SelectAbnormalityPageTargetActionWindow()
    {
        CreateActionWindow();

        _actionWindow.AddAction(new SelectAbnormalityPageTargetAction(_actionWindow));

        var model = new AbnormalityPageTargetSelectModel
        {
            BattleState = BattleState.BattleState.CollectState(),
            AbnormalityPage = AbnormalityPageModel.From(AbnormalityPageSelectionUtils.GetSelectedAbnormalityPage())
        };

        _actionWindow.SetForce(1.5f, "Please select a librarian for abnormality page", JsonConvert.SerializeObject(model), true);
        _actionWindow.Register();
    }

    private void StartNeuroTurnActionWindow(bool firstTime = false)
    {
        CreateActionWindow();

        var canPlayCard = false;
        var canRemoveCard = false;
        foreach (var librarian in BattleObjectManager.instance.GetAliveList(Faction.Player))
        {
            if (librarian.IsKnockout() || !librarian.IsActionable()) continue;

            for (var i = 0; i < librarian.speedDiceCount; i++)
            {
                var dice = librarian.GetSpeedDiceResult(i);
                if (dice.breaked || !dice.isControlable) continue;

                var isCardPlayed = librarian.cardSlotDetail.cardAry[i] != null;
                if (!isCardPlayed && librarian.CanPlayAnyCard())
                {
                    if (NeuroIntegrationPlugin.Instance.PlayCardAction)
                    {
                        _actionWindow.AddAction(new PlayCardAction(librarian, i, _actionWindow));
                        canPlayCard = true;
                    }
                }
                else
                {
                    if (NeuroIntegrationPlugin.Instance.RemoveCardAction)
                    {
                        _actionWindow.AddAction(new RemoveCardAction(librarian, i, _actionWindow));
                        canRemoveCard = true;
                    }
                }
            }
        }

        if (NeuroIntegrationPlugin.Instance.EndTurnAction)
        {
            _actionWindow.AddAction(new EndTurnAction(_actionWindow));
        }

        var state = BattleState.BattleState.CollectState();
        var context = firstTime ? "Its your turn;" : "Its still your turn;";
        if (canPlayCard)
        {
            context += " You can play card;";
        }

        if (canRemoveCard)
        {
            context += " You can remove card;";
        }

        _actionWindow.SetForce(1.5f, context, JsonConvert.SerializeObject(state), true);
        _actionWindow.Register();
    }
}
