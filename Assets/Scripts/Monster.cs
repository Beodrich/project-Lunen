using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [Header("Individual Stuff")]

    public string Nickname;
    public int Level;

    [VectorLabels("Curr", " Last", " Next")]
    public Vector3Int Exp;

    public List<GameObject> ActionSet;
    public List<GameObject> StatusEffectObjects;
    private List<Effects> StatusEffects = new List<Effects>();

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
    public Director loopback;
    [HideInInspector]
    public float CurrCooldown;

    private float ExpAddEvery = 0.1f;
    private float ExpAddCurrent = 0.1f;
    private int FractionOfExp = 30;
    private int FractionOfHealth = 10;
    public int ExpToAdd;
    public int HealthToSubtract;

    private bool CooldownDone;
    private int EndOfTurnDamage;
    [HideInInspector]
    public int MoveAffinityCost;
    [EnumNamedArray(typeof(Types.Element))]
    public List<float> DamageTakenScalar;

    private void Start()
    {
        CurrCooldown = 1f;
        CooldownDone = true;
    }

    private void Update()
    {
        if (loopback != null)
        {
            if (loopback.DirectorDeltaTime != 0)
            {
                if (CurrCooldown > 0f)
                {
                    CurrCooldown -= loopback.DirectorDeltaTime;
                    CooldownDone = false;
                }
                else
                {
                    if (!CooldownDone)
                    {
                        //This is the point where the cooldown finishes. There's a lot to program here.
                        CalculateStats();
                        if (EndOfTurnDamage > 0)
                        {
                            TakeDamage(EndOfTurnDamage);
                        }
                        CooldownDone = true;
                    }
                    else CurrCooldown = 0f;
                }
                ExpAddCurrent -= loopback.DirectorDeltaTime;
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
                    if (HealthToSubtract > 0)
                    {
                        int maxHPPerTick = (Health.x + Health.y) / FractionOfHealth;
                        if (maxHPPerTick == 0) maxHPPerTick = 1;
                        if (HealthToSubtract >= maxHPPerTick) //If exp recieved is greater than or equal to max pool per tick
                        {
                            HealthToSubtract -= maxHPPerTick; //Subtract exp from pool
                            Health.z -= maxHPPerTick; //Add from pool to exp total
                        }
                        else
                        {
                            Health.z -= HealthToSubtract; //Finish off exp pool
                            HealthToSubtract = 0; //Set exp pool to zero.
                        }
                        if (Health.z <= 0)
                        {
                            if (loopback != null) loopback.LunenHasDied(this);
                            HealthToSubtract = 0;
                            //if (MonsterTeam == Director.Team.EnemyTeam) Destroy(gameObject);
                        }
                    }
                }
            }
        }
        
        
    }

    public void LevelUp()
    {
        Level++;
        CalculateStats();
        CalculateExpTargets();
        loopback.ScanBothParties();
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
        Nickname = template.Name;
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
            MoveAffinityCost += ActionSet[i].GetComponent<Action>().AdditionalAffinityCost;
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

    public void TakeDamage(int value)
    {
        HealthToSubtract += value;
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
             switch (StatusEffects[i].AttackEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        switch (StatusEffects[i].GlobalAttack.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                AfterEffectStats.x += (AfterEffectStats.x * (StatusEffects[i].GlobalAttack.PercentageChange / 100));
                                break;
                            case Effects.StatChange.NumberType.HardNumber:
                                AfterEffectStats.x += StatusEffects[i].GlobalAttack.HardNumberChange;
                                break;
                        }
                        break;

                }
                switch (StatusEffects[i].DefenseEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        switch (StatusEffects[i].GlobalDefense.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                AfterEffectStats.y += (AfterEffectStats.y * (StatusEffects[i].GlobalDefense.PercentageChange / 100));
                                break;
                            case Effects.StatChange.NumberType.HardNumber:
                                AfterEffectStats.y += StatusEffects[i].GlobalDefense.HardNumberChange;
                                break;
                        }
                        break;

                }
                switch (StatusEffects[i].SpeedEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        switch (StatusEffects[i].GlobalSpeed.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                AfterEffectStats.z += (AfterEffectStats.z * (StatusEffects[i].GlobalSpeed.PercentageChange / 100));
                                break;
                            case Effects.StatChange.NumberType.HardNumber:
                                AfterEffectStats.z += StatusEffects[i].GlobalSpeed.HardNumberChange;
                                break;
                        }
                        break;

                }
                switch (StatusEffects[i].DamageTakenEffect)
                {
                    case Effects.RangeOfEffect.Global:
                        for (int j = 0; j < 11; j++)
                        {
                            DamageTakenScalar[j] += (DamageTakenScalar[j] * (StatusEffects[i].GlobalDamageTaken.PercentageChange / 100));
                        }
                        break;
                    case Effects.RangeOfEffect.TypeBased: //StatusEffects[i].DamageTakenByType
                        for (int j = 0; j < StatusEffects[i].TypesAfflicted.VulnerableTypes.Count; j++)
                        {
                            DamageTakenScalar[(int)StatusEffects[i].TypesAfflicted.VulnerableTypes[j]] += DamageTakenScalar[(int)StatusEffects[i].TypesAfflicted.VulnerableTypes[j]] * (StatusEffects[i].GlobalDamageTaken.PercentageChange / 100);
                        }
                        break;
                }

                switch (StatusEffects[i].EndOfTurnDamage)
                {
                    case Effects.HealthRelativity.OfMaxHP:
                        switch (StatusEffects[i].EndOfTurnDamageType.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                EndOfTurnDamage += Mathf.FloorToInt((Health.x + Health.y) * (StatusEffects[i].EndOfTurnDamageType.PercentageChange / 100));
                                Debug.Log(EndOfTurnDamage);
                                break;
                        }

                        break;
                    case Effects.HealthRelativity.OfRemainingHP:
                        switch (StatusEffects[i].EndOfTurnDamageType.StatChangeType)
                        {
                            case Effects.StatChange.NumberType.Percentage:
                                
                                EndOfTurnDamage += Mathf.FloorToInt((Health.z) * (StatusEffects[i].EndOfTurnDamageType.PercentageChange / 100));
                                Debug.Log(EndOfTurnDamage);
                                break;
                        }
                        break;
                }
                if (StatusEffects[i].SwapsAttackAndDefense) switchATKandDEF = true;
            }

        if (switchATKandDEF)
        {
            float temp = AfterEffectStats.x;
            AfterEffectStats.x = AfterEffectStats.y;
            AfterEffectStats.y = temp;
        }
    }

    public void GetStatusEffects()
    {
        StatusEffects.Clear();

        for (int i = 0; i < StatusEffectObjects.Count; i++)
        {
            StatusEffects.Add(StatusEffectObjects[i].GetComponent<Effects>());
        }
    }

    public void EndTurn()
    {
        CurrCooldown = SourceLunen.CooldownTime;
        loopback.Player1MenuClick(loopback.MenuOpen);
        GetStatusEffects();
        for (int i = 0; i < StatusEffects.Count; i++)
        {
            StatusEffects[i].ExpiresIn--;
            if (StatusEffects[i].ExpiresIn < 0)
            {
                GameObject temp = StatusEffectObjects[i];
                if (StatusEffects[i].InflictsAnotherEffect)
                {
                    GameObject newBuff = Instantiate(StatusEffects[i].NextEffect);
                    newBuff.transform.SetParent(this.transform);
                    StatusEffectObjects.Add(newBuff);
                }
                StatusEffects.RemoveAt(i);
                StatusEffectObjects.RemoveAt(i);
                Destroy(temp);
                i--;
            }
        }
        CalculateStats();
    }

    public void SetObjectName()
    {
        transform.name = SourceLunen.Name + "_" + Nickname + "_Monster";
    }

    public int GetMaxHealth()
    {
        return Health.x + Health.y;
    }
}
