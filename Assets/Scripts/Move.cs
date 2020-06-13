using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Move : MonoBehaviour
{
    [HideInInspector]
    public PlayerLogic logic;



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
    public bool ableToMove = true;
    public bool isMoving = false;
    public bool animMoving = false;
    public bool diagonalMovement;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Vector3 endPosition;
    [HideInInspector] public Vector3 centerPosition;
    private float t;
    private float factor;

    private Vector2 checkPoint1;
    private Vector2 checkPoint2;


    public Collider2D hit;
    public Animator animator;

/*
    public bool HasCooldown
    {
        get
        {
            return cooldownCount > 0;
        }
    }
    */

    public void Awake()
    {
        logic = GetComponent<PlayerLogic>();
        animator = GetComponent<Animator>();
    }
    
 
    public void Update() {
        centerPosition = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z);
        if (!isMoving) {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                {
                    input.y = 0;
                    if (input.x > 0) input.x = 1; else if (input.x < 0) input.x = -1;
                }
                else
                {
                    input.x = 0;
                    if (input.y > 0) input.y = 1; else if (input.y < 0) input.y = -1;
                }

            animMoving = false;
            if (input != Vector2.zero && ableToMove)
            {
                factor = 1f;
                last = input;
                checkPoint1 = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
                checkPoint2 = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
                
                hit = Physics2D.OverlapArea(checkPoint1,checkPoint2);
                
                if (logic.MoveBegin(hit))
                {
                    SetFacingDirection(input);
                    StartCoroutine(move(transform));
                }
            }
            SetWalkAnimation();
        }
    }

    public void SetWalkAnimation()
    {
        animator.SetFloat("Horizontal", last.x);
        animator.SetFloat("Vertical", last.y);
        animator.SetBool("Moving", animMoving);
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
        logic.MoveEnd();
        yield return 0;
    }
}
