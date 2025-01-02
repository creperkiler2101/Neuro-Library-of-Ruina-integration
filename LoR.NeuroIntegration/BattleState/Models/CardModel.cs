using System.Collections.Generic;

namespace LoR.NeuroIntegration.BattleState;

public class CardModel
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public List<string> Abilities { get; set; }
    public string Type { get; set; }
    public List<CardDiceModel> Dices { get; set; }
}
