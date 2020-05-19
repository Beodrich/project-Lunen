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
    public GameObject Animation;

    [Separator("Effects")]

    public IntendedEffect Effect;
    public MonsterAim Target;
    [ConditionalField(nameof(Effect), false, IntendedEffect.DealDamage)] public int Power;
    

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
            case IntendedEffect.ApplyBuff:
                break;
        }
        MonsterUser.CurrCooldown = MonsterUser.SourceLunen.CooldownTime;
        MonsterUser.loopback.Player1MenuClick(MonsterUser.loopback.MenuOpen);
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
}
