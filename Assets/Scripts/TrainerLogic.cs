using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MyBox;

public class TrainerLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public Move move;

    public MoveScripts.Direction startLookDirection;
    public bool limitRange;

    [ConditionalField(nameof(limitRange))] public int rangeLimit;

    Vector3 Size;
    Vector3 StartPosition;

    private int maxSeeDistance = 30;

    public List<GameObject> TeamObjects;
    public List<Monster> Team;

    [Header("Trainer Lunen Info")]

    public bool engaged;
    public bool defeated;
    // Start is called before the first frame update
    void Start()
    {
        GetImportantVariables();
        SetMoveLook(startLookDirection);
    }

    void SetMoveLook(MoveScripts.Direction direction)
    {
        move.SetFacingDirectionLogic(direction);
    }
    
    void GetImportantVariables()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        if (move == null) move = GetComponent<Move>();

        UpdateTeam();
    }

    void UpdateTeam()
    {
        TeamObjects.Clear();
        Team.Clear();
        TeamObjects.AddRange(gameObject.transform.Cast<Transform>().Where(c => c.gameObject.tag == "Monster").Select(c => c.gameObject).ToArray());
        for (int i = 0; i < TeamObjects.Count; i++)
        {
            Team.Add(TeamObjects[i].GetComponent<Monster>());
        }
    }

    void Update()
    {
        TrainerScan();
    }

    private void OnDrawGizmosSelected()
    {
        GetImportantVariables();
        SetMoveLook(startLookDirection);
        TrainerScan();
        
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(StartPosition, Size); 
    }

    private void TrainerScan()
    {
        if (!defeated)
        {
            Vector2 checkVector;
            Vector2 size2;
            Vector2 directionVector;
            bool foundWall = false;
            bool foundPlayer = false;
            int maxRange = maxSeeDistance;
            if (limitRange) maxRange = rangeLimit+1;
            int foundRange = -1;
            while (!foundWall && foundRange < maxRange)
            {
                foundRange++;
                checkVector = MoveScripts.GetFrontVector2(move, (float)foundRange, true);
                Collider2D hit = Physics2D.OverlapArea(checkVector, checkVector);
                foundWall = MoveScripts.CheckForTag(this.gameObject,hit,"Wall");
                foundPlayer = MoveScripts.CheckForTag(this.gameObject,hit,"Player");
                if (foundPlayer) sr.battleSetup.StartCutscene(GetComponent<Cutscene>());
            }
            foundRange--;
            checkVector = MoveScripts.GetFrontVector2(move, (float)foundRange/2, true);
            size2 = MoveScripts.GetVector2FromDirection(move.lookDirection) * foundRange;
            directionVector = MoveScripts.GetVector2FromDirection(move.lookDirection) * 0.5f;
            if (size2.x == 0) size2.x = 1;
            if (size2.y == 0) size2.y = 1;
            Size = new Vector3(size2.x, size2.y, 1);
            StartPosition = new Vector3(checkVector.x + directionVector.x, checkVector.y + directionVector.y, -1);
        }
        
    }

    public void StartTrainerBattle()
    {
        if (EditorApplication.isPlaying && !sr.battleSetup.InBattle)
        {
            ClearTeamOfNull();
            if (!defeated && !engaged)
            {
                engaged = true;
                sr.battleSetup.GenerateTrainerBattle(this);
                sr.battleSetup.MoveToBattle(0,0);
                
            }
        }
    }

    public void ExitBattle(bool defeat)
    {
        sr.eventLog.AddEvent("Setting defeated to: " + defeat);
        defeated = defeat;
        engaged = false;
    }

    public void MoveEnd()
    {

    }
    public void ClearTeamOfNull()
    {
        for (int i = 0; i < TeamObjects.Count; i++)
        {
            if (TeamObjects[i] == null)
            {
                TeamObjects.RemoveAt(i);
                i--;
            }
        }
    }
    
}