using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

[CreateAssetMenu(fileName = "New Action", menuName = "GameElements/Action")]
public class Action : ScriptableObject
{
    [HideInInspector] public SetupRouter sr;

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

    public bool IsAnAttack()
    {
        foreach(ActionPart part in PartsOfAction)
        {
            if (part.Effect == IntendedEffect.DealDamage) return true;
        }
        return false;
    }

    public void Execute()
    {
        sr = MonsterUser.sr;
        sr.soundManager.PlaySoundEffect("ConfirmSelection");
        Director.Team actionTeam = MonsterUser.MonsterTeam;
        Director.Team targetTeam = Director.Team.PlayerTeam;
        if (actionTeam == Director.Team.PlayerTeam) targetTeam = Director.Team.EnemyTeam;
        sr.eventLog.AddEvent("Attack: " + MonsterUser.Nickname + " attacks with " + Name + " targetting one enemy");
        if (customEvent != null)
        {
            customEvent.Invoke();
        }
        foreach (ActionPart part in PartsOfAction)
        {
            switch (part.Target)
            {
                case MonsterAim.SingleOpponent:
                    ExecutePerMonster(part, sr.canvasCollection.GetTargetMonster(actionTeam, targetTeam));
                    break;
                case MonsterAim.AllOpponents:
                    for (int i = 0; i < sr.director.GetLunenCountOut(targetTeam); i++)
                    {
                        ExecutePerMonster(part, sr.director.GetMonsterOut(targetTeam, i));
                    }
                    break;
                case MonsterAim.AllAllies:
                    for (int i = 0; i < sr.director.GetLunenCountOut(actionTeam); i++)
                    {
                        ExecutePerMonster(part, sr.director.GetMonsterOut(actionTeam, i));
                    }
                    break;
                case MonsterAim.SingleAlly:
                    ExecutePerMonster(part, sr.canvasCollection.GetTargetMonster(actionTeam, actionTeam));
                break;
                case MonsterAim.Self:
                    ExecutePerMonster(part, MonsterUser);
                    break;
            }
        }
        sr.database.SetTriggerValue("BattleVars/AnimationTime", TimePausePeriod);
        sr.database.SetTriggerValue("BattleVars/AttackLunenName", MonsterUser.Nickname);
        sr.database.SetTriggerValue("BattleVars/AttackLunenAction", name);
        sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("LunenAttack"));
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
        float STAB = Type.SameTypeAttackBonus(MonsterUser.SourceLunen.Elements, Type);
        float Modifier = Type.TypeMatch(Type, MonsterTarget.SourceLunen.Elements);
        float Damage = (3 + ((float)MonsterUser.Level / 100) * ((float)part.Power / 2) * (1 + Attack / 100) * (1 - (0.004f * Defense))) * STAB * Modifier;
        MonsterTarget.TakeDamage(Mathf.RoundToInt(Damage));
        MonsterTarget.PlayHurtSFXType = Modifier;

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
