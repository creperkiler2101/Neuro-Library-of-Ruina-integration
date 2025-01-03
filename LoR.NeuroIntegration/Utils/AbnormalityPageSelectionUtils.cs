using HarmonyLib;
using System.Collections.Generic;

namespace LoR.NeuroIntegration.Utils;

public static class AbnormalityPageSelectionUtils
{
    public static bool IsSelectingEgo()
    {
        var levelUpUI = BattleManagerUI.Instance.ui_levelup;
        var isEgo = (bool)AccessTools.Field(levelUpUI.GetType(), "_isEgo").GetValue(levelUpUI);
        return isEgo;
    }

    public static bool IsNeedToSelectLibrarian()
    {
        var levelUpUI = BattleManagerUI.Instance.ui_levelup;
        var needUnitSelection = (bool)AccessTools.Field(levelUpUI.GetType(), "_needUnitSelection").GetValue(levelUpUI);
        return needUnitSelection;
    }

    public static EmotionCardXmlInfo GetSelectedAbnormalityPage()
    {
        var levelUpUI = BattleManagerUI.Instance.ui_levelup;
        var selectedCard = (EmotionCardXmlInfo)AccessTools.Field(levelUpUI.GetType(), "_selectedCard").GetValue(levelUpUI);
        return selectedCard;
    }

    public static IEnumerable<EmotionCardXmlInfo> GetAvailablePages()
    {
        var levelUpUI = BattleManagerUI.Instance.ui_levelup;
        foreach (var page in levelUpUI.candidates)
        {
            yield return page.Card;
        }
    }

    public static IEnumerable<BattleDiceCardModel> GetAvailableEgoCards()
    {
        var levelUpUI = BattleManagerUI.Instance.ui_levelup;
        foreach (var page in levelUpUI.egoSlotList)
        {
            yield return page.CardModel;
        }
    }
}
