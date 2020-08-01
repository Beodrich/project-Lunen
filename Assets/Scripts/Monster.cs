﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Monster : MonoBehaviour
{
    [Header("Individual Stuff")]

    public string Nickname;
    public int Level;

    [VectorLabels("Curr", " Last", " Next")]
    public Vector3Int Exp;

    public List<Action> ActionSet = new List<Action>();
    public List<MonsterEffect> StatusEffects = new List<MonsterEffect>();
    public List<int> ActionCooldown = new List<int>();

    [Header("Stats")]

    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Health;
    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Attack;
    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Defense;
    [VectorLabels("Base", " Mod", " Current")]
    public Vector3Int Speed;

    [VectorLabels("Attack", " Defense", " Speed")]
    public Vector3 AfterEffectStats;



    [Header("Wild Monster Stuff")]

    public Director.Team MonsterTeam;

    [HideInInspector]
    public Lunen SourceLunen;
    [HideInInspector]
    public SetupRouter sr;
    [HideInInspector]
    public float CurrCooldown;

    private float ExpAddEvery = 0.1f;
    private float ExpAddCurrent = 0.1f;
    private int FractionOfExp = 10;
    private int FractionOfHealth = 10;
    public int ExpToAdd;
    public int HealthToSubtract;

    public bool CooldownDone;
    private int EndOfTurnDamage;
    [SerializeField] private bool PlayHurtSFX = true;
    [HideInInspector] public float PlayHurtSFXType = 1;
    public bool LunenOut;
    public int LunenOrder;
    public AIScripts.AILevel level;
    [HideInInspector]
    public int MoveAffinityCost;
    public bool actionSuccess;
    public List<float> DamageTakenScalar;
    public UIElementCollection currentuiec;

    private void Start()
    {
        CurrCooldown = 1000f;
        CooldownDone = false;
    }

    private void Update()
    {
        if (sr != null)
        {
            if (sr.canvasCollection.partyPanelOpenForBattle)
            {
                if (Level < sr.database.LevelCap)
                {
                    ExpAddCurrent -= Time.deltaTime;
                    if (ExpAddCurrent < 0)
                    {
                        ExpAddCurrent += ExpAddEvery;
                        if (ExpToAdd > 0)
                        {
                            int maxExpPerTick = (Exp.z - Exp.y) / FractionOfExp;
                            if (maxExpPerTick == 0) maxExpPerTick = 1;
                            if (maxExpPerTick >= (Exp.z - Exp.x)) //If next exp tick goes over or equals next
                            {
                                if (ExpToAdd >= (Exp.z - Exp.x)) //If exp recieved is greater than or equal to next level
                                {
                                    ExpToAdd -= (Exp.z - Exp.x); //Subtract exp from pool
                                    Exp.x += (Exp.z - Exp.x); //Add from pool to exp total
                                    LevelUp();
                                }
                                else
                                {
                                    Exp.x += ExpToAdd; //Finish off exp pool
                                    ExpToAdd = 0; //Set exp pool to zero.
                                }
                            }
                            else
                            {
                                if (ExpToAdd >= maxExpPerTick) //If exp recieved is greater than or equal to max pool per tick
                                {
                                    ExpToAdd -= maxExpPerTick; //Subtract exp from pool
                                    Exp.x += maxExpPerTick; //Add from pool to exp total
                                }
                                else
                                {
                                    Exp.x += ExpToAdd; //Finish off exp pool
                                    ExpToAdd = 0; //Set exp pool to zero.
                                }
                            }
                        }
                    }
                }
            }
            if (sr.director.DirectorDeltaTime != 0 && LunenOut && LunenOrder < sr.director.MaxLunenOut)
            {
                if (CurrCooldown > 0f)
                {
                    if (CurrCooldown > GetMaxCooldown()) CurrCooldown = GetMaxCooldown();
                    CurrCooldown -= sr.director.DirectorDeltaTime;
                    CooldownDone = false;
                }
                else
                {
                    if (!CooldownDone)
                    {
                        //This is the point where the cooldown finishes. There's a lot to program here.
                        CalculateStats();
                        TickUpMoveCooldowns();
                        sr.canvasCollection.ScanParty(MonsterTeam);
                        if (MonsterTeam == Director.Team.EnemyTeam && sr.director.PlayerLunenAlive.Count != 0)
                        {
                            //StartAI
                            if (!(bool)sr.database.GetTriggerValue("BattleVars/LunenAttacking"))
                            {
                                PerformAction(AIScripts.StartDecision(sr, this));
                                CooldownDone = true;
                            }
                            
                        }
                        else if (MonsterTeam != Director.Team.EnemyTeam)
                        {
                            sr.soundManager.PlaySoundEffect("LunenCooldownDone");
                            CooldownDone = true;
                        }
                        if (EndOfTurnDamage > 0)
                        {
                            TakeDamage(EndOfTurnDamage);
                        }
                        
                    }
                    else
                    {
                        if (MonsterTeam != Director.Team.EnemyTeam)
                        {
                            currentuiec.SetCollectionState(UITransition.State.Enable);
                        }
                        CurrCooldown = 0f;
                    }
                }
                
                
                if (HealthToSubtract > 0)
                {
                    int maxHPPerTick = (Health.x + Health.y) / FractionOfHealth;
                    if (maxHPPerTick == 0) maxHPPerTick = 1;
                    if (PlayHurtSFX)
                    {
                        if (HealthToSubtract < Health.z)
                        {
                            if (PlayHurtSFXType > 1) sr.soundManager.PlaySoundEffect("LunenHurtSuper");
                            else if (PlayHurtSFXType < 1) sr.soundManager.PlaySoundEffect("LunenHurtLesser");
                            else sr.soundManager.PlaySoundEffect("LunenHurtNormal");
                        }
                        
                        PlayHurtSFX = false;
                    }
                    if (HealthToSubtract > maxHPPerTick) //If exp recieved is greater than or equal to max pool per tick
                    {
                        HealthToSubtract -= maxHPPerTick; //Subtract exp from pool
                        Health.z -= maxHPPerTick; //Add from pool to exp total
                    }
                    else
                    {
                        Health.z -= HealthToSubtract; //Finish off exp pool
                        HealthToSubtract = 0; //Set exp pool to zero.
                        PlayHurtSFX = true;
                    }
                    if (Health.z <= 0)
                    {
                        if (sr != null) sr.director.LunenHasDied(this);
                        HealthToSubtract = 0;
                        //if (MonsterTeam == Director.Team.EnemyTeam) Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void PerformAction(int index)
    {
        sr.canvasCollection.EnsureValidTarget();
        Action action = ActionSet[index];
        if (sr.director.CanUseMove(this, action, index) == 1)
        {
            ActionCooldown[index] = 0;
            sr.database.SetTriggerValue("BattleVars/LunenAttacking", true);
            action.MonsterUser = this;
            action.Execute();
        }
        else
        {
            EndTurn();
        }
        
    }

    public void PerformAction(AIDecision decision)
    {
        sr.canvasCollection.EnemySelfTarget = decision.targetSelfIndex;
        sr.canvasCollection.EnemyOtherTarget = decision.targetOtherIndex;
        PerformAction(decision.moveIndex);
    }

    public void LevelUp()
    {
        Level++;
        if (SourceLunen.Evolves)
        {
            if (SourceLunen.EvolutionLevel <= Level)
            {
                Evolve();
            }
        }
        CalculateStats();
        CalculateExpTargets();
        GetLevelUpMove(Level);
        sr.canvasCollection.ScanBothParties();
        sr.canvasCollection.UpdatePartyPanelLunen();
        
    }

    public void Evolve()
    {
        SourceLunen = SourceLunen.EvolutionLunen;
        sr.eventLog.AddEvent(Nickname + " evolves into " + SourceLunen.name);
        TemplateToMonster(SourceLunen);
    }

    public void GetLevelUpMove(int index)
    {
        foreach (Lunen.LearnedAction action in SourceLunen.LearnedActions)
        {
            if (Level == action.level)
            {
                ActionSet.Add(action.action);
                ActionCooldown.Add(0);
            }
        }
    }

    public void GetPreviousMoves()
    {
        foreach (Lunen.LearnedAction action in SourceLunen.LearnedActions)
        {
            if (Level >= action.level)
            {
                ActionSet.Add(action.action);
                ActionCooldown.Add(0);
            }
        }
    }

    public void SortMoves(bool highLevelFirst)
    {
        /*
        if (highLevelFirst)
            //ActionSet = ActionSet.OrderByDescending(x=>x.GetComponent<Action>().SourceLunenLearnedLevel).ToList();
        else
            //ActionSet = ActionSet.OrderBy(x=>x.GetComponent<Action>().SourceLunenLearnedLevel).ToList();
            */

        if (highLevelFirst) ActionSet.Reverse();
    }

    public void TemplateToMonster(Lunen template)
    {
        SourceLunen = template;

        Health.x = template.Health.x;
        Attack.x = template.Attack.x;
        Defense.x = template.Defense.x;
        Speed.x = template.Speed.x;

        Health.y = template.Health.y * Level;
        Attack.y = template.Attack.y * Level;
        Defense.y = template.Defense.y * Level;
        Speed.y = template.Speed.y * Level;
        Health.z = GetMaxHealth();
        CalculateStats();
        CalculateExpTargets();
        UpdateMoveCost();
        Exp.x = Exp.y;
        Nickname = template.name;
        SetObjectName();
    }

    public void AssortPointsAI(int points)
    {
        for (int i = 0; i < points; i++)
        {
            int dividend = 4;
            int random = Random.Range(0, dividend);
            switch (random)
            {
                case 0: Health.y += 1; break;
                case 1: Attack.y += 1; break;
                case 2: Defense.y += 1; break;
                case 3: Speed.y += 1; break;
            }

        }
    }

    public void UpdateMoveCost()
    {
        MoveAffinityCost = 0;
        for (int i = 0; i < ActionSet.Count; i++)
        {
            MoveAffinityCost += ActionSet[i].AdditionalAffinityCost;
        }
    }

    public void GetExp(int value)
    {
        ExpToAdd += value;
    }

    public void CalculateExpTargets()
    {
        Exp.y = (Level) * (Level) * (Level);
        Exp.z = (Level + 1) * (Level + 1) * (Level + 1);
    }
    
    public void TickUpMoveCooldowns(int amount = 1)
    {
        for (int i = 0; i < ActionCooldown.Count; i++)
        {
            ActionCooldown[i] += amount;
        }
    }

    public void TakeDamage(int value)
    {
        if (value < 0) Heal(-value);
        else
        {
            HealthToSubtract += value;
        }
        
    }

    public void Heal(int value)
    {
        if (value < 0) TakeDamage(-value);
        else
        {
            Health.z += value;
            if (Health.z > GetMaxHealth())
            {
                Health.z = GetMaxHealth();
            }
        }
    }

    public void CalculateStats()
    {
        bool switchATKandDEF = false;

        Attack.z = Attack.x + Attack.y;
        Defense.z = Defense.x + Defense.y;
        Speed.z = Speed.x + Speed.y;

        AfterEffectStats.x = Attack.z;
        AfterEffectStats.y = Defense.z;
        AfterEffectStats.z = Speed.z;

        EndOfTurnDamage = 0;

        DamageTakenScalar.Clear();

        for (int i = 0; i < 11; i++) DamageTakenScalar.Add(1f);

        for (int i = 0; i < StatusEffects.Count; i++)
        {
             switch (StatusEffects[i].Effect.AttackEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        switch (StatusEffects[i].Effect.GlobalAttack.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                AfterEffectStats.x += (AfterEffectStats.x * (StatusEffects[i].Effect.GlobalAttack.PercentageChange / 100));
                                break;
                            case Effects.StatChange.NumberType.HardNumber:
                                AfterEffectStats.x += StatusEffects[i].Effect.GlobalAttack.HardNumberChange;
                                break;
                        }
                        break;

                }
                switch (StatusEffects[i].Effect.DefenseEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        switch (StatusEffects[i].Effect.GlobalDefense.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                AfterEffectStats.y += (AfterEffectStats.y * (StatusEffects[i].Effect.GlobalDefense.PercentageChange / 100));
                                break;
                            case Effects.StatChange.NumberType.HardNumber:
                                AfterEffectStats.y += StatusEffects[i].Effect.GlobalDefense.HardNumberChange;
                                break;
                        }
                        break;

                }
                switch (StatusEffects[i].Effect.SpeedEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        switch (StatusEffects[i].Effect.GlobalSpeed.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                AfterEffectStats.z += (AfterEffectStats.z * (StatusEffects[i].Effect.GlobalSpeed.PercentageChange / 100));
                                break;
                            case Effects.StatChange.NumberType.HardNumber:
                                AfterEffectStats.z += StatusEffects[i].Effect.GlobalSpeed.HardNumberChange;
                                break;
                        }
                        break;

                }
                switch (StatusEffects[i].Effect.DamageTakenEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        for (int j = 0; j < 11; j++)
                        {
                            DamageTakenScalar[j] += (DamageTakenScalar[j] * (StatusEffects[i].Effect.GlobalDamageTaken.PercentageChange / 100));
                        }
                        break;
                    case Effects.RangeOfEffect.TypeBased: //StatusEffects[i].DamageTakenByType
                        for (int j = 0; j < StatusEffects[i].Effect.TypesAfflicted.VulnerableTypes.Count; j++)
                        {
                            DamageTakenScalar[StatusEffects[i].Effect.TypesAfflicted.VulnerableTypes[j].indexValue] += DamageTakenScalar[StatusEffects[i].Effect.TypesAfflicted.VulnerableTypes[j].indexValue] * (StatusEffects[i].Effect.GlobalDamageTaken.PercentageChange / 100);
                        }
                        break;
                }

                switch (StatusEffects[i].Effect.EndOfTurnDamage)
                {
                    case Effects.HealthRelativity.OfMaxHP:
                        switch (StatusEffects[i].Effect.EndOfTurnDamageType.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                EndOfTurnDamage += Mathf.FloorToInt((Health.x + Health.y) * (StatusEffects[i].Effect.EndOfTurnDamageType.PercentageChange / 100));
                                Debug.Log(EndOfTurnDamage);
                                break;
                        }

                        break;
                    case Effects.HealthRelativity.OfRemainingHP:
                        switch (StatusEffects[i].Effect.EndOfTurnDamageType.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                
                                EndOfTurnDamage += Mathf.FloorToInt((Health.z) * (StatusEffects[i].Effect.EndOfTurnDamageType.PercentageChange / 100));
                                Debug.Log(EndOfTurnDamage);
                                break;
                        }
                        break;
                }
                if (StatusEffects[i].Effect.SwapsAttackAndDefense) switchATKandDEF = true;
            }

        if (switchATKandDEF)
        {
            float temp = AfterEffectStats.x;
            AfterEffectStats.x = AfterEffectStats.y;
            AfterEffectStats.y = temp;
        }
    }

    public void EndTurn()
    {
        ResetCooldown();
        if (MonsterTeam == Director.Team.PlayerTeam) sr.canvasCollection.DescriptionPanels[LunenOrder].GetComponent<UIElementCollection>().SetCollectionState(UITransition.State.Disable);
        for (int i = 0; i < StatusEffects.Count; i++)
        {
            if (StatusEffects[i].Expires)
            {
                StatusEffects[i].ExpiresIn--;
                if (StatusEffects[i].ExpiresIn < 0)
                {
                    if (StatusEffects[i].Effect.InflictsAnotherEffect)
                    {
                        MonsterEffect newBuff = new MonsterEffect();
                        newBuff.Effect = StatusEffects[i].Effect.NextEffect;
                        StatusEffects.Add(newBuff);
                    }
                    StatusEffects.RemoveAt(i);
                    i--;
                }
            }
            
        }
        CalculateStats();
    }

    public float GetMaxCooldown()
    {
        float cooldown = 15f - Mathf.Sqrt(AfterEffectStats.z);
        if (MonsterTeam == Director.Team.EnemyTeam)
        {
            cooldown *= 1.5f;
        }
        if (cooldown < 3) cooldown = 3;
        return cooldown;
    }

    public void ResetCooldown(float offset = 0)
    {
        CurrCooldown = GetMaxCooldown() - offset;
        CooldownDone = false;
    }

    public void ResetMoveCooldown()
    {
        ActionCooldown = new List<int>();
        foreach (Action a in ActionSet) ActionCooldown.Add(0);
    }

    public void SetObjectName()
    {
        transform.name = SourceLunen.name + "_" + Nickname + "_Monster";
    }

    public int GetMaxHealth()
    {
        return Health.x + Health.y;
    }

    public void ActionSwap(int first, int second)
    {
        Action lunen1 = ActionSet[first];
        ActionSet[first] = ActionSet[second];
        ActionSet[second] = lunen1;
    }

    public bool HasRetaliatoryEffects()
    {
        foreach (MonsterEffect effect in StatusEffects)
        {
            if (effect.Effect.onTakingDamageDo != Effects.OnTakingDamageDo.NoEffect)
            {
                return true;
            }
        }
        return false;
    }

    public List<Effects> GetRetaliatoryEffects()
    {
        List<Effects> effectsList = new List<Effects>();
        foreach (MonsterEffect effect in StatusEffects)
        {
            if (effect.Effect.onTakingDamageDo != Effects.OnTakingDamageDo.NoEffect)
            {
                effectsList.Add(effect.Effect);
            }
        }
        return effectsList;
    }

    public int HasEffect(Effects effect)
    {
        for (int i = 0; i < StatusEffects.Count; i++)
        {
            if (StatusEffects[i].Effect == effect) return i;
        }
        return -1;
    }

    public void AddEffect(Effects effect, int duration)
    {
        int foundEffect = HasEffect(effect);
        if (foundEffect == -1)
        {
            MonsterEffect newEffect = new MonsterEffect();
            newEffect.Effect = effect;
            newEffect.ExpiresIn = duration;
            StatusEffects.Add(newEffect);
            CalculateStats();
        }
        else
        {
            StatusEffects[foundEffect].ExpiresIn = duration;
        }
    }

    public void RemoveEffects(bool positive = true, bool negative = true, bool statusEffect = true)
    {
        bool applyEffect = false;
        for (int i = 0; i < StatusEffects.Count; i++)
        {
            applyEffect = false;
            if (StatusEffects[i].Effect.IsAStatusEffect && statusEffect) applyEffect = true;
            else if (!StatusEffects[i].Effect.IsPositiveBuff && negative) applyEffect = true;
            else if (StatusEffects[i].Effect.IsPositiveBuff && positive) applyEffect = true;
            if (applyEffect)
            {
                StatusEffects.RemoveAt(i);
                i--;
            }
        }
    }
}
