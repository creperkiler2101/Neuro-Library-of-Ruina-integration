using LoR.NeuroIntegration.Utils;
using System.Collections.Generic;

namespace LoR.NeuroIntegration.Extensions;

public static class LibrarianExtensions
{
    public static string GetName(this BattleUnitModel librarian)
    {
        return librarian.UnitData.unitData.name;
    }

    public static string GetUniqueName(this BattleUnitModel librarian)
    {
        return BattleUnitNameMap.GetBattleUnitName(librarian);
    }

    public static int GetRemainingLight(this BattleUnitModel librarian)
    {
        return librarian.cardSlotDetail.PlayPoint - librarian.cardSlotDetail.ReservedPlayPoint;
    }

    public static IEnumerable<BattleDiceCardModel> GetPlayableCards(this BattleUnitModel librarian)
    {
        foreach (var card in librarian.GetFullHand())
        {
            if (librarian.CheckCardAvailableForPlayer(card))
            {
                yield return card;
            }
        }
    }

    public static IEnumerable<BattleDiceCardModel> GetFullHand(this BattleUnitModel librarian)
    {
        foreach (var card in librarian.allyCardDetail.GetHand())
        {
            yield return card;
        }

        foreach (var card in librarian.personalEgoDetail.GetHand())
        {
            yield return card;
        }

        if (!librarian.IsRedMist())
        {
            foreach (var card in Singleton<SpecialCardListModel>.Instance.GetHand())
            {
                yield return card;
            }
        }
    }

    public static bool IsRedMist(this BattleUnitModel librarian)
    {
        return librarian.Book.GetBookClassInfoId() == 250022;
    }

    public static bool CanPlayAnyCard(this BattleUnitModel librarian)
    {
        foreach (var card in librarian.GetFullHand())
        {
            if (librarian.CheckCardAvailableForPlayer(card))
            {
                return true;
            }
        }

        return false;
    }

    public static List<BattleUnitModel> GetEnemyTeam(this BattleUnitModel librarian)
    {
        return BattleObjectManager.instance.GetList(librarian.faction == Faction.Player ? Faction.Enemy : Faction.Player);
    }
}
