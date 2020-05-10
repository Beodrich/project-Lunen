using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    struct Stat
    {
        int Base;
        int Mod;
        int Current;
    }

    private Stat Health;
    private Stat Attack;
    private Stat Defense;
    private Stat Speed;

    public int Level;
}
