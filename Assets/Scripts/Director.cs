using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;
    public List<GameObject> DescriptionPanels;
    public List<GameObject> Player1LunenButtons;
    public List<GameObject> Player2LunenButtons;
    public List<LunenButton> Player1LunenButtonScripts;
    public List<LunenButton> Player2LunenButtonScripts;
    public List<LunenActionPanel> LunenPanels;

    Player Player1Script;
    Player Player2Script;

    [HideInInspector]
    public GameObject battleSetupObject;
    [HideInInspector]
    public BattleSetup battleSetup;

    public int MenuOpen = 0;
    public int EnemyTarget = 0;

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
    }

    public void ScanBothParties()
    {
        ScanPlayer1Party();
        ScanPlayer2Party();
    }

    public void ScanPlayer1Party()
    {
        
        Player1Script.ReloadTeam();

        for (int i = 0; i < 3; i++)
        {
            if (Player1Script.LunenOut.Count > i)
            {
                Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
                Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = Player1Script.LunenOut[i].Nickname;
                Player1LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + Player1Script.LunenOut[i].Level;
                Player1LunenButtons[i].SetActive(true);
                Player1Script.LunenOut[i].loopback = this;
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

        for (int i = 0; i < 3; i++)
        {
            if (Player2Script.LunenOut.Count > i)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = Player2Script.LunenOut[i].Nickname;
                Player2LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + Player2Script.LunenOut[i].Level;
                Player2LunenButtons[i].SetActive(true);
                Player2Script.LunenOut[i].loopback = this;
                AssignPlayer2Bars(i);
            }
            else Player2LunenButtons[i].SetActive(false);
        }

        if (Player2Script.LunenAlive == 0)
        {
            battleSetup.MoveToOverworld(1);
        }
    }

    public void PerformAction(int index)
    {
        Player1Script.ReloadTeam();
        Player2Script.ReloadTeam();
        Action action = Player1Script.LunenOut[MenuOpen - 1].ActionSet[index].GetComponent<Action>();
        action.MonsterUser = Player1Script.LunenOut[MenuOpen - 1];
        action.MonsterTarget = Player2Script.LunenOut[EnemyTarget];
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

    private void Update()
    {
        
    }

    public void ScanLunen(Monster scan, int index)
    {

    }

    public void Player1MenuClick(int index)
    {
        if (MenuOpen == index)
        {
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
                    }
                    DescriptionPanels[index - 1].SetActive(true);
                    MenuOpen = index;
                }
            }
            else
            {
                if (MenuOpen != 0)
                {
                    DescriptionPanels[MenuOpen - 1].SetActive(false);
                }
                DescriptionPanels[index - 1].SetActive(true);
                MenuOpen = index;
            }
            
        }
    }

    public void Player2LunenTarget(int index)
    {
        EnemyTarget = index;
    }
}
