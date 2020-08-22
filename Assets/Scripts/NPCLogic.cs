using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class NPCLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public Move move;

    public AnimationSet animationSet;
    public MoveScripts.Direction lookDirection;

    public bool wanders;
    [ConditionalField(nameof(wanders))] public float wanderLow;
    [ConditionalField(nameof(wanders))] public float wanderHigh;
    [ConditionalField(nameof(wanders))] public float wanderCurrent;

    void Start()
    {
        GetImportantVariables();
        move.lookDirection = lookDirection;
    }
    
    void GetImportantVariables()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        if (move == null) move = GetComponent<Move>();

        move.animationSet = animationSet;
        wanderCurrent = SetNewWaitTime(wanderLow, wanderHigh);
    }

    private void Update()
    {
        if (wanders)
        {
            if (!sr.battleSetup.cutsceneLoopGoing && !sr.battleSetup.gamePaused)
            {
                wanderCurrent -= Time.deltaTime;
                if (wanderCurrent < 0)
                {

                    RandomWalk();
                    wanderCurrent = SetNewWaitTime(wanderLow, wanderHigh);
                }
            }
        }
    }

    private void RandomWalk()
    {
        int direction = Random.Range(0,4);
        MoveScripts.Direction nextDirection = (MoveScripts.Direction)direction;
        move.NPCmove(nextDirection);
        
    }

    private float SetNewWaitTime(float low, float high)
    {
        return Random.Range(low, high);
    }
}
