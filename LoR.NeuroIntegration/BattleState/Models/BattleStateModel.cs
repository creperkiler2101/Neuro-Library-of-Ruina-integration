using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoR.NeuroIntegration.BattleState;

public class BattleStateModel
{
    public List<BattleUnitStateModel> Librarians { get; set; }
    public List<BattleUnitStateModel> Enemies { get; set; }
}
