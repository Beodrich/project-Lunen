using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class SceneAttributes : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public Database database;
    public GameScene thisScene;
    public List<Cutscene> sceneCutscenes;
    public bool spawnPlayer;
    [HideInInspector] public string[] doorArray;

    private void Awake()
    {
        GameObject main = GameObject.Find("BattleSetup");
        if (main == null)
        {
            return;
        }
        sr = main.GetComponent<SetupRouter>();
        sr.sceneAttributes = this;

        sr.saveSystemObject.isLoading = false;
        sr.canvasCollection.CloseState(CanvasCollection.UIState.SceneSwitch);

        if (spawnPlayer) PreparePlayer();

        if (sr.battleSetup.InCutscene)
        {
            if (sr.battleSetup.GetCurrentCutscenePart().type == CutscenePart.PartType.ChangeScene)
            {
                sr.battleSetup.AdvanceCutscene();
            }
        }
    }

    public void PreparePlayer()
    {
        GameObject newPlayer = Instantiate(database.Player);
        newPlayer.transform.position = sr.battleSetup.loadPosition;
        Vector2 input = MoveScripts.GetVector2FromDirection(sr.battleSetup.loadDirection);
        Move playerMove = newPlayer.GetComponent<Move>();
        playerMove.SetFacingDirection(input);
        playerMove.lookDirection = sr.battleSetup.loadDirection;
        if (sr.battleSetup.loadAnimTime != 0f)
        {
            playerMove.animTime = sr.battleSetup.loadAnimTime;
            playerMove.ContinueWalkAnimation();
        }
        
        if (sr.battleSetup.loadMoving)
        {
            StartCoroutine(playerMove.move(playerMove.transform)); 
        }
        playerMove.SetWalkAnimation();
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    private void Gizmo_DrawEntrance(Vector3 position, Vector3 size)
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(position, size); 
    }

    public void RefreshDoors()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        thisScene.entranceList.Clear();

        foreach (GameObject door in doors)
        {
            DoorToLocation d = door.GetComponent<DoorToLocation>();
            DatabaseSceneEntrance dse = new DatabaseSceneEntrance();
            dse.name = d.name;
            dse.facingDirection = d.exitDirection;
            dse.position = Vector3Int.FloorToInt(d.transform.position);
            dse.guid = d.GetComponent<GuidComponent>().GetGuid();
            dse.guidString = dse.guid.ToString();
            thisScene.entranceList.Add(dse);
        }

        doorArray = thisScene.GetEntrancesArray();
        
    }
}
