using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public bool inGrass = false;
    public GameObject grassObject;
    [Space(10)]
    public bool inTrainerView = false;
    public GameObject trainerObject;
    [Space(10)]
    public bool inDoor = false;
    public GameObject doorObject;

    [HideInInspector]
    public Move move;
    [HideInInspector]
    public BattleSetup battle;
    private Rigidbody2D rb2D;
    // Start is called before the first frame update
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        move = GetComponent<Move>();

        GameObject battleObject = GameObject.Find("BattleSetup");
        if (battleObject != null) battle = battleObject.GetComponent<BattleSetup>();
        if (battle != null)
        {
            transform.position = battle.lastSceneLocation;
            battle.logic = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool MoveBegin(Collider2D hit)
    {
        inGrass = false;
        inTrainerView = false;
        inDoor = false;
        if (hit == null)
        {
            return true;
        }
        else
        {
            switch(hit.gameObject.tag)
            {
                default: return true;
                case "Wall": return false;
                case "Grass":
                    inGrass = true;
                    grassObject = hit.gameObject;
                    return true;
                case "TrainerSight":
                    inTrainerView = true;
                    trainerObject = hit.gameObject;
                    return true;
                case "Door":
                    inDoor = true;
                    doorObject = hit.gameObject;
                    return true;
            }
        }
    }

    public void MoveEnd()
    {
        //This function is called when the move function finished its movement.
        if (inGrass)
        {
            battle.lastSceneLocation = transform.position;
            battle.TryWildEncounter(grassObject.GetComponent<GrassEncounter>());
        }
        if (inTrainerView)
        {
            battle.lastSceneLocation = transform.position;
            battle.GenerateTrainerBattle(trainerObject.GetComponent<TrainerEncounter>());
            battle.MoveToBattle(0, 0);
        }
        if (inDoor)
        {
            battle.NewOverworld(doorObject.GetComponent<DoorToLocation>());
        }
    }
}
