using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Effects : MonoBehaviour
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
        public List<Types.Element> VulnerableTypes;
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

    public int ExpiresIn;
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
}
