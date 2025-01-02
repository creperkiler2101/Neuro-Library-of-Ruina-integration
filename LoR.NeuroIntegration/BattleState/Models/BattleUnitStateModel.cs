using System.Collections.Generic;

namespace LoR.NeuroIntegration.BattleState;

public class BattleUnitStateModel
{
    public string Name { get; set; }
    public bool IsControllable { get; set; }
    public int CurrentLight { get; set; }
    public int ReservedLight { get; set; }
    public int MaxLight { get; set; }
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int CurrentSP { get; set; }
    public int MaxSP { get; set; }
    public bool IsStaggered { get; set; }
    public bool IsDead { get; set; }
    public string SlashHpResist { get; set; }
    public string PierceHpResist { get; set; }
    public string BluntHpResist { get; set; }
    public string SlashSpResist { get; set; }
    public string PierceSpResist { get; set; }
    public string BluntSpResist { get; set; }
    public List<SpeedDiceModel> SpeedDices { get; set; }
    public List<PassiveModel> Passives { get; set; }
    public List<BuffModel> Buffs { get; set; }
}
