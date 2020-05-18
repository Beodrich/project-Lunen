using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
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
}
