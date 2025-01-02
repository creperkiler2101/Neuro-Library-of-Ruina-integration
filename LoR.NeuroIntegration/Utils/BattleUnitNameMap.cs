using LoR.NeuroIntegration.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace LoR.NeuroIntegration.Utils;

public static class BattleUnitNameMap
{
    private static Dictionary<string, BattleUnitModel> _map = [];

    public static void Refresh()
    {
        var oldMap = _map;
        _map = [];

        var units = BattleObjectManager.instance.GetList();
        var unitNameMap = new Dictionary<string, List<BattleUnitModel>>();
        foreach (var unit in units)
        {
            if (!unitNameMap.TryGetValue(unit.GetName(), out var list))
            {
                list = new List<BattleUnitModel>();
                unitNameMap.Add(unit.GetName(), list);
            }

            list.Add(unit);
        }

        foreach (var pair in unitNameMap)
        {
            var name = pair.Key;
            var list = pair.Value;

            var nameIndex = 1;
            foreach (var unit in list)
            {
                var existingRecord = oldMap.FirstOrDefault(x => x.Value == unit);
                if (existingRecord.Key == null)
                {
                    _map.Add(unit.GetName() + " " + nameIndex, unit);
                    nameIndex++;
                }
                else
                {
                    _map.Add(existingRecord.Key, unit);
                }
            }
        }
    }

    public static List<string> ListEnemyNames()
    {
        var enemies = BattleObjectManager.instance.GetList(Faction.Enemy);
        return enemies.Select(x => x.GetUniqueName()).ToList();
    }

    public static string GetBattleUnitName(BattleUnitModel model)
    {
        return _map.FirstOrDefault(x => x.Value == model).Key;
    }

    public static BattleUnitModel GetBattleUnitModel(string name)
    {
        _map.TryGetValue(name, out BattleUnitModel model);
        return model;
    }
}
