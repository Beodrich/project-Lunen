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
        public LunaDex.Direction spawnFacing = LunaDex.Direction.North;
        public bool moveAtStart = false;
    }
    public GameObject player;
    public List<Entrance> sceneEntrances;

    private void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        sr.sceneAttributes = this;
        int entrance = sr.battleSetup.nextEntrance;
        if (entrance >= sceneEntrances.Count) entrance = 0;
        GameObject newPlayer = Instantiate(player);
        newPlayer.transform.position = new Vector3(sceneEntrances[entrance].spawn.x, sceneEntrances[entrance].spawn.y, 0);
        Vector2 input = sr.lunaDex.GetDirectionVector2(sceneEntrances[entrance].spawnFacing);
        Move playerMove = newPlayer.GetComponent<Move>();
        playerMove.SetFacingDirection(input);
        if (sceneEntrances[entrance].moveAtStart)
        {
            StartCoroutine(playerMove.move(playerMove.transform)); 
        }
        playerMove.SetWalkAnimation();
    }
}
