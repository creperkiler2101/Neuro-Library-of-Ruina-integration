using System.Collections.Generic;

namespace LoR.NeuroIntegration.BattleState;

public class LibrarianModel : BattleUnitStateModel
{
    public List<CardModel> Cards { get; set; }
    public List<CardModel> PersonalEgoCards { get; set; }
    public List<CardModel> SharedEgoCards { get; set; }
}
