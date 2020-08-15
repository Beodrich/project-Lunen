using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class SceneAttributes : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    public Database database;
    public List<Cutscene> sceneCutscenes;
    public bool spawnPlayer;
    [HideInInspector] public string[] doorArray;

    private void Awake()
    {
        #if UNITY_EDITOR // conditional compilation is not mandatory
            
            GameObject findBattle = GameObject.Find("BattleSetup");
            if (findBattle == null)
            {
                database.SetTriggerValue("DEBUGTRIGGERS/FallbackPath", SceneManager.GetActiveScene().path);
                database.SetTriggerValue("DEBUGTRIGGERS/FallbackX", transform.position.x);
                database.SetTriggerValue("DEBUGTRIGGERS/FallbackY", transform.position.y);
                database.SetTriggerValue("DEBUGTRIGGERS/FallbackZ", transform.position.z);
                SceneManager.LoadScene("_preload");
            }
        #endif

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
            if (sr.battleSetup.GetCurrentCutscenePart().cutPartType == CutPartType.ChangeScene)
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
        GetCurrentScene().entranceList.Clear();
        database.GetScenesArray();

        foreach (GameObject door in doors)
        {
            DoorToLocation d = door.GetComponent<DoorToLocation>();
            DatabaseSceneEntrance dse = new DatabaseSceneEntrance();
            dse.name = d.name;
            dse.facingDirection = d.exitDirection;
            dse.position = Vector3Int.FloorToInt(d.transform.position);
            dse.guid = d.GetComponent<GuidComponent>().GetGuid();
            dse.guidString = dse.guid.ToString();
            GetCurrentScene().entranceList.Add(dse);
        }

        doorArray = GetCurrentScene().GetEntrancesArray();
        
    }

    private void OnDrawGizmos()
    {
        DrawGizmoDisk(transform, 0.5f);
    }

    private float GIZMO_DISK_THICKNESS = 0.01f;
    public void DrawGizmoDisk(Transform t, float radius)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f, 0.8f, 0.8f, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(t.position+new Vector3(0.5f, -0.5f), t.rotation, new Vector3(1, 1, GIZMO_DISK_THICKNESS));
        Gizmos.DrawSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public GameScene GetCurrentScene()
    {
        database.CreateScenesTo(GetCurrentSceneIndex());
        return database.AllScenes[GetCurrentSceneIndex()];
    }
}
