using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Scene", menuName = "GameElements/Game Scene")]
public class GameScene : ScriptableObject
{
    public SceneReference scene;
    [HideInInspector] public List<DatabaseSceneEntrance> entranceList;

    public string[] GetEntrancesArray()
    {
        List<string> sceneList = new List<string>();
        foreach (DatabaseSceneEntrance gs in entranceList)
        {
            sceneList.Add(gs.name);
        }
        return sceneList.ToArray();
    }

    public DatabaseSceneEntrance IntToEntrance(int index)
    {
        if (index >= entranceList.Count)
        {
            Debug.LogError("[ERROR] Invalid Entrance Index: " + index + " (Size " + entranceList.Count + ")");
            return entranceList[0];
        }
        return entranceList[index];
    }

    public int EntranceGuidToInt(System.Guid guid)
    {
        for (int i = 0; i < entranceList.Count; i++)
        {
            if (guid == entranceList[i].guid) return i;
        }
        return -1;
    }
}
