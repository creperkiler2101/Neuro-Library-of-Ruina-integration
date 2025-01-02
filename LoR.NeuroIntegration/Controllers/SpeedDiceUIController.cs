using HarmonyLib;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LoR.NeuroIntegration.Controllers;

public class SpeedDiceUIController : MonoBehaviour
{
    public static SpeedDiceUIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayCard(BattleUnitModel librarian, int librarianSpeedDice, BattleUnitModel enemy, int enemySpeedDice, BattleDiceCardModel card, Action callback = null)
    {
        StartCoroutine(PlayCardCoroutine(librarian, librarianSpeedDice, enemy, enemySpeedDice, card, callback));
    }

    public void RemoveCard(BattleUnitModel librarian, int librarianSpeedDice, Action callback = null)
    {
        var speedDiceUI = librarian.view.speedDiceSetterUI.GetSpeedDiceByIndex(librarianSpeedDice);
        AccessTools.Method(speedDiceUI.GetType(), "EmptySpeedDiceSlot").Invoke(speedDiceUI, []);
        callback?.Invoke();
    }

    private IEnumerator PlayCardCoroutine(BattleUnitModel librarian, int librarianSpeedDice, BattleUnitModel enemy, int enemySpeedDice, BattleDiceCardModel card, Action callback = null)
    {
        var speedDiceUI = librarian.view.speedDiceSetterUI.GetSpeedDiceByIndex(librarianSpeedDice);
        AccessTools.Method(speedDiceUI.GetType(), "OnClickSpeedDice").Invoke(speedDiceUI, []);
        
        yield return new WaitForSeconds(1);
        
        var cardUI = BattleManagerUI.Instance.ui_unitCardsInHand
            .GetCardUIList()
            .First(x => x.CardModel == card);

        CursorPositionController.Instance.SetPosition(cardUI.transform.position);
        cardUI.ShowDetail();

        yield return new WaitForSeconds(1);

        cardUI.OnClick();
        var enemySpeedDiceUI = enemy.view.speedDiceSetterUI.GetSpeedDiceByIndex(enemySpeedDice);
        CursorPositionController.Instance.MoveTo(enemySpeedDiceUI.transform.position, 1);

        var logger = BepInEx.Logging.Logger.CreateLogSource("Turn");
        yield return new WaitUntil(() => Vector3.Distance(CursorPositionController.Instance.CursorPosition, enemySpeedDiceUI.transform.position) < 0.01f);
        enemySpeedDiceUI.OnPointerEnter(new BaseEventData(EventSystem.current));
        
        yield return new WaitForSeconds(1);

        AccessTools.Method(enemySpeedDiceUI.GetType(), "OnClickSpeedDice").Invoke(enemySpeedDiceUI, []);
        enemySpeedDiceUI.OnPointerExit(new BaseEventData(EventSystem.current));

        callback?.Invoke();
    }
}
