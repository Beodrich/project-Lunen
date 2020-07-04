using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAttributes : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [System.Serializable]
    public class Entrance
    {
        public string entranceName = "Fallback";
        public Vector2 spawn = new Vector2(0.5f,0.5f);
        public MoveScripts.Direction spawnFacing = MoveScripts.Direction.North;
        public bool moveAtStart = false;
    }
    public GameObject player;
    public List<Entrance> sceneEntrances;
    public List<Cutscene> sceneCutscenes;
    public bool spawnPlayer;
    [HideInInspector] Entrance e;

    private void Awake()
    {
        GameObject main = GameObject.Find("BattleSetup");
        if (main == null)
        {
            SceneManager.LoadScene("_preload");
            return;
        }
        sr = main.GetComponent<SetupRouter>();
        sr.sceneAttributes = this;

        sr.saveSystemObject.isLoading = false;
        sr.canvasCollection.CloseState(CanvasCollection.UIState.SceneSwitch);

        if (sr.battleSetup.loadEntrance)
        {
            e = new Entrance();
            e.spawn = sr.battleSetup.loadPosition;
            e.spawnFacing = sr.battleSetup.loadDirection;
            sr.battleSetup.loadEntrance = false;
        }
        else
        {
            int entrance = sr.battleSetup.nextEntrance;
            if (entrance >= sceneEntrances.Count) entrance = 0;
            e = sceneEntrances[entrance];
        }

        if (spawnPlayer) PreparePlayer();
    }

    public void PreparePlayer()
    {
        GameObject newPlayer = Instantiate(player);
        newPlayer.transform.position = new Vector3(e.spawn.x, e.spawn.y, 0);
        Vector2 input = MoveScripts.GetVector2FromDirection(e.spawnFacing);
        Move playerMove = newPlayer.GetComponent<Move>();
        playerMove.SetFacingDirection(input);
        playerMove.lookDirection = e.spawnFacing;
        if (e.moveAtStart)
        {
            StartCoroutine(playerMove.move(playerMove.transform)); 
        }
        playerMove.SetWalkAnimation();
    }
}
