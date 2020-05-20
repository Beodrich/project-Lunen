using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Action : MonoBehaviour
{
    [System.Serializable]
    public enum MonsterAim
    {
        SingleOpponent,
        AllOpponents,
        SingleAlly,
        AllAllies,
        AllMonstersExceptSelf,
        AllMonsters,
        Self
    }

    [System.Serializable]
    public enum IntendedEffect
    {
        DealDamage,
        ApplyBuff,
        Heal
    }

    [Separator("Basic Action Info")]

    public string Name;
    public Types.Element Type;
    public int Turns;
    public int AdditionalAffinityCost;
    public GameObject Animation;

    [Separator("Effects")]

    public IntendedEffect Effect;
    public MonsterAim Target;
    [ConditionalField(nameof(Effect), false, IntendedEffect.DealDamage)] public int Power;
    [ConditionalField(nameof(Effect), false, IntendedEffect.ApplyBuff)] public GameObject EffectToInflict;


    [HideInInspector]
    public Monster MonsterUser;
    [HideInInspector]
    public Monster MonsterTarget;

    public void Execute()
    {
        switch (Target)
        {
            case MonsterAim.SingleOpponent:
                ExecutePerMonster(MonsterUser.loopback.Player2Script.LunenOut[MonsterUser.loopback.EnemyTarget]);
                break;
            case MonsterAim.Self:
                ExecutePerMonster(MonsterUser);
                break;
        }
        MonsterUser.EndTurn();
        
    }

    public void ExecutePerMonster(Monster target)
    {
        MonsterTarget = target;
        switch (Effect)
        {
            case IntendedEffect.DealDamage:
                Attack();
                break;
            case IntendedEffect.Heal:
                break;
            case IntendedEffect.ApplyBuff:
                ApplyBuff(EffectToInflict);
                break;
        }
    }

    public void Attack()
    {
        float Attack = MonsterUser.AfterEffectStats.x;
        float Defense = MonsterTarget.AfterEffectStats.y;
        float STAB = Types.SameTypeAttackBonus(MonsterUser.SourceLunen.Elements, Type);
        float Modifier = Types.TypeMatch(Type, MonsterTarget.SourceLunen.Elements);
        float Damage = (3 + ((float)MonsterUser.Level / 100) * ((float)Power / 2) * (1 + Attack / 100) * (1 - (0.004f * Defense))) * STAB * Modifier;
        MonsterTarget.TakeDamage(Mathf.RoundToInt(Damage));
        Debug.Log(Damage);
    }

    public void ApplyBuff(GameObject buff)
    {
        GameObject newBuff = Instantiate(buff);
        newBuff.transform.SetParent(MonsterTarget.transform);
        MonsterTarget.StatusEffectObjects.Add(newBuff);
        MonsterTarget.CalculateStats();
    }
}
