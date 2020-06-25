using System.Collections;
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

    public List<PlayerLunen> PlayerTeam;
    public List<System.Guid> TrainersDefeated;
    public float positionX;
    public float positionY;
    public string currentScene;
    public int facingDirection;

    public float respawnX;
    public float respawnY;
    public string respawnScene;
    public int respawnDirection;
    

    public GameData(SetupRouter sr)
    {
        PlayerTeam = new List<PlayerLunen>();
        Debug.Log("Found " + sr.battleSetup.PlayerLunenTeam.Count + " Lunen!");
        for (int i = 0; i < sr.battleSetup.PlayerLunenTeam.Count; i++)
        {
            Monster currentMonster = sr.battleSetup.PlayerLunenTeam[i].GetComponent<Monster>();
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
            PlayerTeam.Add(a);
            
        }

        TrainersDefeated = sr.battleSetup.TrainersDefeated;
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
