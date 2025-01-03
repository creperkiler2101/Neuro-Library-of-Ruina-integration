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
            model.Librarians.Add(ConvertToLibrarianModel(librarian));
        }

        var enemies = BattleObjectManager.instance.GetList(Faction.Enemy);
        foreach (var enemy in enemies)
        {
            model.Enemies.Add(ConvertToEnemyModel(enemy));
        }

        return model;
    }

    private static BattleUnitStateModel ConvertToEnemyModel(BattleUnitModel model)
    {
        var stateModel = new BattleUnitStateModel();
        FillUnitData(model, stateModel);

        return stateModel;
    }

    private static BattleUnitStateModel ConvertToLibrarianModel(BattleUnitModel model)
    {
        var stateModel = new LibrarianModel
        {
            Cards = [],
            PersonalEgoCards = [],
            SharedEgoCards = []
        };

        FillUnitData(model, stateModel);

        foreach (var card in model.allyCardDetail.GetHand())
        {
            stateModel.Cards.Add(CardModel.From(card));
        }

        foreach (var card in model.personalEgoDetail.GetHand())
        {
            stateModel.PersonalEgoCards.Add(CardModel.From(card));
        }

        if (!model.IsRedMist())
        {
            foreach (var card in Singleton<SpecialCardListModel>.Instance.GetHand())
            {
                stateModel.SharedEgoCards.Add(CardModel.From(card));
            }
        }

        return stateModel;
    }

    private static void FillUnitData(BattleUnitModel model, BattleUnitStateModel stateModel)
    {
        stateModel.Name = model.GetUniqueName();
        stateModel.IsControllable = model.IsControlable();
        stateModel.CurrentLight = model.cardSlotDetail.PlayPoint;
        stateModel.ReservedLight = model.cardSlotDetail.ReservedPlayPoint;
        stateModel.MaxLight = model.cardSlotDetail.GetMaxPlayPoint();
        stateModel.CurrentHP = Convert.ToInt32(model.hp);
        stateModel.MaxHP = model.MaxHp;
        stateModel.CurrentSP = model.breakDetail.breakGauge;
        stateModel.MaxSP = model.breakDetail.GetDefaultBreakGauge();
        stateModel.SlashHpResist = model.GetResistHP_Text(BehaviourDetail.Slash);
        stateModel.SlashSpResist = model.GetResistBP_Text(BehaviourDetail.Slash);
        stateModel.PierceHpResist = model.GetResistHP_Text(BehaviourDetail.Penetrate);
        stateModel.PierceSpResist = model.GetResistBP_Text(BehaviourDetail.Penetrate);
        stateModel.BluntHpResist = model.GetResistHP_Text(BehaviourDetail.Hit);
        stateModel.BluntSpResist = model.GetResistBP_Text(BehaviourDetail.Hit);
        stateModel.CurrentEmotionLevel = model.emotionDetail.EmotionLevel;
        stateModel.MaxEmotionLevel = model.emotionDetail.MaximumEmotionLevel;
        stateModel.EmotionCoinsToNextLevel = model.emotionDetail.MaximumCoinNumber;
        stateModel.NegativeEmotionCoins = model.emotionDetail.NegativeCoins.Count;
        stateModel.PositiveEmotionCoins = model.emotionDetail.PositiveCoins.Count;
        stateModel.TotalEmotionCoins = model.emotionDetail.AllEmotionCoins.Count;
        stateModel.IsActionable = model.IsActionable();
        stateModel.IsStaggered = model.IsKnockout();
        stateModel.IsDead = model.IsDeadReal();
        stateModel.Passives = model.passiveDetail.PassiveList.Select(ConvertToModel).ToList();
        stateModel.Buffs = model.bufListDetail.GetActivatedBufList().Select(ConvertToModel).ToList();
        stateModel.SpeedDices = [];

        for (var i = 0; i < model.speedDiceCount; i++)
        {
            stateModel.SpeedDices.Add(ConvertToModel(model, i));
        }
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
            model.PlayedCard = CardModel.From(playedCard.card);
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

    private static TargetedByModel TargetedByConvertToModel(BattlePlayingCardDataInUnitModel allyCard, BattlePlayingCardDataInUnitModel enemyCard)
    {
        var card = CardModel.From(enemyCard.card);
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
}
