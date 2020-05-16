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
}
