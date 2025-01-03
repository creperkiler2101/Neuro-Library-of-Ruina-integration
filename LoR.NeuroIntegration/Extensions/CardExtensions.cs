using LOR_DiceSystem;
using System;
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

    public static bool IsEgoCooldown(this BattleDiceCardModel card)
    {
        if (card.MaxCooltimeValue != 0f)
        {
            return (card.CurrentCooltimeValue / card.MaxCooltimeValue) < 1;
        }

        return false;
    }

    public static string ToReadable(this CardRange cardRange)
    {
        return cardRange switch
        {
            CardRange.Near => "melee",
            CardRange.Far => "ranged",
            CardRange.FarArea => "aoe",
            CardRange.FarAreaEach => "aoe_each",
            CardRange.Special => "special",
            CardRange.Instance => "instance",
            _ => "unknown"
        };
    }

    public static string ToReadable(this BehaviourDetail diceType)
    {
        return diceType switch
        {
            BehaviourDetail.Evasion => "evasion",
            BehaviourDetail.Guard => "block",
            BehaviourDetail.Hit => "blunt",
            BehaviourDetail.Penetrate => "pierce",
            BehaviourDetail.Slash => "slash",
            _ => "unknown"
        };
    }
}
