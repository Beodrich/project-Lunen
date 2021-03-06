﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameScene
{
    public string name;
    public List<DatabaseSceneEntrance> entranceList;

    public string[] GetEntrancesArray()
    {
        List<string> sceneList = new List<string>();
        foreach (DatabaseSceneEntrance gs in entranceList)
        {
            //sceneList.Add(gs.name + " - " + gs.guidString);
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

    public DatabaseSceneEntrance GuidToEntrance(string guid)
    {
        for (int i = 0; i < entranceList.Count; i++)
        {
            if (guid == entranceList[i].guidString) return entranceList[i];
        }
        return null;
    }

    public int EntranceGuidToInt(string guid)
    {
        for (int i = 0; i < entranceList.Count; i++)
        {
            if (guid == entranceList[i].guidString) return i;
        }
        return -1;
    }

    public string IntToGuid(int index)
    {
        if (index < entranceList.Count)
        {
            return entranceList[index].guidString;
        }
        else return "ERROR";
    }
}
