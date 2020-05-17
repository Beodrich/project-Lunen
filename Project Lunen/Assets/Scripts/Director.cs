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

    public int MenuOpen = 0;
    public int EnemyTarget = 0;

    private void Start()
    {
        if (Player1 != null)
        {
            ScanPlayer1Party();
        }
        if (Player2 != null)
        {
            ScanPlayer2Party();
        }
    }

    public void ScanPlayer1Party()
    {
        Player1Script = Player1.GetComponent<Player>();
        Player1Script.ReReferenceMonsters();

        for (int i = 0; i < 3; i++)
        {
            if (Player1Script.LunenInParty.Count > i)
            {
                Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
                Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = Player1Script.MonstersInParty[i].Nickname;
                Player1LunenButtons[i].SetActive(true);
                AssignPlayer1Bars(i);
                LunenPanels[i] = DescriptionPanels[i].GetComponent<LunenActionPanel>();
                LunenPanels[i].FindScripts();
                for (int j = 0; j < 4; j++)
                {
                    if (j >= Player1Script.MonstersInParty[i].ActionSet.Count)
                    {
                        LunenPanels[i].ActionButtons[j].SetActive(false);
                    }
                    else
                    {
                        LunenPanels[i].ActionButtonScripts[j].Name.GetComponent<Text>().text = Player1Script.MonstersInParty[i].ActionSet[j].GetComponent<Action>().Name;
                        LunenPanels[i].ActionButtonScripts[j].Type.GetComponent<Text>().text = Types.GetTypeString(Player1Script.MonstersInParty[i].ActionSet[j].GetComponent<Action>().Type);
                        Player1Script.MonstersInParty[i].loopback = this;
                        
                    }
                }
                //LunenPanels[i].ActionButtonScripts
            }
            else Player1LunenButtons[i].SetActive(false);
        }
    }

    public void ScanPlayer2Party()
    {
        Player2Script = Player2.GetComponent<Player>();
        Player2Script.ReReferenceMonsters();

        for (int i = 0; i < 3; i++)
        {
            if (Player2Script.LunenInParty.Count > i)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = Player2Script.MonstersInParty[i].Nickname;
                Player2LunenButtons[i].SetActive(true);
                AssignPlayer2Bars(i);
            }
            else Player2LunenButtons[i].SetActive(false);
        }
    }

    public void PerformAction(int index)
    {
        Player1Script.ReReferenceMonsters();
        Player2Script.ReReferenceMonsters();
        Action action = Player1Script.MonstersInParty[MenuOpen - 1].ActionSet[index].GetComponent<Action>();
        action.MonsterUser = Player1Script.MonstersInParty[MenuOpen - 1];
        action.MonsterTarget = Player2Script.MonstersInParty[EnemyTarget];
        action.Execute();
    }

    public void AssignPlayer1Bars(int index)
    {
        Player1LunenButtonScripts[index].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = Player1Script.MonstersInParty[index];
        Player1LunenButtonScripts[index].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = Player1Script.MonstersInParty[index];
    }

    public void AssignPlayer2Bars(int index)
    {
        Player2LunenButtonScripts[index].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = Player2Script.MonstersInParty[index];
        Player2LunenButtonScripts[index].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = Player2Script.MonstersInParty[index];
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
                if (Player1Script.MonstersInParty[index - 1].CurrCooldown <= 0f)
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
