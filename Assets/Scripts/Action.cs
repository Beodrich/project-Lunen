﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Heal,
        StatusEffect,
        DealDamageWithStatusEffect
    }
    public string Name;
    public MonsterAim Target;
    public IntendedEffect Effect;
    public Types.Element Type;
    public GameObject Animation;
    public int Power;
    public float TurnsRequired;

    [HideInInspector]
    public Monster MonsterUser;
    [HideInInspector]
    public Monster MonsterTarget;

    public void Execute()
    {
        switch (Effect)
        {
            case IntendedEffect.DealDamage:
                Attack();
                break;
            case IntendedEffect.Heal:
                break;
            case IntendedEffect.StatusEffect:
                break;
            case IntendedEffect.DealDamageWithStatusEffect:
                break;
        }
        MonsterUser.CurrCooldown = MonsterUser.SourceLunen.CooldownTime;
        MonsterUser.loopback.Player1MenuClick(MonsterUser.loopback.MenuOpen);
    }

    public void Attack()
    {
        float Attack = MonsterUser.Attack.z;
        float Defense = MonsterTarget.Defense.z;
        float STAB = Types.SameTypeAttackBonus(MonsterUser.SourceLunen.Elements, Type);
        float Modifier = Types.TypeMatch(Type, MonsterTarget.SourceLunen.Elements);
        float Damage = (3 + ((float)MonsterUser.Level / 100) * ((float)Power / 2) * (1 + (float)Attack / 100) * (1 - (0.004f * (float)Defense))) * STAB * Modifier;
        MonsterTarget.TakeDamage(Mathf.RoundToInt(Damage));
        Debug.Log(Damage);
    }
}
