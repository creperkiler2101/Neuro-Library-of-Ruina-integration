using System.Collections.Generic;

namespace LoR.NeuroIntegration.BattleState;

public class LibrarianModel : BattleUnitStateModel
{
    public List<CardModel> CardsInHand { get; set; }
}
