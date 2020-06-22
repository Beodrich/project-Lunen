﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIScripts
{
    public enum AILevel
    {
        Random,
        Weak,
        Decent,
        Strong,
        Legendary
    }

    public static AIDecision StartDecision(SetupRouter sr, Monster monster)
    {
        AIDecision decision = new AIDecision(0,0);

        switch(monster.level)
        {
            default:
            break;
            case AILevel.Random:
                int movePool = 4;
                if (monster.ActionSet.Count < movePool) movePool = monster.ActionSet.Count;
                decision.moveIndex = GetRandomValue(movePool);
                decision.targetIndex = GetRandomValue(sr.director.GetLunenCountOut(Director.Team.PlayerTeam));
            break;
        }
        return decision;
    }

    public static int GetRandomValue(int max)
    {
        return Random.Range(0,max);
    }
}

public class AIDecision
{
    public int moveIndex;
    public int targetIndex;

    public AIDecision(int move, int target)
    {
        moveIndex = move;
        targetIndex = target;
    }
}