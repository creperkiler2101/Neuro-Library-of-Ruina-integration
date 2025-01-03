using LoR.NeuroIntegration.Extensions;
using LOR_DiceSystem;

namespace LoR.NeuroIntegration.BattleState;

public class CardDiceModel
{
    public int Min { get; set; }
    public int Max { get; set; }
    public string Type { get; set; }
    public bool IsCounter { get; set; }
    public string Ability { get; set; }

    public static CardDiceModel From(DiceBehaviour dice, BattleDiceCardModel card)
    {
        return new()
        {
            Ability = dice.GetAbility(card),
            Min = dice.Min,
            Max = dice.Dice,
            Type = dice.Detail.ToReadable(),
            IsCounter = dice.Type == BehaviourType.Standby,
        };
    }
}
