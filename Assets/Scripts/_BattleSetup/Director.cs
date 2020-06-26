using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Director : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public enum Team
    {
        PlayerTeam,
        EnemyTeam
    }

    public enum LunenState
    {
        All,
        Alive,
        Dead,
        Out
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

    public List<Monster> PlayerLunenMonsters;
    public List<Monster> PlayerLunenAlive;

    public List<Monster> EnemyLunenMonsters;
    public List<Monster> EnemyLunenAlive;

    public int MaxLunenOut = 3;
    public int PlayerLunenCurrentlyOut = 0;
    public int EnemyLunenCurrentlyOut = 0;
    

    [HideInInspector]
    public float DirectorDeltaTime;
    public bool DirectorTimeFlowing;
    public float DirectorTimeToWait;
    public bool DirectorGamePaused;
    public int EnemyLunenSelect;

    private void Awake()
    {
        sr = GetComponent<SetupRouter>();
    }

    private void Update()
    {
        if (DirectorTimeFlowing && !sr.battleSetup.gamePaused && !sr.battleSetup.cutsceneStoppedBattle)
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
        DirectorGamePaused = false;
        LoadTeams();
        ResetLunenCooldowns();
        sr.canvasCollection.ScanBothParties();
        sr.canvasCollection.Player2LunenTarget(0);
    }

    public void CleanUpBattle()
    {
        DirectorGamePaused = true;
        DirectorTimeFlowing = false;
        ResetLunenCooldowns();
    }

    public void ResetLunenCooldowns()
    {
        foreach (Monster m in PlayerLunenMonsters) m.ResetCooldown();
        foreach (Monster m in EnemyLunenMonsters) m.ResetCooldown();
    }

    public void LoadTeams()
    {
        PlayerLunenMonsters = LoadParty(Team.PlayerTeam);
        EnemyLunenMonsters = LoadParty(Team.EnemyTeam);

        PlayerLunenAlive = LoadPartyAlive(PlayerLunenMonsters, Team.PlayerTeam);
        EnemyLunenAlive = LoadPartyAlive(EnemyLunenMonsters, Team.EnemyTeam);
    }

    public List<Monster> LoadParty(Team team)
    {
        List<GameObject> LunenTeamObjects;
        List<Monster> LunenTeamMonsters = new List<Monster>();

        if (team == Team.PlayerTeam) LunenTeamObjects = sr.battleSetup.PlayerLunenTeam;
        else if (team == Team.EnemyTeam) LunenTeamObjects = sr.battleSetup.EnemyLunenTeam;
        else LunenTeamObjects = sr.battleSetup.EnemyLunenTeam;

        for (int i = 0; i < LunenTeamObjects.Count; i++)
        {
            GameObject go = LunenTeamObjects[i];
            LunenTeamMonsters.Add(go.GetComponent<Monster>());
            go.GetComponent<Monster>().MonsterTeam = team;
            go.GetComponent<Monster>().LunenOrder = i;
        }

        return LunenTeamMonsters;
    }

    public List<Monster> LoadPartyAlive(List<Monster> source, Team team)
    {
        if (team == Team.PlayerTeam) PlayerLunenCurrentlyOut = 0;
        if (team == Team.EnemyTeam) EnemyLunenCurrentlyOut = 0;

        List<Monster> AliveLunen = new List<Monster>();

        foreach (Monster m in source)
        {
            if (m.Health.z > 0)
            {
                AliveLunen.Add(m);
                if (team == Team.PlayerTeam && PlayerLunenCurrentlyOut < MaxLunenOut)
                {
                    PlayerLunenCurrentlyOut++;
                    m.LunenOut = true;
                }
                if (team == Team.EnemyTeam && EnemyLunenCurrentlyOut < MaxLunenOut)
                {
                    EnemyLunenCurrentlyOut++;
                    m.LunenOut = true;
                }
            }
        }

        return AliveLunen;
    }

    public void LunenHasDied(Monster lunen)
    {
        lunen.LunenOut = false;
        switch (lunen.MonsterTeam)
        {
            case Team.PlayerTeam:
                PlayerLunenAlive = LoadPartyAlive(PlayerLunenMonsters, Team.PlayerTeam);
                if (PlayerLunenAlive.Count == 0) sr.battleSetup.PlayerLose();
                break;
            case Team.EnemyTeam:
                
                for (int i = 0; i < MaxLunenOut; i++)
                {
                    if (PlayerLunenAlive.Count > i)
                    {
                        PlayerLunenAlive[i].GetExp(CalculateExpPayout(lunen, PlayerLunenAlive[i]));
                    }
                }
                sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Enemy Lunen Defeated"));
                EnemyLunenAlive = LoadPartyAlive(EnemyLunenMonsters, Team.EnemyTeam);
                break;
        }
        sr.canvasCollection.ScanBothParties();
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves1", UITransition.State.Disable);
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves2", UITransition.State.Disable);
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves3", UITransition.State.Disable);
    }

    public void AttemptToCapture()
    {
        //TODO: Add chance to capture
        if (true)
        {
            Monster monsterToCapture = GetMonsterOut(Team.EnemyTeam, sr.canvasCollection.GetLunenSelected(Team.EnemyTeam));
            GameObject monsterToCaptureObject= monsterToCapture.gameObject;
            monsterToCapture.MonsterTeam = Team.PlayerTeam;
            sr.battleSetup.PlayerLunenTeam.Add(monsterToCaptureObject);
            sr.battleSetup.EnemyLunenTeam.RemoveAt(sr.canvasCollection.GetLunenSelected(Team.EnemyTeam));
            sr.battleSetup.PlayerWin();
        }
    }

    public int CalculateExpPayout(Monster deadLunen, Monster lunenGettingEXP)
    {
        double exactPayout = 1;

        //P = Place of battle; Wild battle = 1, Trainer = 1.5
        double P = 1;
        if (sr.battleSetup.typeOfBattle == BattleSetup.BattleType.TrainerBattle) P = 1.5;

        //C = Defeated Lunen’s Affinity Cost
        //double C = deadLunen.SourceLunen.AffinityCost + deadLunen.MoveAffinityCost;
        double C = 10;

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

    public Monster GetMonster(Team lunenTeam, LunenState state, int index)
    {
        if (lunenTeam == Team.PlayerTeam)
        {
            switch (state)
            {
                default:
                    return null;
                case LunenState.Alive:
                case LunenState.Out:
                    if (PlayerLunenAlive.Count > index)
                        return PlayerLunenAlive[index];
                    else
                        return null;
                case LunenState.All:
                    if (PlayerLunenMonsters.Count > index)
                        return PlayerLunenMonsters[index];
                    else
                        return null;

            }
        }
        else if (lunenTeam == Team.EnemyTeam)
        {
            switch (state)
            {
                default:
                    return null;
                case LunenState.Alive:
                case LunenState.Out:
                    if (EnemyLunenAlive.Count > index)
                        return EnemyLunenAlive[index];
                    else
                        return null;
                case LunenState.All:
                    if (EnemyLunenMonsters.Count > index)
                        return EnemyLunenMonsters[index];
                    else
                        return null;

            }
        }
        else return null;
    }

    public Monster GetMonsterOut(Team lunenTeam, int index)
    {
        return GetMonster(lunenTeam, LunenState.Out, index);
    }

    public int GetLunenCount(Team lunenTeam, LunenState state)
    {
        if (lunenTeam == Team.PlayerTeam)
        {
            switch (state)
            {
                default: return 0;
                case LunenState.Alive: return PlayerLunenAlive.Count;
                case LunenState.Out: return (PlayerLunenAlive.Count > MaxLunenOut ? MaxLunenOut : PlayerLunenAlive.Count);
                case LunenState.All: return PlayerLunenMonsters.Count;
                case LunenState.Dead: return (PlayerLunenMonsters.Count - PlayerLunenAlive.Count);
            }
        }
        else if (lunenTeam == Team.EnemyTeam)
        {
            switch (state)
            {
                default: return 0;
                case LunenState.Alive: return EnemyLunenAlive.Count;
                case LunenState.Out: return (EnemyLunenAlive.Count > MaxLunenOut ? MaxLunenOut : EnemyLunenAlive.Count);
                case LunenState.All: return EnemyLunenMonsters.Count;
                case LunenState.Dead: return (EnemyLunenMonsters.Count - EnemyLunenAlive.Count);
            }
        }
        else return 0;
    }

    public int GetLunenCountOut(Team lunenTeam)
    {
        return GetLunenCount(lunenTeam, LunenState.Out);
    }
}
