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
        MonsterUser.CurrCooldown = MonsterUser.LastCooldown = MonsterUser.SourceLunen.CooldownTime;
        MonsterUser.loopback.Player1MenuClick(MonsterUser.loopback.MenuOpen);
    }

    public void Attack()
    {
        float modifier = Types.TypeMatch(Type, MonsterTarget.Elements);
        float damage = ((((2 * MonsterUser.Level / 5) + 2) * Power * (MonsterUser.Attack.z / MonsterTarget.Defense.z) / 50) + 2) * modifier;
        MonsterTarget.Health.z -= Mathf.RoundToInt(damage);
    }
}
