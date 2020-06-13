using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;

public class TrainerLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public TrainerEncounter te;

    public LunaDex.Direction trainerDirection;
    public bool limitRange;

    [ConditionalField(nameof(limitRange))] public int rangeLimit;

    [HideInInspector] public SpriteRenderer sprite;

    [NamedArray(typeof(LunaDex.Direction))] public List<Sprite> facingDirections;

    Vector3 Size;
    Vector3 StartPosition;

    private int maxSeeDistance = 30;
    // Start is called before the first frame update
    void Start()
    {
        GetImportantVariables();
    }
    
    void GetImportantVariables()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        te = GetComponent<TrainerEncounter>();
    }

    void Update()
    {
        TrainerScan();
    }

    private void OnDrawGizmosSelected()
    {
        GetImportantVariables();
        TrainerScan();
        
        Gizmos.DrawCube(StartPosition, Size);
    }

    private void TrainerScan()
    {
        sprite.sprite = facingDirections[(int)trainerDirection];

        float vertCorrect = 0;
        float horiCorrect = 0;

        if (trainerDirection == LunaDex.Direction.South) vertCorrect = 1;
        if (trainerDirection == LunaDex.Direction.West) horiCorrect = 1;
        
        Vector3 centerPosition = new Vector3(transform.position.x +horiCorrect*2+ 0.5f, transform.position.y+vertCorrect*2 - 0.5f, transform.position.z);
        Vector2 input = sr.lunaDex.GetDirectionVector2(trainerDirection);
        Vector2 checkPoint1 = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
        Vector2 checkPoint2 = new Vector2(centerPosition.x+input.x, centerPosition.y+input.y);
        
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        bool foundWall = false;
        int maxRange = maxSeeDistance;
        if (limitRange) maxRange = rangeLimit+(int)vertCorrect*2+(int)horiCorrect*2;
        int foundRange = -1;
        while (!foundWall && foundRange < maxRange)
        {
            foundRange++;
            Collider2D hit = Physics2D.OverlapArea(checkPoint1+input*foundRange,checkPoint2+input*foundRange);
            foundWall = CheckForWall(hit);
            
        }
        foundRange--;
        Size = new Vector3(input.x*foundRange+1, input.y*foundRange+1, 0);
        StartPosition = new Vector3(transform.position.x+horiCorrect+input.x+Size.x/2, transform.position.y+(-1)+vertCorrect+input.y+Size.y/2, transform.position.z);
    }

    private bool CheckForWall(Collider2D hit)
    {
        if (hit != null && hit.gameObject != this.gameObject)
        {
            switch(hit.gameObject.tag)
            {
                default: return false;
                case "Wall": return true;
                case "Player": 
                if (EditorApplication.isPlaying && !sr.battleSetup.InBattle)
                {
                    te.ClearTeamOfNull();
                    if (!te.defeated)
                    {
                        sr.battleSetup.GenerateTrainerBattle(te);
                        sr.battleSetup.MoveToBattle(0,0);
                    }
                }
                return false;
            }
        }
        else return false;
        
    }

    
}