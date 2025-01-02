using LOR_DiceSystem;
using System.Collections.Generic;

namespace LoR.NeuroIntegration.Extensions;

public static class CardExtensions
{
    public static List<string> GetAbilityList(this BattleDiceCardModel card)
    {
        var text = Singleton<BattleCardDescXmlList>.Instance.GetAbilityDesc(card.GetID());        
        if (string.IsNullOrEmpty(text))
        {
            return Singleton<BattleCardAbilityDescXmlList>.Instance.GetAbilityDesc(card.XmlData);
        }

        var abilityList = new List<string>();
        var defaultAbilityDescString = Singleton<BattleCardAbilityDescXmlList>.Instance.GetDefaultAbilityDescString(card.XmlData);
        
        if (!string.IsNullOrEmpty(defaultAbilityDescString))
        {
            abilityList.Add(defaultAbilityDescString);
        }

        abilityList.Add(text);

        return abilityList;
    }

    public static string GetAbility(this DiceBehaviour dice, BattleDiceCardModel card)
    {
        var text = Singleton<BattleCardAbilityDescXmlList>.Instance.GetAbilityDesc(dice);
        if (string.IsNullOrEmpty(text))
        {
            var diceIndex = card.GetBehaviourList().IndexOf(dice);
            text = Singleton<BattleCardDescXmlList>.Instance.GetBehaviourDesc(card.GetID(), diceIndex);
        }

        return text;
    }
}
