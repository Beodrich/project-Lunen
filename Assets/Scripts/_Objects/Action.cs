using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

[CreateAssetMenu(fileName = "New Action", menuName = "GameElements/Action")]
public class Action : ScriptableObject
{
    [HideInInspector] public SetupRouter loopback;

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
        [ConditionalField(nameof(Effect), false, IntendedEffect.ApplyStatusEffect)] public Effects StatusEffect;
        [ConditionalField(nameof(Effect), false, IntendedEffect.ApplyStatusEffect)] public int StatusEffectTurns;
    }

    [Separator("Basic Action Info")]

    public string Name;
    public Type Type;
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
    [HideInInspector]
    public int SourceActionIndex;
    [HideInInspector]
    public int SourceLunenLearnedLevel;

    [Separator("Custom Events")]
    public UnityEvent customEvent;

    public void Execute()
    {
        loopback = MonsterUser.loopback;
        Director.Team actionTeam = MonsterUser.MonsterTeam;
        Director.Team targetTeam = Director.Team.PlayerTeam;
        if (actionTeam == Director.Team.PlayerTeam) targetTeam = Director.Team.EnemyTeam;
        loopback.eventLog.AddEvent("Attack: " + MonsterUser.Nickname + " attacks with " + Name + " targetting one enemy");
        if (customEvent != null)
        {
            customEvent.Invoke();
        }
        foreach (ActionPart part in PartsOfAction)
        {
            switch (part.Target)
            {
                case MonsterAim.SingleOpponent:
                    if (actionTeam == Director.Team.PlayerTeam) ExecutePerMonster(part, loopback.director.GetMonsterOut(targetTeam, loopback.canvasCollection.GetLunenSelected(Director.Team.EnemyTeam)));
                    if (actionTeam == Director.Team.EnemyTeam) ExecutePerMonster(part, loopback.director.GetMonsterOut(targetTeam, loopback.director.EnemyLunenSelect));
                    break;
                case MonsterAim.AllOpponents:
                    for (int i = 0; i < loopback.director.GetLunenCountOut(targetTeam); i++)
                    {
                        ExecutePerMonster(part, loopback.director.GetMonsterOut(targetTeam, i));
                    }
                    break;
                case MonsterAim.Self:
                    ExecutePerMonster(part, MonsterUser);
                    break;
            }
        }
        loopback.director.DirectorTimeToWait = TimePausePeriod;
        loopback.director.DirectorTimeFlowing = false;
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
                ApplyStatusEffect(part);
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
        Debug.Log("Attack: " + Attack + " | Defense: " + Defense);
        float STAB = Type.SameTypeAttackBonus(MonsterUser.SourceLunen.Elements, Type);
        float Modifier = Type.TypeMatch(Type, MonsterTarget.SourceLunen.Elements);
        float Damage = (3 + ((float)MonsterUser.Level / 100) * ((float)part.Power / 2) * (1 + Attack / 100) * (1 - (0.004f * Defense))) * STAB * Modifier;
        MonsterTarget.TakeDamage(Mathf.RoundToInt(Damage));
        //Debug.Log(Damage);
    }

    public void ApplyStatusEffect(ActionPart part)
    {
        MonsterEffect newEffect = new MonsterEffect();
        newEffect.Effect = part.StatusEffect;
        newEffect.ExpiresIn = part.StatusEffectTurns;
        MonsterTarget.StatusEffects.Add(newEffect);
        MonsterTarget.CalculateStats();
    }

    public void SetTypeToUserMonster()
    {
        Type = MonsterUser.SourceLunen.Elements[0];
    }
}
