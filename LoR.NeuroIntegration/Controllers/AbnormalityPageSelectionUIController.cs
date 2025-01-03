using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LoR.NeuroIntegration.Controllers;

public class AbnormalityPageSelectionUIController : MonoBehaviour
{
    public static AbnormalityPageSelectionUIController Instance;

    private void Awake()
    {
        Instance = this;
    }
    public void SelectAbnormalityPage(EmotionCardXmlInfo page)
    {
        StartCoroutine(SelectAbnormalityPageCoroutine(page));
    }

    public void SelectAbnormalityPageTarget(BattleUnitModel librarian)
    {
        StartCoroutine(SelectAbnormalityPageTargetCoroutine(librarian));
    }

    public void SelectEgoCard(BattleDiceCardModel card)
    {
        StartCoroutine(SelectEgoCardCoroutine(card));
    }

    private IEnumerator SelectAbnormalityPageCoroutine(EmotionCardXmlInfo page)
    {
        var pageUI = BattleManagerUI.Instance.ui_levelup.candidates.First(x => x.Card == page);
        
        pageUI.OnPointerEnter(new PointerEventData(EventSystem.current));
        yield return new WaitForSeconds(1.5f);
        pageUI.OnPointerDown(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
        yield return new WaitForSeconds(0.3f);
        pageUI.OnPointerUp(new PointerEventData(EventSystem.current));
    }

    private IEnumerator SelectAbnormalityPageTargetCoroutine(BattleUnitModel librarian)
    {
        librarian.view.abCardSelector.OnPointerEnter(new PointerEventData(EventSystem.current));
        yield return new WaitForSeconds(1);
        BattleManagerUI.Instance.ui_levelup.OnClickTargetUnit(librarian);
    }

    private IEnumerator SelectEgoCardCoroutine(BattleDiceCardModel card)
    {
        var cardUI = BattleManagerUI.Instance.ui_levelup.egoSlotList.First(x => x.CardModel == card);

        cardUI.OnPointerEnter(new PointerEventData(EventSystem.current));
        yield return new WaitForSeconds(1);
        cardUI.OnPointerDown(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
    }
}
