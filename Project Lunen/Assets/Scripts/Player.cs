using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string Name;
    public int AffinityCap;
    public int LevelCap;

    public List<GameObject> LunenInParty;
    public List<GameObject> LunenSidelined;
    public List<GameObject> LunenDead;

    [HideInInspector]
    public List<Monster> MonstersInParty;
    [HideInInspector]
    public List<Monster> MonstersSidelined;
    [HideInInspector]
    public List<Monster> MonstersDead;

    public void ReReferenceMonsters()
    {
        MonstersInParty.Clear();
        for (int i = 0; i < LunenInParty.Count; i++)
        {
            MonstersInParty.Add(LunenInParty[i].GetComponent<Monster>());
        }

        MonstersSidelined.Clear();
        for (int i = 0; i < LunenSidelined.Count; i++)
        {
            MonstersSidelined.Add(LunenSidelined[i].GetComponent<Monster>());
        }

        MonstersDead.Clear();
        for (int i = 0; i < LunenDead.Count; i++)
        {
            MonstersDead.Add(LunenDead[i].GetComponent<Monster>());
        }
    }
}
