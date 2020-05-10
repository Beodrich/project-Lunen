using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        public int Base;
        public int Mod;
        public int Current;
    }

    public Stat Health;
    public Stat Attack;
    public Stat Defense;
    public Stat Speed;

    public Move[] MoveSet;

    public int Level;
    
    public bool DEBUG;

    public UnityEngine.UI.Text DEBUG_TEXT_OUTPUT;
}
