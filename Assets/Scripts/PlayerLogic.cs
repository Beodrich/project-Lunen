using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

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
    
    private Rigidbody2D rb2D;
    // Start is called before the first frame update
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        move = GetComponent<Move>();

        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        //transform.position = sr.battleSetup.lastSceneLocation;
        sr.playerLogic = this;
        sr.cameraFollow.Sel = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        move.ableToMove = !sr.battleSetup.InBattle;
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
            sr.battleSetup.lastSceneLocation = transform.position;
            sr.battleSetup.TryWildEncounter(grassObject.GetComponent<GrassEncounter>());
        }
        if (inTrainerView)
        {
            sr.battleSetup.lastSceneLocation = transform.position;
            sr.battleSetup.GenerateTrainerBattle(trainerObject.GetComponent<TrainerEncounter>());
            sr.battleSetup.MoveToBattle(0, 0);
        }
        if (inDoor)
        {
            sr.battleSetup.NewOverworld(doorObject.GetComponent<DoorToLocation>());
        }
    }
}
