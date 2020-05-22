using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{
    public enum Team
    {
        PlayerTeam,
        EnemyTeam
    }

    public GameObject Player1;
    public GameObject Player2;
    public List<GameObject> DescriptionPanels;
    public List<GameObject> Player1LunenButtons;
    public List<GameObject> Player2LunenButtons;
    public List<LunenButton> Player1LunenButtonScripts;
    public List<LunenButton> Player2LunenButtonScripts;
    public List<LunenActionPanel> LunenPanels;

    [HideInInspector] public Player Player1Script;
    [HideInInspector] public Player Player2Script;

    [HideInInspector]
    public GameObject battleSetupObject;
    [HideInInspector]
    public BattleSetup battleSetup;

    public int MaxLunenOut = 3;
    public int MenuOpen = 0;
    public int EnemyTarget = 0;

    [HideInInspector]
    public float DirectorDeltaTime;
    public bool DirectorTimeFlowing;
    public float DirectorTimeToWait;
    public bool DirectorGamePaused;

    private void Start()
    {
        Player1Script = Player1.GetComponent<Player>();
        Player2Script = Player2.GetComponent<Player>();

        battleSetupObject = GameObject.Find("BattleSetup");
        if (battleSetupObject != null)
        {
            battleSetup = battleSetupObject.GetComponent<BattleSetup>();
            Player1Script.LunenTeam = battleSetup.PlayerLunenTeam;
            Player2Script.LunenTeam = battleSetup.EnemyLunenTeam;
        }

        ScanPlayer1Party();
        ScanPlayer2Party();
        Player2LunenTarget(0);
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

    public void ScanBothParties()
    {
        ScanPlayer1Party();
        ScanPlayer2Party();
    }

    public void ScanPlayer1Party()
    {
        
        Player1Script.ReloadTeam();

        for (int i = 0; i < MaxLunenOut; i++)
        {
            if (Player1Script.LunenOut.Count > i)
            {
                Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
                Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = Player1Script.LunenOut[i].Nickname;
                Player1LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + Player1Script.LunenOut[i].Level;
                Player1LunenButtons[i].SetActive(true);
                Player1Script.LunenOut[i].loopback = this;
                Player1Script.LunenOut[i].MonsterTeam = Team.PlayerTeam;
                AssignPlayer1Bars(i);
                LunenPanels[i] = DescriptionPanels[i].GetComponent<LunenActionPanel>();
                LunenPanels[i].FindScripts();
                for (int j = 0; j < 4; j++)
                {
                    if (j >= Player1Script.LunenOut[i].ActionSet.Count)
                    {
                        LunenPanels[i].ActionButtons[j].SetActive(false);
                    }
                    else
                    {
                        LunenPanels[i].ActionButtonScripts[j].Name.GetComponent<Text>().text = Player1Script.LunenOut[i].ActionSet[j].GetComponent<Action>().Name;
                        LunenPanels[i].ActionButtonScripts[j].Type.GetComponent<Text>().text = Types.GetTypeString(Player1Script.LunenOut[i].ActionSet[j].GetComponent<Action>().Type);
                        
                        
                    }
                }
                //LunenPanels[i].ActionButtonScripts
            }
            else Player1LunenButtons[i].SetActive(false);
        }
    }

    public void ScanPlayer2Party()
    {
        
        Player2Script.ReloadTeam();

        for (int i = 0; i < MaxLunenOut; i++)
        {
            if (Player2Script.LunenOut.Count > i)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = Player2Script.LunenOut[i].Nickname;
                Player2LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + Player2Script.LunenOut[i].Level;
                Player2LunenButtons[i].SetActive(true);
                Player2Script.LunenOut[i].loopback = this;
                Player2Script.LunenOut[i].MonsterTeam = Team.EnemyTeam;
                AssignPlayer2Bars(i);
            }
            else Player2LunenButtons[i].SetActive(false);
        }

        if (Player2Script.LunenAlive == 0)
        {
            battleSetup.MoveToOverworld();
        }
    }

    public void PerformAction(int index)
    {
        Player1Script.ReloadTeam();
        Player2Script.ReloadTeam();
        Action action = Player1Script.LunenOut[MenuOpen - 1].ActionSet[index].GetComponent<Action>();
        action.MonsterUser = Player1Script.LunenOut[MenuOpen - 1];
        while (Player2Script.LunenOut.Count <= EnemyTarget) EnemyTarget--;
        //action.MonsterTarget = Player2Script.LunenOut[EnemyTarget];
        action.Execute();
    }

    public void AssignPlayer1Bars(int index)
    {
        Player1LunenButtonScripts[index].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = Player1Script.LunenOut[index];
        Player1LunenButtonScripts[index].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = Player1Script.LunenOut[index];
    }

    public void AssignPlayer2Bars(int index)
    {
        Player2LunenButtonScripts[index].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = Player2Script.LunenOut[index];
        Player2LunenButtonScripts[index].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = Player2Script.LunenOut[index];
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
                    if (Player1Script.LunenOut.Count > i)
                    {
                        Player1Script.LunenOut[i].GetExp(CalculateExpPayout(lunen, Player1Script.LunenOut[i]));
                    }
                }
                break;
        }
        ScanBothParties();
    }

    public void AttemptToCapture(GameObject lunenToCapture, GameObject captureDevice)
    {
        //TODO: 
    }

    public int CalculateExpPayout(Monster deadLunen, Monster lunenGettingEXP)
    {
        double exactPayout = 1;

        //P = Place of battle; Wild battle = 1, Trainer = 1.5
        double P = 1;
        if (battleSetup.typeOfBattle == BattleSetup.BattleType.TrainerBattle) P = 1.5;

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

    public void Player1MenuClick(int index)
    {
        if (MenuOpen == index)
        {
            if (MenuOpen < 4)
            {
                Player1LunenButtonScripts[MenuOpen - 1].isSelected = false;
            }
            DescriptionPanels[index - 1].SetActive(false);
            MenuOpen = 0;
        }
        else
        {
            if (index < 4)
            {
                if (Player1Script.LunenOut[index - 1].CurrCooldown <= 0f)
                {
                    if (MenuOpen != 0)
                    {
                        DescriptionPanels[MenuOpen - 1].SetActive(false);
                        Player1LunenButtonScripts[MenuOpen - 1].isSelected = false;
                    }
                    DescriptionPanels[index - 1].SetActive(true);
                    Player1LunenButtonScripts[index - 1].isSelected = true;
                    MenuOpen = index;
                }
            }
            else if (index == 6)
            {
                battleSetup.MoveToOverworld();
            }
            else
            {
                if (MenuOpen != 0)
                {
                    DescriptionPanels[MenuOpen - 1].SetActive(false);
                    if (MenuOpen < 4)
                    {
                        Player1LunenButtonScripts[MenuOpen - 1].isSelected = false;
                    }
                }
                DescriptionPanels[index - 1].SetActive(true);
                MenuOpen = index;
            }
            
        }
    }

    public void Player2LunenTarget(int index)
    {
        Player2LunenButtonScripts[EnemyTarget].isSelected = false;
        Player2LunenButtonScripts[index].isSelected = true;
        EnemyTarget = index;
    }
}
