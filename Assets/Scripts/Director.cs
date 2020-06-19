using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Director : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public enum Team
    {
        PlayerTeam,
        EnemyTeam,
        EnemyTeam2,
        EnemyTeam3,
    }

    [System.Serializable]
    public enum AISkill
    {
        Wild,
        Minimum,
        Medium,
        High,
        Top
    }

    public int MaxPlayers;
    public List<GameObject> Players;
    
    [HideInInspector] public List<Player> PlayerScripts;

    public int MaxLunenOut = 3;
    

    [HideInInspector]
    public float DirectorDeltaTime;
    public bool DirectorTimeFlowing;
    public float DirectorTimeToWait;
    public bool DirectorGamePaused;
    public int EnemyLunenSelect;

    private void Awake()
    {
        sr = GetComponent<SetupRouter>();

        for (int i = 0; i < Players.Count; i++) PlayerScripts.Add(Players[i].GetComponent<Player>());
    }

    private void Update()
    {
        if (DirectorTimeFlowing)
        {
            DirectorDeltaTime = Time.deltaTime;
        }
        else
        {
            DirectorDeltaTime = 0;
            if (!DirectorGamePaused)
            {
                DirectorTimeToWait -= Time.deltaTime;
                if (DirectorTimeToWait <= 0)
                {
                    DirectorTimeFlowing = true;
                }
            }
            
        }
    }

    public void PrepareBattle()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerScripts[i].ReloadTeam();
            PlayerScripts[i].PrepareLunenForBattle();
        }
        sr.canvasCollection.ScanBothParties();
    }

    public void PerformAction(Team team, int lunen, int move)
    {
        for (int i = 0; i < PlayerScripts.Count; i++) PlayerScripts[i].ReloadTeam();
        if (team == Team.PlayerTeam)
        {
            if (PlayerScripts[0].LunenOut.Count > lunen)
            {
                if (PlayerScripts[0].LunenOut[lunen].ActionSet.Count > move)
                {
                    Action action = PlayerScripts[0].LunenOut[lunen].ActionSet[move].GetComponent<Action>();
                    action.MonsterUser = PlayerScripts[0].LunenOut[lunen];
                    while (PlayerScripts[1].LunenOut.Count <= sr.canvasCollection.GetLunenSelected(Team.EnemyTeam)) sr.canvasCollection.EnemyTarget--;
                    action.Execute();
                }
            }
            
        }
        else
        {
            Action action = PlayerScripts[1].LunenOut[lunen].ActionSet[move].GetComponent<Action>();
            action.MonsterUser = PlayerScripts[1].LunenOut[lunen];
            action.Execute();
        }
        
    }

    

    public void LunenHasDied(Monster lunen)
    {
        switch (lunen.MonsterTeam)
        {
            case Team.PlayerTeam:
                break;
            case Team.EnemyTeam:
                for (int i = 0; i < MaxLunenOut; i++)
                {
                    if (PlayerScripts[0].LunenOut.Count > i)
                    {
                        PlayerScripts[0].LunenOut[i].GetExp(CalculateExpPayout(lunen, PlayerScripts[0].LunenOut[i]));
                    }
                }
                PlayerScripts[1].ReloadTeam();
                if (PlayerScripts[1].LunenTeam.Count == 0)
                {
                    sr.battleSetup.MoveToOverworld(true);
                }
                break;
        }
        sr.canvasCollection.ScanBothParties();
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves1", UITransition.State.ImmediateDisable);
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves2", UITransition.State.ImmediateDisable);
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves3", UITransition.State.ImmediateDisable);
    }

    public void AttemptToCapture()
    {
        //TODO: Add chance to capture
        if (true)
        {
            Monster monsterToCapture = PlayerScripts[1].LunenOut[sr.canvasCollection.GetLunenSelected(Team.EnemyTeam)];
            GameObject monsterToCaptureObject= monsterToCapture.gameObject;
            monsterToCapture.MonsterTeam = Team.PlayerTeam;
            sr.battleSetup.PlayerLunenTeam.Add(monsterToCaptureObject);
            PlayerScripts[1].LunenTeam.RemoveAt(sr.canvasCollection.GetLunenSelected(Team.EnemyTeam));
            sr.canvasCollection.ScanBothParties();
        }
    }

    public int CalculateExpPayout(Monster deadLunen, Monster lunenGettingEXP)
    {
        double exactPayout = 1;

        //P = Place of battle; Wild battle = 1, Trainer = 1.5
        double P = 1;
        if (sr.battleSetup.typeOfBattle == BattleSetup.BattleType.TrainerBattle) P = 1.5;

        //C = Defeated Lunen’s Affinity Cost
        double C = deadLunen.SourceLunen.AffinityCost + deadLunen.MoveAffinityCost;

        //[TODO] G = EXP boosting item; no boost = 1, EXP Amplifier = 1.5
        double G = 1;

        //L = Level of the defeated Lunen
        double L = deadLunen.Level;

        //N = Number of non-fainted Lunen that participated in battle
        //double N = Player1Script.LunenOut.Count;
        double N = 1;

        //Tentative EXP calculation = ( P * C * G * L) / (7 * N)
        exactPayout = (P * C * G * L) / (2 * N);

        Debug.Log(exactPayout);

        int exactPayoutInt = Mathf.RoundToInt((float)exactPayout);

        if (exactPayoutInt == 0) exactPayoutInt = 1;

        return exactPayoutInt;
    }

    public Monster GetMonster(Team lunenTeam, int index)
    {
        return PlayerScripts[(int)lunenTeam].LunenOut[index];
    }
}
