using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MyBox;


public static class SaveSystem
{
    public static void SaveGameData(BattleSetup bs, Player player)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/GameData.sav", FileMode.Create);

        GameData data = new GameData(bs, player);

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
    public int currentScene;
    public int facingDirection;

    public GameData(BattleSetup bs, Player player)
    {
        player.ReloadTeam();
        PlayerTeam = new List<PlayerLunen>();
        for (int i = 0; i < player.LunenTeam.Count; i++)
        {
            Monster currentMonster = player.LunenTeam[i].GetComponent<Monster>();
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
        currentScene = (int)bs.lastOverworld;
        facingDirection = (int)bs.sr.playerLogic.move.lookDirection;
    }
}
