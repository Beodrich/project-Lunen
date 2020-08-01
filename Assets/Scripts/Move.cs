using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Move : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public GameObject srObject;

    public enum LogicType
    {
        Null,
        Player,
        Trainer,
        NPC,
        Lunen
    }
    [Foldout("Advanced Settings", true)] 

    [HideInInspector] public LogicType logicType = LogicType.Null;

    [HideInInspector] public PlayerLogic pLogic;
    [HideInInspector] public TrainerLogic tLogic;

    [HideInInspector] public SpriteRenderer spriteRenderer;

    public delegate void MoveEnd();
    [HideInInspector] public MoveEnd endMove;

    public AnimationSet animationSet;

    public float moveSpeed;
    public float gridSize;
    public float playerSize;
    private Vector2 input;
    private Vector2 last;
    [ReadOnly] [SerializeField] private Vector2 thisDirectInput;
    [ReadOnly] [SerializeField] private Vector2 lastDirectInput;
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
    public Collider2D[] hits;

    private float lastAnimX;
    private float lastAnimY;
    private bool lastAnimMoving;

    public float animTime;
    public int animIndex;
    public string animationType;
    public bool animHijack;

    public float moveSpeedModifier;
    public float moveSpeedCurrent;

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

    public void Start()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();

        pLogic = GetComponent<PlayerLogic>();
        tLogic = GetComponent<TrainerLogic>();



        spriteRenderer = GetComponent<SpriteRenderer>();

        if (pLogic != null)
        {
            logicType = LogicType.Player;
            endMove = pLogic.MoveEnd;
            
        }
        else if (tLogic != null)
        {
            logicType = LogicType.Trainer;
            endMove = tLogic.MoveEnd;
        }
        else
        {
            logicType = LogicType.NPC;
            endMove = MoveEndStub;
        }
        moveSpeedCurrent = moveSpeed;
    }
    
    public void StartCutsceneMove(CutPart _part)
    {
        CutPart_Movement part = (CutPart_Movement)_part;
        sr.eventLog.AddEvent("Started Cutscene Move!");
        npcMove = true;
        cutsceneMoveSpaces = npcFallbackMaxMoves;
        if (part.moveType == MoveType.ToSpaces)
        {
            cutsceneMoveSpaces = part.spacesToMove;
        }
        if (part.chooseMoveDirection)
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
                        thisDirectInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                        input = MoveScripts.DigitizeInput(thisDirectInput, lastDirectInput, input);
                        lastDirectInput = thisDirectInput;
                    }

                    animMoving = false;
                    
                    if (input != Vector2.zero && ableToMove)
                    {
                        factor = 1f;
                        last = input;
                        Vector3 checkPoint1 = new Vector2(centerPosition.x+input.x+0.25f, centerPosition.y+input.y+0.25f);
                        Vector3 checkPoint2 = new Vector2(centerPosition.x+input.x-0.25f, centerPosition.y+input.y-0.25f);
                        lookDirection = MoveScripts.GetDirectionFromVector2(input);
                        hits = Physics2D.OverlapAreaAll(checkPoint1,checkPoint2);
                        
                        if (pLogic.MoveBegin(hits))
                        {
                            bool cancelMove = false;
                            if (pLogic.inTrain)
                            {
                                sr.battleSetup.StartCutscene(new PackedCutscene(pLogic.trainObject.GetComponent<Cutscene>()));
                                
                            }
                            if (pLogic.inDoor)
                            {
                                if (pLogic.doorObject.GetComponent<DoorToLocation>().targetScene != null)
                                {
                                    if (pLogic.doorObject.GetComponent<DoorToLocation>().fadeOutOnTransition)
                                    {
                                        sr.canvasCollection.OpenState(CanvasCollection.UIState.SceneSwitch);
                                    }
                                }
                                else
                                {
                                    cancelMove = true;
                                }
                            }
                            SetFacingDirection(input);

                            if (!cancelMove)
                            {
                                if (sr.battleSetup.runButtonHeld) moveSpeedCurrent = moveSpeed * moveSpeedModifier;
                                else moveSpeedCurrent = moveSpeed;
                                StartCoroutine(move(transform));
                            }
                            
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
                        
                        hits = Physics2D.OverlapAreaAll(checkPoint,checkPoint);
                        
                        if (tLogic.MoveBegin(hits))
                        {
                            
                            SetFacingDirection(input);
                            StartCoroutine(move(transform));
                            
                        }
                        else
                        {
                            npcMove = false;
                            tLogic.sr.battleSetup.AdvanceCutscene();
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
                    if (input != Vector2.zero && npcMove)
                    {
                        factor = 1f;
                        
                        checkPoint = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
                        
                        hit = Physics2D.OverlapArea(checkPoint,checkPoint);

                        //MoveScripts.CheckForTag(this.gameObject, hit, "Player")
                        SetFacingDirection(input);
                        StartCoroutine(move(transform));
                    }
                    SetWalkAnimation();
                }
            break;
        }
        
    }

    public void ContinueWalkAnimation()
    {
        lastAnimX = last.x;
        lastAnimY = last.y;
        lastAnimMoving = isMoving;
    }

    public void SetWalkAnimation()
    {
        bool animChanged = false;
        if (lastAnimX != last.x) animChanged = true;
        if (lastAnimY != last.y) animChanged = true;

        lastAnimX = last.x;
        lastAnimY = last.y;
        lastAnimMoving = isMoving;

        if (animChanged)
        {
            animTime = 0;
        }
        else
        {
            animTime += Time.deltaTime * ((moveSpeedCurrent > moveSpeed) ? 2f : 1f);
        }

        if (animHijack)
        {
            if (animTime > animationSet.GetAnimation(animationType).loopTime)
            {
                animHijack = false;
            }
        }
        else
        {
            if (animMoving)
            {
                animationType = "Walk";
            }
            else
            {
                animationType = "Idle";
            }
        }

        

        

        animIndex = animationSet.GetAnimationIndex(animationType, lookDirection, animTime);
        SetSprite();
        //animator.SetFloat("Horizontal", last.x);
        //animator.SetFloat("Vertical", last.y);
        //animator.SetBool("Moving", animMoving);
    }

    public void SetSprite()
    {
        spriteRenderer.sprite = animationSet.GetAnimationSprite(animationType, animIndex);
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
            t += Time.deltaTime * (moveSpeedCurrent/gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            SetWalkAnimation();
            yield return null;
        }
 
        isMoving = false;
        cutsceneMoveSpaces--;
        if (cutsceneMoveSpaces == 0)
        {
            npcMove = false;
            sr.battleSetup.AdvanceCutscene();
        }
        endMove();
        yield return 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (sr == null)
        {
            srObject = GameObject.Find("BattleSetup");
            if (srObject != null)
            {
                sr = srObject.GetComponent<SetupRouter>();
            }
        }
        animTime = 0;
        SetWalkAnimation();
        SetSprite();
    }

    private void MoveEndStub()
    {
        //
    }
}
