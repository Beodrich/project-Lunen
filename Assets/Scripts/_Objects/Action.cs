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
        Heal,
        ReduceHP,
        RemoveEffects
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
        [ConditionalField(nameof(Effect), false, IntendedEffect.Heal)] public Effects.StatChange HealVars;
        [ConditionalField(nameof(Effect), false, IntendedEffect.ReduceHP)] public Effects.StatChange DamageVars;
        [ConditionalField(nameof(Effect), false, IntendedEffect.RemoveEffects)] public bool RemoveBuffs;
        [ConditionalField(nameof(Effect), false, IntendedEffect.RemoveEffects)] public bool RemoveDebuffs;
        [ConditionalField(nameof(Effect), false, IntendedEffect.RemoveEffects)] public bool RemoveStatusEffects;
    }

    [Separator("Basic Action Info")]

    public string Name;
    public Type Type;
    public int Turns;
    public int AdditionalAffinityCost;
    public float TimePausePeriod;
    public GameObject Animation;
    public bool ComboMove;
    [ConditionalField(nameof(ComboMove))] public Type ComboType;

    [TextArea(3,10)]
    public string MoveDescription;

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
                case MonsterAim.AllAllies:
                    for (int i = 0; i < loopback.director.GetLunenCountOut(actionTeam); i++)
                    {
                        ExecutePerMonster(part, loopback.director.GetMonsterOut(actionTeam, i));
                    }
                    break;
                case MonsterAim.Self:
                    ExecutePerMonster(part, MonsterUser);
                    break;
            }
        }
        loopback.database.SetTriggerValue("BattleVars/AnimationTime", TimePausePeriod);
        loopback.database.SetTriggerValue("BattleVars/AttackLunenName", MonsterUser.Nickname);
        loopback.database.SetTriggerValue("BattleVars/AttackLunenAction", name);
        loopback.battleSetup.StartCutscene(loopback.database.GetPackedCutscene("LunenAttack"));
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
                Heal(part);
                break;
            case IntendedEffect.ReduceHP:
                ReduceHP(part);
                break;
            case IntendedEffect.RemoveEffects:
                MonsterTarget.RemoveEffects(part.RemoveBuffs, part.RemoveDebuffs, part.RemoveStatusEffects);
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

        if (MonsterTarget.CooldownDone)
        {
            MonsterTarget.TickUpMoveCooldowns(-1);
        }

        if (MonsterTarget.HasRetaliatoryEffects())
        {
            List<Effects> listOfRetaliatoryEffects = MonsterTarget.GetRetaliatoryEffects();
            foreach (Effects effect in listOfRetaliatoryEffects)
            {
                switch (effect.onTakingDamageDo)
                {
                    case Effects.OnTakingDamageDo.InflictEffect:
                        MonsterUser.AddEffect(effect.OnTakeDamageEffect, effect.OnTakeDamageEffectDuration);
                    break;
                    case Effects.OnTakingDamageDo.ReduceHealth:
                        int damageValue = 0;
                        if (effect.TakeDamageHealthReduction.StatChangeType == Effects.StatChange.NumberType.Percentage)
                        {
                            damageValue = (int)(MonsterUser.Health.z * (effect.TakeDamageHealthReduction.PercentageChange / 100));
                        }
                        else
                        {
                            damageValue = (effect.TakeDamageHealthReduction.HardNumberChange);
                        }
                        MonsterUser.TakeDamage(damageValue);
                    break;
                }
            }
        }

        MonsterTarget.CurrCooldown += (2f);
        MonsterTarget.CooldownDone = false;
        if (MonsterTarget.MonsterTeam == Director.Team.PlayerTeam) MonsterTarget.currentuiec.SetCollectionState(UITransition.State.Disable);
        //Debug.Log(Damage);
    }

    public void Heal(ActionPart part)
    {
        int healValue = 0;
        if (part.HealVars.StatChangeType == Effects.StatChange.NumberType.Percentage)
        {
            healValue = (int)(MonsterTarget.Health.z * (part.HealVars.PercentageChange / 100));
        }
        else
        {
            healValue = (part.HealVars.HardNumberChange);
        }
        MonsterTarget.Heal(healValue);
    }

    public void ReduceHP(ActionPart part)
    {
        int damageValue = 0;
        if (part.DamageVars.StatChangeType == Effects.StatChange.NumberType.Percentage)
        {
            damageValue = (int)(MonsterTarget.Health.z * (part.DamageVars.PercentageChange / 100));
        }
        else
        {
            damageValue = (part.DamageVars.HardNumberChange);
        }
        MonsterTarget.TakeDamage(damageValue);
    }

    public void ApplyStatusEffect(ActionPart part)
    {
        MonsterTarget.AddEffect(part.StatusEffect, part.StatusEffectTurns);
    }

    public void SetTypeToUserMonster()
    {
        Type = MonsterUser.SourceLunen.Elements[0];
    }

    public Type GetMoveType(Monster userMonster = null)
    {
        switch (name)
        {
            default:
                return Type;
            case "Tackle":
                if (userMonster != null)
                {
                    return userMonster.SourceLunen.Elements[0];
                }
                else
                {
                    return null;
                }
        }
    }

    public static string GetNameOfMonsterAim(ActionPart part)
    {
        switch (part.Target)
        {
            default:
                return "Error";
            case Action.MonsterAim.AllMonsters:
                return "All Monsters";
            case Action.MonsterAim.AllOpponents:
                return "All Opponents";
            case Action.MonsterAim.AllAllies:
                return "All Allies";
            case Action.MonsterAim.Self:
                return "Self";
            case Action.MonsterAim.SingleOpponent:
                return "Single Opponent";
            case Action.MonsterAim.SingleAlly:
                return "Single Ally";
        }
    }
}
