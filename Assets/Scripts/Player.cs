using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

public class Player : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public bool isAI;
    [ConditionalField(nameof(isAI))] public Director.AISkill skillLevel;

    public string Name;
    public int AffinityCap;
    public int LevelCap;

    public List<GameObject> LunenTeam;
    public int LunenAlive;
    public int LunenDead;

    [HideInInspector]
    public List<Monster> LunenOut;
    [HideInInspector]
    public int LunenMax = 3;

    private void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
    }
    
    private void Update()
    {
        if (isAI)
        {
            if (sr.battleSetup.InBattle)
            {
                AI_Update();
            }
        }
    }

    public void TEST_AddTeam()
    {
        LunenTeam.Clear();
        LunenTeam.AddRange(gameObject.transform.Cast<Transform>().Where(c => c.gameObject.tag == "Monster").Select(c => c.gameObject).ToArray());
    }

    public void ReloadTeam()
    {
        if (LunenTeam.Count == 0) TEST_AddTeam();
        LunenOut.Clear();
        LunenAlive = 0;
        LunenDead = 0;

        List<GameObject> LunenGood = new List<GameObject>();
        List<GameObject> LunenBad = new List<GameObject>();

        for (int i = 0; i < LunenTeam.Count; i++)
        {
            Monster tempLunen = LunenTeam[i].GetComponent<Monster>();
            tempLunen.CalculateStats();
            if (tempLunen.Health.z <= 0)
            {
                LunenBad.Add(LunenTeam[i]);
                LunenDead++;
            }
            else
            {
                if (LunenOut.Count < LunenMax)
                {
                    LunenOut.Add(tempLunen);
                }
                LunenGood.Add(LunenTeam[i]);
                LunenAlive++;
            }
        }
        LunenTeam.Clear();
        LunenTeam.AddRange(LunenGood);
        LunenTeam.AddRange(LunenBad);
    }

    public void PrepareLunenForBattle()
    {
        for (int i = 0; i < LunenOut.Count; i++)
        {
            LunenOut[i].ResetCooldown();
        }
    }

    public void AI_Update()
    {
        for (int i = 0; i < LunenOut.Count; i++)
        {
            if (LunenOut[i].CooldownDone)
            {
                AI_DecideAction(LunenOut[i], i);
            }
        }
    }

    public void AI_DecideAction(Monster monster, int index)
    {
        //Lunen has finished cooldown, AI decides what move to use on which lunen.
        switch(skillLevel)
        {
            case Director.AISkill.Wild:
            //enemy AI will choose target and move randomly
                int movePool = 4;
                if (monster.ActionSet.Count < movePool) movePool = monster.ActionSet.Count;
                int moveChosen = Random.Range(0, movePool);
                int monsterPool = sr.director.PlayerScripts[0].LunenOut.Count;
                int monsterChosen = Random.Range(0,monsterPool);
                sr.director.EnemyLunenSelect = monsterChosen;
                sr.director.PerformAction(monster.MonsterTeam, index, moveChosen);
                monster.ResetCooldown();
            break;
        }
    }
}
