namespace LoR.NeuroIntegration.BattleState;

public class TargetedByModel
{
    public string AttackerName { get; set; }
    public int AttackerSpeedDice { get; set; }
    public int AttackerSpeed { get; set; }
    public bool IsClash { get; set; }
    public CardModel Card { get; set; }
}
