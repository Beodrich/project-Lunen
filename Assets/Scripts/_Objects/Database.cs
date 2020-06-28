using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "GameElements/Internal/Database")]
public class Database : ScriptableObject
{
    public int LevelCap;
    
    public List<Item> AllItems;
    public List<Lunen> AllLunen;
    public List<Action> AllActions;

    public List<CutsceneScript> GlobalCutsceneList;

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

    public int ItemToIndex(Item item)
    {
        for (int i = 0; i < AllItems.Count; i++)
        {
            if (item == AllItems[i]) return i;
        }
        return -1;
    }

    public Item IndexToItem(int index)
    {
        return AllItems[index];
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

    public PackedCutscene GetPackedCutscene(int index)
    {
        if (index < GlobalCutsceneList.Count) return new PackedCutscene(GlobalCutsceneList[index]);
        else return null;
    }

    public PackedCutscene GetPackedCutscene(string find)
    {
        foreach (CutsceneScript cs in GlobalCutsceneList)
        {
            if (find == cs.name) return new PackedCutscene(cs);
        }
        Debug.Log("[ERROR] Unable To Find Cutscene: " + find);
        return null;
    }
}
