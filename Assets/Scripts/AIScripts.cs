using System.Collections;
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

    public static void StartDecision()
    {
        /*
        for (int i = 0; i < LunenOut.Count; i++)
        {
            if (LunenOut[i].CooldownDone)
            {
                AI_DecideAction(LunenOut[i], i);
            }
        }
        */
    }

    /*
    public static void AI_DecideAction(Monster monster, int index)
    {
        //Lunen has finished cooldown, AI decides what move to use on which lunen.
        switch(skillLevel)
        {
            case Director.AISkill.Wild:
            //enemy AI will choose target and move randomly
                int movePool = 4;
                if (monster.ActionSet.Count < movePool) movePool = monster.ActionSet.Count;
                int moveChosen = Random.Range(0, movePool);
                int monsterPool = sr.director.PlayerScripts[0].LunenOut.Count;
                int monsterChosen = Random.Range(0,monsterPool);
                sr.director.EnemyLunenSelect = monsterChosen;
                sr.director.PerformAction(monster.MonsterTeam, index, moveChosen);
                monster.ResetCooldown();
            break;
        }
    }
    */
}
