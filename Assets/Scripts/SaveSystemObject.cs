﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemObject : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    void Awake()
    {
        sr = GetComponent<SetupRouter>();
    }
    
    public void SaveGame()
    {
        SaveSystem.SaveGameData(sr.battleSetup, sr.director.PlayerScripts[0]);
    }

    public bool LoadGame()
    {
        GameData gameData = SaveSystem.LoadGameData();
        if (gameData != null)
        {
            //Beginning load

            //Perform Clean-up
            sr.battleSetup.DestroyAllChildLunen();

            //Load standard variables
            sr.battleSetup.TrainersDefeated = gameData.TrainersDefeated;

            //Then give the player all their lunen
            for (int i = 0; i < gameData.PlayerTeam.Count; i++)
            {
                GameObject newMonsterObject = Instantiate(sr.lunaDex.MonsterTemplate);
                Monster newMonster = newMonsterObject.GetComponent<Monster>();

                newMonster.loopback = sr;
                newMonster.Level = gameData.PlayerTeam[i].level;
                newMonster.SourceLunenIndex = gameData.PlayerTeam[i].species;

                newMonster.TemplateToMonster(sr.lunaDex.GetLunen((LunaDex.LunenEnum)newMonster.SourceLunenIndex));

                newMonster.Exp.x = gameData.PlayerTeam[i].exp;
                newMonster.Health.z = gameData.PlayerTeam[i].currentHealth;
                newMonster.ActionSet.Clear();
                for (int j = 0; j < gameData.PlayerTeam[i].learnedMoves.Count; j++)
                {
                    GameObject newAction = Instantiate(sr.lunaDex.GetActionObject((LunaDex.ActionEnum)gameData.PlayerTeam[i].learnedMoves[j]));
                    newAction.GetComponent<Action>().SourceActionIndex = gameData.PlayerTeam[i].learnedMoves[j];
                    newAction.transform.parent = newMonsterObject.transform;
                    newMonster.ActionSet.Add(newAction);
                }

                sr.battleSetup.PlayerLunenTeam.Add(newMonsterObject);
                sr.director.PlayerScripts[0].LunenTeam.Add(newMonsterObject);
                newMonsterObject.transform.parent = this.transform;
            }
            sr.director.PlayerScripts[0].ReloadTeam();

            //Preparing player position, area, and facing direction
            sr.battleSetup.loadEntrance = true;
            sr.battleSetup.loadPosition = new Vector2(gameData.positionX, gameData.positionY);
            sr.battleSetup.loadDirection = (MoveScripts.Direction)gameData.facingDirection;
            sr.listOfScenes.LoadScene((ListOfScenes.LocationEnum)gameData.currentScene);
            return true;
        }
        return false;
    }
}
