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

    public AnimationSet animationSet;

    public MoveScripts.Direction startLookDirection;
    public bool limitRange;

    [ConditionalField(nameof(limitRange))] public int rangeLimit;

    [Space(10)]

    public List<GenerateMonster.LunenSetup> TeamComp;

    Vector3 Size;
    Vector3 StartPosition;

    private int maxSeeDistance = 30;

    [HideInInspector] public List<GameObject> TeamObjects;
    [HideInInspector] public List<Monster> Team;
    public List<string> TrainerLookStop;

    [Header("Trainer Lunen Info")]

    public bool overrideDefeated;
    public bool engaged;
    public bool defeated;
    // Start is called before the first frame update
    void Start()
    {
        GetImportantVariables();
        GetDefeated();
        CreateTeam();
        SetMoveLook(startLookDirection);
    }

    void CreateTeam()
    {
        for (int i = 0; i < TeamComp.Count; i++)
        {
            GameObject newMonsterObject = sr.generateMonster.GenerateLunen(TeamComp[i].species, TeamComp[i].level);
            TeamObjects.Add(newMonsterObject);
            Team.Add(newMonsterObject.GetComponent<Monster>());
            newMonsterObject.transform.SetParent(this.transform);
        }
    }

    void SetMoveLook(MoveScripts.Direction direction)
    {
        move.SetFacingDirectionLogic(direction);
    }
    
    void GetImportantVariables()
    {
        GameObject main = GameObject.Find("BattleSetup");
        if (main != null)
        {
            sr = main.GetComponent<SetupRouter>();
            if (move == null) move = GetComponent<Move>();

            move.animationSet = animationSet;

            UpdateTeam();
        }
        
        
    }

    void GetDefeated()
    {
        if (sr != null && !overrideDefeated) defeated = sr.battleSetup.GuidInList(GetComponent<GuidComponent>().GetGuid());
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
                Collider2D[] hit = Physics2D.OverlapAreaAll(checkVector, checkVector);
                foundWall = MoveScripts.CheckForTag(this.gameObject,hit,TrainerLookStop);
                if (foundRange < maxRange)
                {
                    foundPlayer = MoveScripts.CheckForTag(this.gameObject,hit,"Player");
                    if (foundPlayer && !sr.saveSystemObject.isLoading && !sr.battleSetup.playerDead && !engaged) sr.battleSetup.StartCutscene(new PackedCutscene(GetComponent<Cutscene>()));
                }
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
        #if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
        #endif
        if (!sr.battleSetup.InBattle)
        {
            ClearTeamOfNull();
            if (!defeated && !engaged)
            {
                engaged = true;
                sr.canvasCollection.Player2BattleFieldSprites[0].SetAnimationSet(animationSet);
                sr.battleSetup.GenerateTrainerBattle(this);
                sr.battleSetup.EnterBattle();
                
            }
        }
    }

    public void ExitBattle(bool defeat)
    {
        sr.eventLog.AddEvent("Setting defeated to: " + defeat);
        defeated = defeat;
        engaged = false;
        if (defeated) sr.battleSetup.GuidList.Add(GetComponent<GuidComponent>().GetGuid());
    }

    public bool MoveBegin(Collider2D hit)
    {
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
                case "Player": return false;
                case "Creature": return false;
                case "Trainer": return false;
                case "Thing": return false;
                case "NPC": return false;
                case "TrainerSight":
                    return true;
                case "Door":
                    return true;
            }
        }
    }

    public bool MoveBegin(Collider2D[] hit)
    {
        int pathsFound = 0;
        if (hit.Length > 0)
        {
            int found = 0;
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].gameObject.tag != "Path" && hit[i].gameObject.tag != "Grass")
                {
                    found += MoveBegin(hit[i]) ? 1 : 0;
                }
                else
                {
                    pathsFound++;
                }
                
            }
            if (pathsFound == hit.Length) return true;
            return (found > 0);
        }
        else return false;
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

    public void MoveEnd()
    {

    }
    
}