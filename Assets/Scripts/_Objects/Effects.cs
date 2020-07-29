using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New Effect", menuName = "GameElements/Effect")]
public class Effects : ScriptableObject
{
    [System.Serializable]
    public class StatChange
    {
        public enum NumberType
        {
            NoChange,
            HardNumber,
            Percentage
        }

        //[ConditionalField(nameof(AffectStat), false)]
        public NumberType StatChangeType;
        [ConditionalField(nameof(StatChangeType), false, NumberType.HardNumber)] public int HardNumberChange;
        [ConditionalField(nameof(StatChangeType), false, NumberType.Percentage)] public float PercentageChange;
    }

    [System.Serializable]
    public class EnumFlags
    {
        public List<Type> VulnerableTypes;
    }

    public enum RangeOfEffect
    {
        NoEffect,
        Global,
        TypeBased
    }

    public enum HealthRelativity
    {
        NoEffect,
        OfMaxHP,
        OfRemainingHP
    }

    public enum OnTakingDamageDo
    {
        NoEffect,
        InflictEffect,
        ReduceHealth
    }

    public int ExpiresIn;
    public bool IsPositiveBuff;
    public RangeOfEffect AttackEffect;
    [ConditionalField(nameof(AttackEffect), false, RangeOfEffect.Global)] public StatChange GlobalAttack;
    public RangeOfEffect DefenseEffect;
    [ConditionalField(nameof(DefenseEffect), false, RangeOfEffect.Global)] public StatChange GlobalDefense;
    public RangeOfEffect SpeedEffect;
    [ConditionalField(nameof(SpeedEffect), false, RangeOfEffect.Global)] public StatChange GlobalSpeed;
    public RangeOfEffect DamageTakenEffect;
    [ConditionalField(nameof(DamageTakenEffect), false, RangeOfEffect.Global)] public StatChange GlobalDamageTaken;
    [ConditionalField(nameof(DamageTakenEffect), false, RangeOfEffect.TypeBased)] public StatChange DamageTakenByType;
    [ConditionalField(nameof(DamageTakenEffect), false, RangeOfEffect.TypeBased)] public EnumFlags TypesAfflicted;
    public HealthRelativity EndOfTurnDamage;
    [ConditionalField(nameof(EndOfTurnDamage), false, HealthRelativity.OfMaxHP, HealthRelativity.OfRemainingHP)] public StatChange EndOfTurnDamageType;
    
    public OnTakingDamageDo onTakingDamageDo;
    [ConditionalField(nameof(onTakingDamageDo), false, OnTakingDamageDo.ReduceHealth)] public StatChange TakeDamageHealthReduction;
    [ConditionalField(nameof(onTakingDamageDo), false, OnTakingDamageDo.InflictEffect)] public Effects OnTakeDamageEffect;
    [ConditionalField(nameof(onTakingDamageDo), false, OnTakingDamageDo.InflictEffect)] public int OnTakeDamageEffectDuration;
    public bool ShowMiscChanges;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotHitMultipleTargets;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotUseAttackingActions;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotGetStatusEffects;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotGetBuffs;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotGetAttackBuffs;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotGetDefenseBuffs;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotGetSpeedBuffs;
    [ConditionalField(nameof(ShowMiscChanges))] public bool CannotEscape;
    [ConditionalField(nameof(ShowMiscChanges))] public bool SwapsAttackAndDefense;
    [ConditionalField(nameof(ShowMiscChanges))] public bool UsesRandomMoves;
    [ConditionalField(nameof(ShowMiscChanges))] public bool IsAStatusEffect;
    [ConditionalField(nameof(ShowMiscChanges))] public bool InflictsAnotherEffect;
    [ConditionalField(nameof(InflictsAnotherEffect))] public Effects NextEffect;

    public string GetDescription(int turns = -1)
    {
        string returnValue = "";
        if (IsAStatusEffect)
        {
            returnValue = "Inflicts " + name + " For " + turns + " Turns"; 
        }
        else
        {
            int effects = 0;
            if (AttackEffect != RangeOfEffect.NoEffect)
            {
                returnValue += "ATKMOD: ";
                if (GlobalAttack.StatChangeType == StatChange.NumberType.HardNumber)
                {
                    if (GlobalAttack.HardNumberChange > 0) returnValue += "+";
                    returnValue += GlobalAttack.HardNumberChange;
                }
                else if (GlobalAttack.StatChangeType == StatChange.NumberType.Percentage)
                {
                    if (GlobalAttack.PercentageChange > 0) returnValue += "+";
                    returnValue += GlobalAttack.PercentageChange + "%";
                }
                effects++;
            }
            if (DefenseEffect != RangeOfEffect.NoEffect)
            {
                if (effects>0) returnValue += " | ";
                returnValue += "DEFMOD: ";
                if (GlobalDefense.StatChangeType == StatChange.NumberType.HardNumber)
                {
                    if (GlobalDefense.HardNumberChange > 0) returnValue += "+";
                    returnValue += GlobalDefense.HardNumberChange;
                }
                else if (GlobalDefense.StatChangeType == StatChange.NumberType.Percentage)
                {
                    if (GlobalDefense.PercentageChange > 0) returnValue += "+";
                    returnValue += GlobalDefense.PercentageChange + "%";
                }
                effects++;
            }
            if (SpeedEffect != RangeOfEffect.NoEffect)
            {
                if (effects>0) returnValue += " | ";
                returnValue += "SPDMOD: ";
                if (GlobalSpeed.StatChangeType == StatChange.NumberType.HardNumber)
                {
                    if (GlobalSpeed.HardNumberChange > 0) returnValue += "+";
                    returnValue += GlobalSpeed.HardNumberChange;
                }
                else if (GlobalSpeed.StatChangeType == StatChange.NumberType.Percentage)
                {
                    if (GlobalSpeed.PercentageChange > 0) returnValue += "+";
                    returnValue += GlobalSpeed.PercentageChange + "%";
                }
                effects++;
            }
            if (DamageTakenEffect != RangeOfEffect.NoEffect)
            {
                
            }

            returnValue += " For " + turns + " Turns"; 
        }
        return returnValue;
    }
}

[System.Serializable]
public class MonsterEffect
{
    public Effects Effect;
    public bool Expires;
    public int ExpiresIn;
}
