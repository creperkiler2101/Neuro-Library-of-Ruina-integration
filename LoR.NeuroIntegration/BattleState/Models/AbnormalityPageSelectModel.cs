using System.Collections.Generic;

namespace LoR.NeuroIntegration.BattleState;

public class AbnormalityPageSelectModel
{
    public List<AbnormalityPageModel> AbnormalityPages { get; set; }
    public BattleStateModel BattleState { get; set; }
}
