using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoR.NeuroIntegration.BattleState;

public class EgoCardSelectModel
{
    public List<CardModel> EgoCards { get; set; }
    public BattleStateModel BattleState { get; set; }
}
