using System.Collections.Generic;

namespace LoR.NeuroIntegration.BattleState;

public class SpeedDiceModel
{
    public int Index { get; set; }
    public int Speed { get; set; }
    public bool IsBroken { get; set; }
    public bool IsTargetable { get; set; }
    public CardModel PlayedCard { get; set; }
    public List<TargetedByModel> TargetedBy { get; set; }
}
