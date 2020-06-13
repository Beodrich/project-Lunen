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
    public bool isMoving = false;
    public bool diagonalMovement;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;

    public RaycastHit2D hit;
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
        startPosition = transform.position;
    }
    
 
    public void Update() {
        if (!isMoving) {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                {
                    input.y = 0;
                }
                else
                {
                    input.x = 0;
                }
            

            if (input != Vector2.zero)
            {
                factor = 1f;
                hit = Physics2D.Raycast(transform.position, new Vector2(input.x, input.y), playerSize/factor);
                if (logic.MoveBegin(hit.collider))
                {
                    last = input;
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
        animator.SetBool("Moving", (input != Vector2.zero));
    }
 
    public IEnumerator move(Transform transform) {
        isMoving = true;
        startPosition = transform.position;
        t = 0;
 
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
