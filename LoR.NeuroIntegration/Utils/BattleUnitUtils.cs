using System;
using System.Collections.Generic;
using System.Linq;

namespace LoR.NeuroIntegration.Utils;

public static class BattleUnitUtils
{
    public static int GetMaxSpeedDiceCount()
    {
        var units = BattleObjectManager.instance.GetList();
        var max = 0;

        foreach (var unit in units)
        {
            max = Math.Max(max, unit.speedDiceCount);
        }

        return max;
    }

    public static BattleUnitModel GetLibrarian(string uniqueName)
    {
        return BattleUnitNameMap.GetBattleUnitModel(uniqueName);
    }

    public static List<BattleUnitModel> GetLibrarians()
    {
        return BattleObjectManager.instance.GetList(Faction.Player);
    }

    public static List<BattleUnitModel> GetAliveLibrarians()
    {
        return BattleObjectManager.instance.GetList(Faction.Player).Where(x => !x.IsExtinction()).ToList();
    }

    public static bool CanUseCard(BattleUnitModel attacker, int attackerSpeedDice, BattleUnitModel target, int targetSpeedDice, BattleDiceCardModel card)
    {
        return BattleUnitModel.IsTargetableUnit(card, attacker, target, targetSpeedDice) && !IsDiceBlocked(target, targetSpeedDice);
    }

    public static bool IsDiceBlocked(BattleUnitModel target, int targetSpeedDice)
    {
        if (!target.IsTargetable(null))
        {
            return true;
        }

        if (target.speedDiceCount - 1 == targetSpeedDice && !target.IsTargetable_theLast())
        {
            return true;
        }

        return false;
    }
}
