using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Move : MonoBehaviour
{
    [HideInInspector]
    public PlayerLogic logic;

    
    public enum checkDirection
    {
        North,
        South,
        East,
        West,
        Null
    }

    public float moveSpeed;
    public float gridSize;
    public float playerSize;
    private enum Orientation {
        Horizontal,
        Vertical
    };
    private Orientation gridOrientation = Orientation.Vertical;
    private Vector2 input;
    public bool isMoving = false;
    public bool diagonalMovement;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;

    public checkDirection goDirection;
    public RaycastHit2D hit;
    private Vector2 moveDirection;

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
        startPosition = transform.position;
    }
    
 
    public void Update() {
        if (!isMoving) {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (diagonalMovement)
            {
                if (input.x > 0) input.x = 1;
                if (input.x < 0) input.x = -1;
                if (input.y > 0) input.y = 1;
                if (input.y < 0) input.y = -1;
            }
            else
            {
                if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                {
                    input.y = 0;
                }
                else
                {
                    input.x = 0;
                }
            }
            

            if (input != Vector2.zero)
            {
                factor = 1f;
                if (Mathf.Abs(input.x) + Mathf.Abs(input.y) == 2) factor = 0.7071f;
                hit = Physics2D.Raycast(transform.position, new Vector2(input.x, input.y), playerSize/factor);
                if (logic.MoveBegin(hit.collider))
                {
                    StartCoroutine(move(transform));
                }
                else if (factor == 0.7071f)
                {
                    factor = 1f;
                    hit = Physics2D.Raycast(transform.position, new Vector2(input.x, 0), playerSize/factor);
                    if (logic.MoveBegin(hit.collider))
                    {
                        input.y = 0;
                        StartCoroutine(move(transform));
                    }
                    else
                    {
                        hit = Physics2D.Raycast(transform.position, new Vector2(0, input.y), playerSize/factor);
                        if (logic.MoveBegin(hit.collider))
                        {
                            input.x = 0;
                            StartCoroutine(move(transform));
                        }
                    }
                }
            }
            
        }
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
