using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Director : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    [System.Serializable]
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

    [HideInInspector] public int MaxLunenPossible = 3;
    public int MaxLunenOut = 2;
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
        sr.database.SetTriggerValue("BattleVars/LunenAttacking", false);
        LoadTeams();
        ResetLunenCooldowns();
        ResetLunenMoveCooldowns();
        sr.canvasCollection.ScanBothParties();
        sr.canvasCollection.Player2LunenTarget(0);
    }

    public void CleanUpBattle()
    {
        DirectorGamePaused = true;
        DirectorTimeFlowing = false;
        ResetLunenCooldowns();
        ResetLunenEffects();
    }

    public void ResetLunenMoveCooldowns()
    {
        foreach (Monster m in PlayerLunenMonsters) m.ResetMoveCooldown();
        foreach (Monster m in EnemyLunenMonsters) m.ResetMoveCooldown();
    }

    public void ResetLunenCooldowns()
    {
        foreach (Monster m in PlayerLunenMonsters) m.ResetCooldown();
        foreach (Monster m in EnemyLunenMonsters) m.ResetCooldown();
    }

    public void ResetLunenEffects()
    {
        foreach (Monster m in PlayerLunenMonsters) m.RemoveEffects();
        foreach (Monster m in EnemyLunenMonsters) m.RemoveEffects();
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
                    m.LunenOrder = PlayerLunenCurrentlyOut;
                    PlayerLunenCurrentlyOut++;
                    m.LunenOut = true;
                }
                if (team == Team.EnemyTeam && EnemyLunenCurrentlyOut < MaxLunenOut)
                {
                    m.LunenOrder = EnemyLunenCurrentlyOut;
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
        sr.database.SetTriggerValue("BattleVars/DeadLunen", lunen.Nickname);

        switch (lunen.MonsterTeam)
        {
            case Team.PlayerTeam:
                sr.database.SetTriggerValue("BattleVars/DeadLunenYours", true);
                sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Lunen Defeated"));
                PlayerLunenAlive = LoadPartyAlive(PlayerLunenMonsters, Team.PlayerTeam);
                sr.database.SetTriggerValue("BattleVars/PlayerLunenLeft", PlayerLunenAlive.Count);
                break;
            case Team.EnemyTeam:
                
                for (int i = 0; i < MaxLunenOut; i++)
                {
                    if (PlayerLunenAlive.Count > i)
                    {
                        PlayerLunenAlive[i].GetExp(CalculateExpPayout(lunen, PlayerLunenAlive[i]));
                    }
                }
                sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Lunen Defeated"));
                EnemyLunenAlive = LoadPartyAlive(EnemyLunenMonsters, Team.EnemyTeam);
                sr.database.SetTriggerValue("BattleVars/EnemyLunenLeft", EnemyLunenAlive.Count);
                break;
        }
        sr.canvasCollection.ScanBothParties();
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves1", UITransition.State.Disable);
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves2", UITransition.State.Disable);
        sr.canvasCollection.UIObjects[1].GetComponent<UIPanelCollection>().SetPanelState("LunenMoves3", UITransition.State.Disable);
    }

    public void AttemptToCapture(float ballModifier)
    {
        //TODO: Add chance to capture
        Monster monster = GetMonsterOut(Team.EnemyTeam, sr.canvasCollection.GetLunenSelected(Team.EnemyTeam));
        sr.battleSetup.attemptToCaptureMonster = monster;
        sr.database.SetTriggerValue("BattleVars/CaptureLunenName", monster.Nickname);

        float captureValue = GetCaptureValue(monster, ballModifier);
        List<float> captureValues = GetShakeValues(captureValue, 3);

        float catchChance = Random.Range(0,101);

        Debug.Log("Capture Value: " + captureValue);
        Debug.Log("Catch Chance: " + catchChance);

        if (catchChance <= captureValue) //Successful capture condition: Catch Chance Within Capture Value
        {
            sr.database.SetTriggerValue("BattleVars/CaptureLunenShakes", 4);
            
        }
        else
        {
            int captureWobble = Random.Range(0,4);
            sr.database.SetTriggerValue("BattleVars/CaptureLunenShakes", captureWobble);
        }
        sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Lunen Capture"));
    }

    public float GetCaptureValue(Monster monster, float ballModifier)
    {
        if (ballModifier == -1) //Basically if ball is Master Ball
        {
            return 100;
        }
        
        float value = 1;

        float monsterMaxHP = monster.GetMaxHealth();
        float monsterCurrentHP = monster.Health.z;
        float monsterCatchRate = monster.SourceLunen.CatchRate;

        //((( 3 * Max HP - 2 * HP ) * (Catch Rate * Ball Modifier ) / (3 * Max HP) ) * Status Modifier

        value = (((3*monsterMaxHP-2*monsterCurrentHP)*(monsterCatchRate*ballModifier)/(3*monsterMaxHP)));
        //TODO: Status Modifiers

        

        return value;
    }

    public List<float> GetShakeValues(float captureValue, int maxShakes)
    {
        List<float> values = new List<float>();
        for (int i = 0; i <= maxShakes; i++)
        {
            values.Add(captureValue + captureValue*((float)i/2));
        }
        values.Reverse();
        return values;
    }

    public void CaptureLunen(Monster monster)
    {
        GameObject monsterToCaptureObject = monster.gameObject;
        monster.MonsterTeam = Team.PlayerTeam;
        if (sr.battleSetup.PlayerLunenTeam.Count >= 7)
        {
            monster.Heal(monster.GetMaxHealth());
            sr.storageSystem.StoreLunen(monster);
        }
        else
        {
            sr.battleSetup.PlayerLunenTeam.Add(monsterToCaptureObject);
        }
        
        sr.battleSetup.EnemyLunenTeam.RemoveAt(sr.canvasCollection.GetLunenSelected(Team.EnemyTeam));
        sr.battleSetup.PlayerWin();
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

        Debug.Log("Exp Payout: " + exactPayout);

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

    public int CanUseMove(Monster user, Action action, int actionIndex)
    {
        /*
        1 = Can Use Move
        2 = No Type For Combo Move
        3 = Cooldown Not Finished
        */
        if (user.ActionCooldown[actionIndex] < action.Turns)
        {
            return 3;
        }
        else
        {
            if (action.ComboMove)
            {
                for (int i = 0; i < GetLunenCountOut(user.MonsterTeam); i++)
                {
                    Monster testMonster = GetMonsterOut(user.MonsterTeam, i);
                    if (testMonster != user)
                    {
                        if (testMonster.SourceLunen.HasType(action.ComboType))
                        {
                            return 1;
                        }
                    }
                }
                return 2;
            }
            else
            {
                return 1;
            }
        }
        
    }
}
