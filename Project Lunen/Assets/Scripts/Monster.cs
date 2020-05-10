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

    public int Species;
    public string Nickname;
    public int Level;
    public int NextXP;
    public int CurrXP;

    public Stat Health;
    public Stat Attack;
    public Stat Defense;
    public Stat Speed;

    public Action[] ActionSet;

    
    
    public bool DEBUG;

    public Text DEBUG_TEXT_OUTPUT;

    void Start()
    {
        Attack.Current = Attack.Base + Attack.Mod;
        Defense.Current = Defense.Base + Defense.Mod;
        Speed.Current = Speed.Base + Speed.Mod;
        if (DEBUG)
        {
            if (DEBUG_TEXT_OUTPUT != null)
            {
                string output = "Species: " + "!UNIMPLEMENTED!"
                    + "\n" + "Nickname: " + Nickname + "\n"
                    + StatInfo("Health", Health) + StatInfo("Attack", Attack) + StatInfo("Defense", Defense) + StatInfo("Speed", Speed);
                DEBUG_TEXT_OUTPUT.GetComponent<Text>().text = output;
            }
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
