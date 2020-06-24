using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MyBox;


public static class SaveSystem
{
    public static void SaveGameData(BattleSetup bs)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/GameData.sav", FileMode.Create);

        GameData data = new GameData(bs);

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
    public SceneReference currentScene;
    public int facingDirection;

    public float respawnX;
    public float respawnY;
    public SceneReference respawnScene;
    public int respawnDirection;
    

    public GameData(BattleSetup bs)
    {
        PlayerTeam = new List<PlayerLunen>();
        Debug.Log("Found " + bs.PlayerLunenTeam.Count + " Lunen!");
        for (int i = 0; i < bs.PlayerLunenTeam.Count; i++)
        {
            Monster currentMonster = bs.PlayerLunenTeam[i].GetComponent<Monster>();
            PlayerLunen a = new PlayerLunen();
            a.species = currentMonster.SourceLunenIndex;
            a.nickname = currentMonster.Nickname;
            a.level = currentMonster.Level;
            a.exp = currentMonster.Exp.x;
            a.currentHealth = currentMonster.Health.z;
            a.learnedMoves = new List<int>();
            for (int j = 0; j < currentMonster.ActionSet.Count; j++)
            {
                a.learnedMoves.Add(currentMonster.ActionSet[j].GetComponent<Action>().SourceActionIndex);
            }
            PlayerTeam.Add(a);
            
        }

        TrainersDefeated = bs.TrainersDefeated;
        positionX = bs.sr.playerLogic.transform.position.x;
        positionY = bs.sr.playerLogic.transform.position.y;
        currentScene = bs.lastOverworld;
        facingDirection = (int)bs.sr.playerLogic.move.lookDirection;

        respawnX = bs.respawnLocation.x;
        respawnY = bs.respawnLocation.y;
        respawnScene = bs.respawnScene;
        respawnDirection = (int)bs.respawnDirection;
    }
}
