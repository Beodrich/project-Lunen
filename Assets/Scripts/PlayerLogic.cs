using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public List<AnimationSet> playerAnimSets;

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

    public void MoveEnd()
    {
        //This function is called when the move function finished its movement.
        if (move.moveDetection.inGrass && !sr.battleSetup.cutsceneLoopGoing)
        {
            sr.battleSetup.lastSceneLocation = transform.position;
            sr.battleSetup.TryWildEncounter(move.moveDetection.grassObject.GetComponent<GrassEncounter>());
        }
        if (move.moveDetection.inDoor)
        {
            sr.battleSetup.NewOverworld(move.moveDetection.doorObject.GetComponent<DoorToLocation>(), transform.position - move.moveDetection.doorObject.transform.position);
        }

        if (move.moveDetection.inShop)
        {
            sr.canvasCollection.GetShopStats(move.moveDetection.shopObject.GetComponent<UI_Shop>());
            sr.canvasCollection.OpenState(CanvasCollection.UIState.Shop);
            sr.canvasCollection.OpenInventoryWindow();
        }
        else if (move.moveDetection.shopObject != null)
        {
            //shopObject.GetComponent<ShopTrigger>().shop.Hide();
            sr.canvasCollection.CloseState(CanvasCollection.UIState.Shop);
            sr.canvasCollection.CloseInventoryWindow(true);
            move.moveDetection.shopObject = null;
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
