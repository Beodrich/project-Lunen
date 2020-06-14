using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Move : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public enum LogicType
    {
        Null,
        Player,
        Trainer,
        NPC,
        Lunen
    }

    [HideInInspector] public LogicType logicType = LogicType.Null;

    [HideInInspector] public PlayerLogic pLogic;
    [HideInInspector] public TrainerLogic tLogic;

    public delegate void MoveEnd();
    [HideInInspector] public MoveEnd endMove;

    public float moveSpeed;
    public float gridSize;
    public float playerSize;
    private enum Orientation {
        Horizontal,
        Vertical
    };
    private Orientation gridOrientation = Orientation.Vertical;
    private Vector2 input;
    private Vector2 last;
    public bool isMoving = false;
    public bool animMoving = false;
    public bool npcMove = false;
    public bool diagonalMovement;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Vector3 endPosition;
    [HideInInspector] public Vector3 centerPosition;
    private float t;
    private float factor;
    private int npcFallbackMaxMoves = 30;
    public int cutsceneMoveSpaces = 0;
    public MoveScripts.Direction lookDirection;

    private Vector2 checkPoint;

    public Collider2D hit;
    public Animator animator;

    public bool ableToMove
    {
        get
        {
            switch (logicType)
            {
                default: return false;
                case LogicType.Player: return (pLogic.sr.battleSetup.PlayerCanMove() || cutsceneMoveSpaces > 0);
                case LogicType.Trainer: return (npcMove);
            }
            
        }
    }

    public bool playerInputAccepted
    {
        get
        {
            switch (logicType)
            {
                default: return false;
                case LogicType.Player: return (cutsceneMoveSpaces <= 0);
            }
            
        }
    }

    public void Awake()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();

        pLogic = GetComponent<PlayerLogic>();
        tLogic = GetComponent<TrainerLogic>();

        if (pLogic != null)
        {
            logicType = LogicType.Player;
            endMove = pLogic.MoveEnd;
        }
        if (tLogic != null)
        {
            logicType = LogicType.Trainer;
            endMove = tLogic.MoveEnd;
        }

        if (logicType == LogicType.Null) logicType = LogicType.NPC;

        animator = GetComponent<Animator>();

        SetFacingDirectionLogic(lookDirection);
    }
    
    public void StartCutsceneMove(Cutscene.Part part)
    {
        sr.eventLog.AddEvent("Started Cutscene Move!");
        npcMove = true;
        cutsceneMoveSpaces = npcFallbackMaxMoves;
        if (part.moveType == Cutscene.MoveType.ToSpaces)
        {
            cutsceneMoveSpaces = part.spacesToMove;
        }
        if (!part.moveInFacingDirection)
        {
            SetFacingDirectionLogic(part.movementDirection);
        }
    }
 
    public void Update() {
        centerPosition = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z);
        switch (logicType)
        {
            default: break;
            case LogicType.Player:
                if (!isMoving)
                {
                    if (playerInputAccepted)
                    {
                        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                        input = MoveScripts.DigitizeInput(input);
                    }

                    animMoving = false;
                    if (input != Vector2.zero && ableToMove)
                    {
                        factor = 1f;
                        last = input;
                        checkPoint = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
                        lookDirection = MoveScripts.GetDirectionFromVector2(input);
                        hit = Physics2D.OverlapArea(checkPoint,checkPoint);
                        
                        if (pLogic.MoveBegin(hit))
                        {
                            SetFacingDirection(input);
                            
                            StartCoroutine(move(transform));
                        }
                    }
                    SetWalkAnimation();
                }
                break;
            case LogicType.Trainer:
                if (!isMoving)
                {
                    input = MoveScripts.GetVector2FromDirection(lookDirection);

                    animMoving = false;
                    last = input;
                    if (input != Vector2.zero && ableToMove)
                    {
                        factor = 1f;
                        
                        checkPoint = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
                        
                        hit = Physics2D.OverlapArea(checkPoint,checkPoint);
                        
                        if (!MoveScripts.CheckForTag(this.gameObject, hit, "Player"))
                        {
                            SetFacingDirection(input);
                            StartCoroutine(move(transform));
                            
                        }
                        else
                        {
                            npcMove = false;
                            tLogic.sr.battleSetup.cutsceneAdvance = true;
                        }
                    }
                    SetWalkAnimation();
                }
            break;
            case LogicType.NPC:
                if (!isMoving)
                {
                    input = MoveScripts.GetVector2FromDirection(lookDirection);

                    animMoving = false;
                    last = input;
                    if (input != Vector2.zero && ableToMove)
                    {
                        factor = 1f;
                        
                        checkPoint = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
                        
                        hit = Physics2D.OverlapArea(checkPoint,checkPoint);
                        
                        if (!MoveScripts.CheckForTag(this.gameObject, hit, "Player"))
                        {
                            SetFacingDirection(input);
                            StartCoroutine(move(transform));
                            
                        }
                        else
                        {
                            npcMove = false;
                            tLogic.sr.battleSetup.cutsceneAdvance = true;
                        }
                    }
                    SetWalkAnimation();
                }
            break;
        }
        
    }

    public void SetWalkAnimation()
    {
        animator.SetFloat("Horizontal", last.x);
        animator.SetFloat("Vertical", last.y);
        animator.SetBool("Moving", animMoving);
    }

    public void SetFacingDirectionLogic(MoveScripts.Direction newDirection)
    {
        lookDirection = newDirection;
        SetFacingDirection(MoveScripts.GetVector2FromDirection(lookDirection));
    }

    public void SetFacingDirection(Vector2 next)
    {
        last = next;
        input = next;
        animMoving = true;
    }
 
    public IEnumerator move(Transform transform) {
        isMoving = true;
        startPosition = transform.position;
        t = 0;
        factor = 1;
 
        endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize, startPosition.y + System.Math.Sign(input.y) * gridSize, startPosition.z);
        
        while (t < 1f) {
            t += Time.deltaTime * (moveSpeed/gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
 
        isMoving = false;
        cutsceneMoveSpaces--;
        if (cutsceneMoveSpaces == 0)
        {
            npcMove = false;
            tLogic.sr.battleSetup.cutsceneAdvance = true;
        }
        endMove();
        yield return 0;
    }
}
