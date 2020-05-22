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
        Self,
        SingleOpponentAndSelf
    }

    [System.Serializable]
    public enum IntendedEffect
    {
        DealDamage,
        ApplyStatusEffect,
        ApplyBuff,
        Heal
    }

    [System.Serializable]
    public class ActionPart
    {
        public IntendedEffect Effect;
        public MonsterAim Target;
        [ConditionalField(nameof(Effect), false, IntendedEffect.DealDamage)] public int Power;
        [ConditionalField(nameof(Effect), false, IntendedEffect.ApplyBuff)] public GameObject EffectToInflict;
        [ConditionalField(nameof(Effect), false, IntendedEffect.ApplyStatusEffect)] public LunaDex.StatusEffectEnum StatusEffect;
        [ConditionalField(nameof(Effect), false, IntendedEffect.ApplyStatusEffect)] public int StatusEffectTurns;
    }

    [Separator("Basic Action Info")]

    public string Name;
    public Types.Element Type;
    public int Turns;
    public int AdditionalAffinityCost;
    public float TimePausePeriod;
    public GameObject Animation;

    [Separator("Effects")]
    public List<ActionPart> PartsOfAction;
    
    
    


    [HideInInspector]
    public Monster MonsterUser;
    [HideInInspector]
    public Monster MonsterTarget;

    public void Execute()
    {
        foreach (ActionPart part in PartsOfAction)
        {
            switch (part.Target)
            {
                case MonsterAim.SingleOpponent:
                    ExecutePerMonster(part, MonsterUser.loopback.Player2Script.LunenOut[MonsterUser.loopback.EnemyTarget]);
                    break;
                case MonsterAim.AllOpponents:
                    for (int i = 0; i < MonsterUser.loopback.Player2Script.LunenOut.Count; i++)
                    {
                        ExecutePerMonster(part, MonsterUser.loopback.Player2Script.LunenOut[i]);
                    }
                    break;
                case MonsterAim.Self:
                    ExecutePerMonster(part, MonsterUser);
                    break;
            }
        }
        MonsterUser.loopback.DirectorTimeToWait = TimePausePeriod;
        MonsterUser.loopback.DirectorTimeFlowing = false;
        MonsterUser.EndTurn();
        
    }

    public void ExecutePerMonster(ActionPart part, Monster target)
    {
        MonsterTarget = target;
        switch (part.Effect)
        {
            case IntendedEffect.DealDamage:
                Attack(part);
                break;
            case IntendedEffect.ApplyBuff:
                ApplyBuff(part);
                break;
            case IntendedEffect.ApplyStatusEffect:
                ApplyStatusEffect(part);
                break;
            case IntendedEffect.Heal:
                break;
            
        }
    }

    public void Attack(ActionPart part)
    {
        float Attack = MonsterUser.AfterEffectStats.x;
        float Defense = MonsterTarget.AfterEffectStats.y;
        float STAB = Types.SameTypeAttackBonus(MonsterUser.SourceLunen.Elements, Type);
        float Modifier = Types.TypeMatch(Type, MonsterTarget.SourceLunen.Elements);
        float Damage = (3 + ((float)MonsterUser.Level / 100) * ((float)part.Power / 2) * (1 + Attack / 100) * (1 - (0.004f * Defense))) * STAB * Modifier;
        MonsterTarget.TakeDamage(Mathf.RoundToInt(Damage));
        Debug.Log(Damage);
    }

    public void ApplyBuff(ActionPart part)
    {
        GameObject newBuff = Instantiate(part.EffectToInflict);
        newBuff.transform.SetParent(MonsterTarget.transform);
        MonsterTarget.StatusEffectObjects.Add(newBuff);
        MonsterTarget.CalculateStats();
    }

    public void ApplyStatusEffect(ActionPart part)
    {
        GameObject newBuff = Instantiate(MonsterUser.loopback.battleSetup.referenceDex.GetStatusEffectObject(part.StatusEffect));
        newBuff.GetComponent<Effects>().ExpiresIn = part.StatusEffectTurns;
        newBuff.transform.SetParent(MonsterTarget.transform);
        MonsterTarget.StatusEffectObjects.Add(newBuff);
        MonsterTarget.CalculateStats();
    }
}
