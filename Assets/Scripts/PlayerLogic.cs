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
    public Vector3 frontOfCharacter;
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
        if (sr.battleSetup.PlayerCanMove() && !move.isMoving)
        {
            if (Input.GetButtonDown("Submit"))
            {
                Vector2 faceDirection = MoveScripts.GetVector2FromDirection(MoveScripts.GetOppositeDirection(move.lookDirection));
                Vector2 checkPoint = MoveScripts.GetFrontVector2(move, 1, true);
                Collider2D[] hit = Physics2D.OverlapAreaAll(checkPoint,checkPoint);
                InteractBegin(hit);
            }
        }
    }

    public void InteractBegin(Collider2D[] hit)
    {
        for (int i = 0; i < hit.Length; i++)
        {
            switch(hit[i].gameObject.tag)
            {
                default: break;
                case "Trainer":
                    StartNPCConversation(hit[i].gameObject);
                break;
                case "Thing":
                    StartNPCConversation(hit[i].gameObject);
                break;
                case "NPC":
                    StartNPCConversation(hit[i].gameObject);
                break;
            }
        }
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
                case "Creature": return false;
                case "Trainer": return false;
                case "Thing": return false;
                case "NPC": return false;
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

    public bool MoveBegin(Collider2D[] hit)
    {
        inGrass = false;
        inTrainerView = false;
        inDoor = false;
        bool inGrass2 = false;
        bool inTrainerView2 = false;
        bool inDoor2 = false;
        if (hit.Length > 0)
        {
            int found = 0;
            for (int i = 0; i < hit.Length; i++)
            {
                found += MoveBegin(hit[i]) ? 1 : 0;
                if (inGrass) inGrass2 = true;
                if (inTrainerView) inTrainerView2 = true;
                if (inDoor) inDoor2 = true;
            }
            inGrass = inGrass2;
            inTrainerView = inTrainerView2;
            inDoor = inDoor2;
            return (found > 0);
        }
        
        else return true;
    }

    public void MoveEnd()
    {
        //This function is called when the move function finished its movement.
        if (inGrass)
        {
            sr.battleSetup.lastSceneLocation = transform.position;
            sr.battleSetup.TryWildEncounter(grassObject.GetComponent<GrassEncounter>());
        }
        if (inDoor)
        {
            sr.battleSetup.NewOverworld(doorObject.GetComponent<DoorToLocation>());
        }
    }

    public bool StartNPCConversation(GameObject npc)
    {
        //Make the npc turn to face you
        Move moveScript = npc.GetComponent<Move>();
        if (moveScript != null) moveScript.SetFacingDirectionLogic(MoveScripts.GetOppositeDirection(move.lookDirection));

        //Check if the npc has a cutscene attached and run it.
        Cutscene c = npc.GetComponent<Cutscene>();
        if (c != null) sr.battleSetup.StartCutscene(c); else return false;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        frontOfCharacter = MoveScripts.GetFrontVector2(move, 1, true);
        Gizmos.DrawCube(frontOfCharacter, new Vector3(1,1,1)); 
    }
}
