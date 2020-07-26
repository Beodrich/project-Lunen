﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MyBox;


public static class SaveSystem
{
    public static void SaveGameData(SetupRouter sr)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/GameData.sav", FileMode.Create);

        GameData data = new GameData(sr);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGameData()
    {
        if (File.Exists(Application.persistentDataPath + "/GameData.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/GameData.sav", FileMode.Open);

            GameData gameData = bf.Deserialize(stream) as GameData;

            stream.Close();
            return gameData;
        }
        else return null;
    }
}

[Serializable]
public class GameData
{
    [Serializable]
    public class PlayerLunen
    {
        public int species;
        public string nickname;
        public int level;
        public int exp;
        public int currentHealth;
        public List<int> learnedMoves;

    }
    [Serializable]
    public class InventoryItem
    {
        public int itemIndex;
        public int itemAmount;
    }

    [Serializable]
    public class TriggerSet
    {
        public string setName;
        public List<TriggerSave> triggerParts = new List<TriggerSave>();
    }

    [Serializable]
    public class TriggerSave
    {
        public string triggerTITLE;
        public TriggerTypes triggerType;

        public bool triggerBool;
        public int triggerInt;
        public float triggerFloat;
        public double triggerDouble;
        public string triggerString;
    }

    public List<PlayerLunen> PlayerTeam;
    public List<PlayerLunen> StorageLunen;
    public List<InventoryItem> InventoryItems;
    public List<System.Guid> GuidList;
    public List<TriggerSet> SaveTriggers;
    public float positionX;
    public float positionY;
    public string currentScene;
    public int facingDirection;

    public float respawnX;
    public float respawnY;
    public string respawnScene;
    public int respawnDirection;

    public static PlayerLunen GeneratePlayerLunen(Monster currentMonster, SetupRouter sr)
    {
        PlayerLunen a = new PlayerLunen();
        a.species = sr.database.LunenToIndex(currentMonster.SourceLunen);
        a.nickname = currentMonster.Nickname;
        a.level = currentMonster.Level;
        a.exp = currentMonster.Exp.x;
        a.currentHealth = currentMonster.Health.z;
        a.learnedMoves = new List<int>();
        for (int j = 0; j < currentMonster.ActionSet.Count; j++)
        {
            a.learnedMoves.Add(sr.database.ActionToIndex(currentMonster.ActionSet[j]));
        }
        return a;
    }
    

    public GameData(SetupRouter sr)
    {
        PlayerTeam = new List<PlayerLunen>();
        //Debug.Log("Found " + sr.battleSetup.PlayerLunenTeam.Count + " Lunen!");
        foreach (GameObject monster in sr.battleSetup.PlayerLunenTeam) PlayerTeam.Add(GeneratePlayerLunen(monster.GetComponent<Monster>(), sr));
        StorageLunen = sr.storageSystem.StoredLunen;

        InventoryItems = new List<InventoryItem>();
        //Debug.Log("Found " + sr.inventory.listOfItems.Count + " Item" + (sr.inventory.listOfItems.Count == 1 ? "" : "s") + "!");
        for (int i = 0; i < sr.inventory.listOfItems.Count; i++)
        {
            Inventory.InventoryEntry currentItem = sr.inventory.listOfItems[i];
            InventoryItem a = new InventoryItem();
            a.itemIndex = sr.database.ItemToIndex(currentItem.item);
            a.itemAmount = currentItem.amount;
            InventoryItems.Add(a);
        }

        SaveTriggers = new List<TriggerSet>();
        for (int i = 0; i < sr.database.GlobalStoryTriggerList.Count; i++)
        {
            TriggerSet set = new TriggerSet();
            set.setName = sr.database.GlobalStoryTriggerList[i].name;
            foreach (TriggerPart part in sr.database.GlobalStoryTriggerList[i].triggerParts)
            {
                TriggerSave save = new TriggerSave();
                save.triggerTITLE = part.title;
                save.triggerType = part.type;
                save.triggerBool = part.triggerBool;
                save.triggerInt = part.triggerInt;
                save.triggerFloat = part.triggerFloat;
                save.triggerDouble = part.triggerDouble;
                save.triggerString = part.triggerString;
                set.triggerParts.Add(save);
            }
            SaveTriggers.Add(set);
        }

        GuidList = sr.battleSetup.GuidList;
        positionX = sr.playerLogic.transform.position.x;
        positionY = sr.playerLogic.transform.position.y;
        currentScene = sr.battleSetup.lastOverworld;
        facingDirection = (int)sr.playerLogic.move.lookDirection;

        respawnX = sr.battleSetup.respawnLocation.x;
        respawnY = sr.battleSetup.respawnLocation.y;
        respawnScene = sr.battleSetup.respawnScene;
        respawnDirection = (int)sr.battleSetup.respawnDirection;
    }
}
