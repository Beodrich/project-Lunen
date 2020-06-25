using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "GameElements/Internal/Database")]
public class Database : ScriptableObject
{
    public int LevelCap;
    
    public List<ScriptableItem> AllItems;
    public List<Lunen> AllLunen;
    public List<Action> AllActions;

    public List<Cutscene> GlobalCutsceneList;

    public GameObject MonsterTemplate;

    public int LunenToIndex(Lunen lunen)
    {
        for (int i = 0; i < AllLunen.Count; i++)
        {
            if (lunen == AllLunen[i]) return i;
        }
        return -1;
    }

    public Lunen IndexToLunen(int index)
    {
        return AllLunen[index];
    }

    public int ActionToIndex(Action action)
    {
        for (int i = 0; i < AllActions.Count; i++)
        {
            if (action == AllActions[i]) return i;
        }
        return -1;
    }

    public Action IndexToAction(int index)
    {
        return AllActions[index];
    }
}
