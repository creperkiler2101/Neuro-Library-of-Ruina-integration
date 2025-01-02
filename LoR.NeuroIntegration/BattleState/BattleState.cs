using LoR.NeuroIntegration.Extensions;
using LOR_DiceSystem;
using System;
using System.Linq;

namespace LoR.NeuroIntegration.BattleState;

public static class BattleState
{
    public static BattleStateModel CollectState()
    {
        var model = new BattleStateModel
        {
            Enemies = [],
            Librarians = []
        };

        var librarians = BattleObjectManager.instance.GetList(Faction.Player);
        foreach (var librarian in librarians)
        {
            model.Librarians.Add(ConvertToModel(librarian));
        }

        var enemies = BattleObjectManager.instance.GetList(Faction.Enemy);
        foreach (var enemy in enemies)
        {
            model.Enemies.Add(ConvertToModel(enemy));
        }

        return model;
    }

    private static BattleUnitStateModel ConvertToModel(BattleUnitModel model)
    {
        var stateModel = new BattleUnitStateModel
        {
            Name = model.GetUniqueName(),
            IsControllable = model.IsControlable(),
            CurrentLight = model.cardSlotDetail.PlayPoint,
            ReservedLight = model.cardSlotDetail.ReservedPlayPoint,
            MaxLight = model.cardSlotDetail.GetMaxPlayPoint(),
            CurrentHP = Convert.ToInt32(model.hp),
            MaxHP = model.MaxHp,
            CurrentSP = model.breakDetail.breakGauge,
            MaxSP = model.breakDetail.GetDefaultBreakGauge(),
            SlashHpResist = model.GetResistHP_Text(BehaviourDetail.Slash),
            SlashSpResist = model.GetResistBP_Text(BehaviourDetail.Slash),
            PierceHpResist = model.GetResistHP_Text(BehaviourDetail.Penetrate),
            PierceSpResist = model.GetResistBP_Text(BehaviourDetail.Penetrate),
            BluntHpResist = model.GetResistHP_Text(BehaviourDetail.Hit),
            BluntSpResist = model.GetResistBP_Text(BehaviourDetail.Hit),
            IsStaggered = model.IsKnockout(),
            IsDead = model.IsDeadReal(),
            Passives = model.passiveDetail.PassiveList.Select(ConvertToModel).ToList(),
            Buffs = model.bufListDetail.GetActivatedBufList().Select(ConvertToModel).ToList(),
            SpeedDices = []
        };

        for (var i = 0; i < model.speedDiceCount; i++)
        {
            stateModel.SpeedDices.Add(ConvertToModel(model, i));
        }

        return stateModel;
    }

    private static SpeedDiceModel ConvertToModel(BattleUnitModel unit, int speedDiceIndex)
    {
        var speedDice = unit.GetSpeedDiceResult(speedDiceIndex);
        var model = new SpeedDiceModel
        {
            Index = speedDiceIndex,
            Speed = unit.GetSpeed(speedDiceIndex),
            IsBroken = speedDice.breaked,
            IsTargetable = true,
            TargetedBy = []
        };

        var playedCard = unit.cardSlotDetail.cardAry[speedDiceIndex];
        if (playedCard != null)
        {
            model.PlayedCard = ConvertToModel(playedCard);
        }

        var enemyUnits = unit.GetEnemyTeam();
        foreach (var enemy in enemyUnits)
        {
            foreach (var enemyPlayedCard in enemy.cardSlotDetail.cardAry)
            {
                if (enemyPlayedCard != null && enemyPlayedCard.target == unit && enemyPlayedCard.targetSlotOrder == speedDiceIndex)
                {
                    model.TargetedBy.Add(TargetedByConvertToModel(playedCard, enemyPlayedCard));
                }
            }
        }

        return model;
    }

    private static CardModel ConvertToModel(BattlePlayingCardDataInUnitModel playedCard)
    {
        return new()
        {
            Cost = playedCard.card.GetCost(),
            Name = playedCard.card.GetName(),
            Type = GetCardType(playedCard.card.GetSpec().Ranged),
            Abilities = playedCard.card.GetAbilityList(),
            Dices = playedCard.card.GetBehaviourList().Select(x => ConvertToModel(x, playedCard.card)).ToList()
        };
    }

    private static TargetedByModel TargetedByConvertToModel(BattlePlayingCardDataInUnitModel allyCard, BattlePlayingCardDataInUnitModel enemyCard)
    {
        var card = ConvertToModel(enemyCard);
        var isClash = allyCard != null && allyCard.target == enemyCard.owner && allyCard.slotOrder == enemyCard.targetSlotOrder;

        return new()
        {
            AttackerName = enemyCard.owner.GetUniqueName(),
            AttackerSpeed = enemyCard.owner.GetSpeed(enemyCard.slotOrder),
            AttackerSpeedDice = enemyCard.slotOrder,
            Card = card,
            IsClash = isClash
        };
    }

    private static CardDiceModel ConvertToModel(DiceBehaviour dice, BattleDiceCardModel card)
    {
        return new()
        {
            Ability = dice.GetAbility(card),
            Min = dice.Min,
            Max = dice.Dice,
            Type = GetDiceType(dice.Detail),
            IsCounter = dice.Type == BehaviourType.Standby,
        };
    }

    private static PassiveModel ConvertToModel(PassiveAbilityBase passive)
    {
        return new()
        {
            Name = Singleton<PassiveDescXmlList>.Instance.GetName(passive.id),
            Description = Singleton<PassiveDescXmlList>.Instance.GetDesc(passive.id),
        };
    }

    private static BuffModel ConvertToModel(BattleUnitBuf buf)
    {
        return new()
        {
            Name = buf.bufActivatedName,
            Description = buf.bufActivatedText,
            Stack = buf.stack
        };
    }

    private static string GetDiceType(BehaviourDetail diceType)
    {
        return diceType switch
        {
            BehaviourDetail.Evasion => "evasion",
            BehaviourDetail.Guard => "block",
            BehaviourDetail.Hit => "blunt",
            BehaviourDetail.Penetrate => "pierce",
            BehaviourDetail.Slash => "slash",
            _ => "unknown"
        };
    }

    private static string GetCardType(CardRange cardRange)
    {
        return cardRange switch
        {
            CardRange.Near => "melee",
            CardRange.Far => "ranged",
            CardRange.FarArea => "aoe",
            CardRange.FarAreaEach => "aoe_each",
            CardRange.Special => "special",
            CardRange.Instance => "instance",
            _ => "unknown"
        };
    }
}
