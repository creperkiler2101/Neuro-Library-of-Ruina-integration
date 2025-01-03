using LoR.NeuroIntegration.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace LoR.NeuroIntegration.BattleState;

public class CardModel
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public List<string> Abilities { get; set; }
    public string Type { get; set; }
    public bool IsRecharging { get; set; }
    public bool ExhaustOnUse { get; set; }
    public bool TargetSelf { get; set; }
    public bool TargetAlly { get; set; }
    public bool TargetEnemy { get; set; }
    public List<CardDiceModel> Dices { get; set; }

    public static CardModel From(BattleDiceCardModel card)
    {
        return new()
        {
            Cost = card.GetCost(),
            Name = card.GetName(),
            Type = card.GetSpec().Ranged.ToReadable(),
            IsRecharging = card.IsEgoCooldown(),
            ExhaustOnUse = card.IsExhaustOnUse(),
            Abilities = card.GetAbilityList(),
            TargetAlly = card.IsTargetableAllUnit() || card.IsOnlyAllyUnit(),
            TargetSelf = card.IsTargetableAllUnit() || card.IsTargetableSelf(),
            TargetEnemy = card.IsTargetableAllUnit() || (!card.IsTargetableSelf() && !card.IsOnlyAllyUnit()),
            Dices = card.GetBehaviourList().Select(x => CardDiceModel.From(x, card)).ToList()
        };
    }
}
