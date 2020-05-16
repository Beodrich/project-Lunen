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

        for (int i = 0; i < 3; i++)
        {
            if (Player1Script.LunenInParty.Count > i)
            {
                Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
                Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = Player1Script.LunenInParty[i].GetComponent<Monster>().Nickname;
                Player1LunenButtons[i].SetActive(true);
                LunenPanels[i] = DescriptionPanels[i].GetComponent<LunenActionPanel>();
                LunenPanels[i].FindScripts();
                for (int j = 0; j < 4; j++)
                {
                    if (j >= Player1Script.LunenInParty[i].GetComponent<Monster>().ActionSet.Count)
                    {
                        LunenPanels[i].ActionButtons[j].SetActive(false);
                    }
                    else
                    {
                        LunenPanels[i].ActionButtonScripts[j].Name.GetComponent<Text>().text = Player1Script.LunenInParty[i].GetComponent<Monster>().ActionSet[j].GetComponent<Action>().Name;
                        LunenPanels[i].ActionButtonScripts[j].Type.GetComponent<Text>().text = Types.GetTypeString(Player1Script.LunenInParty[i].GetComponent<Monster>().ActionSet[j].GetComponent<Action>().Type);
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

        for (int i = 0; i < 3; i++)
        {
            if (Player2Script.LunenInParty.Count > i)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = Player2Script.LunenInParty[i].GetComponent<Monster>().Nickname;
                Player2LunenButtons[i].SetActive(true);
            }
            else Player2LunenButtons[i].SetActive(false);
        }
    }

    public void PerformAction(int index)
    {
        Action action = Player1Script.LunenInParty[MenuOpen - 1].GetComponent<Monster>().ActionSet[index].GetComponent<Action>();
        action.MonsterUser = Player1Script.LunenInParty[MenuOpen - 1].GetComponent<Monster>();
        action.MonsterTarget = Player2Script.LunenInParty[EnemyTarget].GetComponent<Monster>();
        action.Execute();
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
            if (MenuOpen != 0)
            {
                DescriptionPanels[MenuOpen - 1].SetActive(false);
            }
            DescriptionPanels[index - 1].SetActive(true);
            MenuOpen = index;
        }
    }

    public void Player2LunenTarget(int index)
    {
        EnemyTarget = index;
    }
}
