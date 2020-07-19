using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemObject : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public bool isLoading;

    void Awake()
    {
        sr = GetComponent<SetupRouter>();
    }
    
    public void SaveGame()
    {
        SaveSystem.SaveGameData(sr);
    }

    public bool LoadGame()
    {
        GameData gameData = SaveSystem.LoadGameData();
        if (gameData != null)
        {
            //Beginning load
            isLoading = true;
            if (sr.battleSetup.InBattle) sr.battleSetup.ExitBattleState();
            sr.battleSetup.InCutscene = false;
            sr.battleSetup.gamePaused = false;
            sr.battleSetup.ForceEndCutscene();
            sr.battleSetup.choiceOpen = false;
            sr.battleSetup.dialogueBoxNext = false;
            sr.battleSetup.dialogueBoxOpen = false;

            sr.canvasCollection.CloseState(CanvasCollection.UIState.MainMenu);
            sr.canvasCollection.CloseState(CanvasCollection.UIState.Options);
            sr.canvasCollection.MenuPanelOpen = false;
            sr.canvasCollection.OptionsPanelOpen = false;
            

            //Perform Clean-up
            sr.battleSetup.DestroyAllChildLunen();

            //Load standard variables
            sr.battleSetup.GuidList = gameData.GuidList;

            //Give the player all their items
            sr.inventory.listOfItems.Clear();
            foreach (GameData.InventoryItem a in gameData.InventoryItems)
                sr.inventory.listOfItems.Add(new Inventory.InventoryEntry(sr.database.IndexToItem(a.itemIndex), a.itemAmount));

            //Restore Save Triggers
            foreach (GameData.TriggerSet triggerSet in gameData.SaveTriggers)
            {
                StoryTrigger st = sr.database.StringToStoryTrigger(triggerSet.setName);
                foreach (GameData.TriggerSave save in triggerSet.triggerParts)
                {
                    switch (save.triggerType)
                    {
                        case TriggerTypes.Bool: st.SetTriggerValue(save.triggerTITLE, save.triggerBool); break;
                        case TriggerTypes.Int: st.SetTriggerValue(save.triggerTITLE, save.triggerInt); break;
                        case TriggerTypes.Float: st.SetTriggerValue(save.triggerTITLE, save.triggerFloat); break;
                        case TriggerTypes.Double: st.SetTriggerValue(save.triggerTITLE, save.triggerDouble); break;
                        case TriggerTypes.String: st.SetTriggerValue(save.triggerTITLE, save.triggerString); break;
                    }
                }
            }

            //Then give the player all their lunen
            for (int i = 0; i < gameData.PlayerTeam.Count; i++)
            {
                GameObject newMonsterObject = sr.generateMonster.GenerateLunen(
                    sr.database.IndexToLunen(gameData.PlayerTeam[i].species),
                    gameData.PlayerTeam[i].level,
                    GenerateMonster.SortMovesType.None
                    );
                Monster newMonster = newMonsterObject.GetComponent<Monster>();

                newMonster.Exp.x = gameData.PlayerTeam[i].exp;
                newMonster.Health.z = gameData.PlayerTeam[i].currentHealth;
                
                for (int j = 0; j < gameData.PlayerTeam[i].learnedMoves.Count; j++)
                {
                    newMonster.ActionSet.Add(sr.database.IndexToAction(gameData.PlayerTeam[i].learnedMoves[j]));
                }

                sr.battleSetup.PlayerLunenTeam.Add(newMonsterObject);
                newMonsterObject.transform.parent = this.transform;
            }
            sr.director.LoadTeams();

            sr.battleSetup.lastOverworld = gameData.currentScene;
            

            //Get respawn variables
            sr.battleSetup.respawnLocation = new Vector2(gameData.respawnX, gameData.respawnY);
            sr.battleSetup.respawnScene = gameData.respawnScene;
            sr.battleSetup.respawnDirection = (MoveScripts.Direction)gameData.respawnDirection;

            //Preparing player position, area, and facing direction
            sr.battleSetup.NewOverworldAt(sr.battleSetup.lastOverworld, new Vector2(gameData.positionX, gameData.positionY), (MoveScripts.Direction)gameData.facingDirection);
            
            return true;
        }
        return false;
    }
}
