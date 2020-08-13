﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public List<AnimationSet> playerAnimSets;

    public bool inGrass = false;
    public GameObject grassObject;
    [Space(10)]
    public bool inTrainerView = false;
    public GameObject trainerObject;
    [Space(10)]
    public bool inDoor = false;
    public GameObject doorObject;
    [Space(10)]
    public bool inShop = false;
    public GameObject shopObject;
    [Space(10)]
    public bool inTrain = false;
    public GameObject trainObject;

    [HideInInspector]
    public Move move;
    
    private Rigidbody2D rb2D;
    public Vector3 frontOfCharacter;
    WallWalkScript wws;
    // Start is called before the first frame update
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        move = GetComponent<Move>();

        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        //transform.position = sr.battleSetup.lastSceneLocation;
        sr.playerLogic = this;
        sr.cameraFollow.Sel = this.gameObject;

        int animChoose = (int)sr.database.GetTriggerValue("PlayerInfo/AnimSet");
        if (animChoose >= playerAnimSets.Count) animChoose = 0;
        move.animationSet = playerAnimSets[animChoose];
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.battleSetup.PlayerCanMove() && !move.isMoving )
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
        inShop = false;
        inTrain = false;

        if (hit == null)
        {
            return true;
        }
        else
        {
            switch(hit.gameObject.tag)
            {
                default: return true;
                case "Wall":
                    wws = hit.gameObject.GetComponent<WallWalkScript>();
                    if (wws != null)
                    {
                        switch (move.lookDirection)
                        {
                            default: return false;
                            case MoveScripts.Direction.North: return wws.CanWalkNorth;
                            case MoveScripts.Direction.South: return wws.CanWalkSouth;
                            case MoveScripts.Direction.East: return wws.CanWalkEast;
                            case MoveScripts.Direction.West: return wws.CanWalkWest;
                        }
                    }
                    else
                    {
                        return false;
                    }
                
                case "Water": return false;
                case "Creature": return false;
                case "Trainer": return false;
                case "Thing": return false;
                case "NPC": return false;
                case "ShopKeeper":
                    inShop = true;
                    shopObject = hit.gameObject;
                    return true;
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
                case "Train":
                    inTrain = true;
                    trainObject = hit.gameObject;
                    return true;
            }
        }
    }

    public bool MoveBegin(Collider2D[] hit)
    {
        inGrass = false;
        inTrainerView = false;
        inDoor = false;
        inShop = false;
        inTrain = false;

        bool inGrass2 = false;
        bool inTrainerView2 = false;
        bool inDoor2 = false;
        bool inShop2 = false;
        bool inTrain2 = false;

        int pathsFound = 0;
        if (hit.Length > 0)
        {
            int found = 0;
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].gameObject.tag != "Path")
                {
                    if (hit[i].gameObject.tag != "Grass")
                    {
                        found += MoveBegin(hit[i]) ? 1 : 0;
                        if (inTrainerView) inTrainerView2 = true;
                        if (inDoor) inDoor2 = true;
                        if (inShop) inShop2 = true;
                        if (inTrain) inTrain2 = true;
                    }
                    else
                    {
                        inGrass = true;
                        grassObject = hit[i].gameObject;
                        inGrass2 = true;
                        pathsFound++;
                    }
                    
                }
                else
                {
                    pathsFound++;
                }
                
            }
            inGrass = inGrass2;
            inTrainerView = inTrainerView2;
            inDoor = inDoor2;
            inShop = inShop2;
            inTrain = inTrain2;
            
            if (pathsFound == hit.Length)
            {
                return true;
            }
            return (found > 0);
        }
        else return false;
    }

    public void MoveEnd()
    {
        //This function is called when the move function finished its movement.
        if (inGrass && !sr.battleSetup.cutsceneLoopGoing)
        {
            sr.battleSetup.lastSceneLocation = transform.position;
            sr.battleSetup.TryWildEncounter(grassObject.GetComponent<GrassEncounter>());
        }
        if (inDoor)
        {
            sr.battleSetup.NewOverworld(doorObject.GetComponent<DoorToLocation>(), transform.position - doorObject.transform.position);
        }

        if (inShop)
        {
            sr.canvasCollection.GetShopStats(shopObject.GetComponent<UI_Shop>());
            sr.canvasCollection.OpenState(CanvasCollection.UIState.Shop);
            sr.canvasCollection.OpenInventoryWindow();
        }
        else if (shopObject != null)
        {
            //shopObject.GetComponent<ShopTrigger>().shop.Hide();
            sr.canvasCollection.CloseState(CanvasCollection.UIState.Shop);
            sr.canvasCollection.CloseInventoryWindow(true);
            shopObject = null;
        }
    }

    public bool StartNPCConversation(GameObject npc)
    {
        //Make the npc turn to face you
        Move moveScript = npc.GetComponent<Move>();
        if (moveScript != null) moveScript.SetFacingDirectionLogic(MoveScripts.GetOppositeDirection(move.lookDirection));

        //Check if the npc is a trainer and set the appropriate route
        string route = "";
        if (npc.GetComponent<TrainerLogic>() != null)
        {
            if (npc.GetComponent<TrainerLogic>().defeated) route = "Trainer Defeated";
        }

        //Check if the npc has a cutscene attached and run it.
        PackedCutscene c = new PackedCutscene(npc.GetComponent<Cutscene>());
        if (c != null) sr.battleSetup.StartCutscene(c, route); else return false;

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        frontOfCharacter = MoveScripts.GetFrontVector2(move, 1, true);
        Gizmos.DrawCube(frontOfCharacter, new Vector3(0.5f,0.5f,1)); 
    }
}
