using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        public int Base;
        public int Mod;
        public int Current;
    }

    public string Nickname;
    public string Species;
    public int Level;
    public int NextXP;
    public int CurrXP;
    public float CurrCooldown;
    public float LastCooldown = 1f;

    public Stat Health;
    public Stat Attack;
    public Stat Defense;
    public Stat Speed;

    public Types.Element[] Elements;

    public List<GameObject> ActionSet;

    public GameObject DEBUG_TEXT_OUTPUT;

    [HideInInspector]
    public Director loopback;

    private void Start()
    {
        CurrCooldown = LastCooldown = 1f;
        if (DEBUG_TEXT_OUTPUT != null)
        {
            DEBUG_DISPLAY_TEXT();
        }
    }

    private void Update()
    {
        if (Time.unscaledDeltaTime < 0.25f)
        {
            if (CurrCooldown > 0f)
            {
                CurrCooldown -= Time.unscaledDeltaTime;
            }
            else
            {
                CurrCooldown = 0f;
            }
        }
    }

    public void TemplateToMonster(Lunen template)
    {
        Health.Base = template.BaseHealth;
        Attack.Base = template.BaseAttack;
        Defense.Base = template.BaseDefense;
        Speed.Base = template.BaseSpeed;
        Species = template.Name;
        AssortPointsAI(Level * template.PointsPerLevel);
        Health.Current = GetMaxHealth();
        CalculateStats();
        Nickname = Species;
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
                case 0: Health.Mod += 1; break;
                case 1: Attack.Mod += 1; break;
                case 2: Defense.Mod += 1; break;
                case 3: Speed.Mod += 1; break;
            }

        }
    }

    public void CalculateStats()
    {
        Attack.Current = Attack.Base + Attack.Mod;
        Defense.Current = Defense.Base + Defense.Mod;
        Speed.Current = Speed.Base + Speed.Mod;
    }

    public void SetObjectName()
    {
        transform.name = Species + "_" + Nickname + "_Monster";
    }

    public int GetMaxHealth()
    {
        return Health.Base + Health.Mod;
    }

    public void DEBUG_DISPLAY_TEXT()
    {
        if (DEBUG_TEXT_OUTPUT != null)
        {
            string output = "Species: " + Species
                + "\n" + "Nickname: " + Nickname + "\n"
                + "Level: " + Level + "\n"
                + StatInfo("Health", Health) + StatInfo("Attack", Attack) + StatInfo("Defense", Defense) + StatInfo("Speed", Speed);
            for (int i = 0; i < ActionSet.Count; i++)
            {
                output += ActionSet[i].name + "\n";
            }
            DEBUG_TEXT_OUTPUT.GetComponent<Text>().text = output;
        }
    }

    public string StatInfo(string name, Stat input)
    {
        string output =
            name + ": " + "\n" +
            "   BAS: " + input.Base + "\n" +
            "   MOD: " + input.Mod + "\n" +
            "   CUR: " + input.Current + "\n";
        return output;
    }
}
