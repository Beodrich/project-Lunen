using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        sr.sceneAttributes = this;

        sr.saveSystemObject.isLoading = false;

        GameObject newPlayer = Instantiate(player);
        Entrance e;

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
