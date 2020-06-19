using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasCollection : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    [System.Serializable]
    public enum UIState
    {
        Overworld,
        Battle,
        Shop,
        MainMenu,
        Options,
        Inventory,
        Lunen
    }
    [EnumNamedArray(typeof(UIState))]
    public List<GameObject> UIObjects;
    public GameObject DialogueWindow;
    public Text DialogueText;
    public UIState currentState;

    [Header("Battle Elements")]
    public List<GameObject> DescriptionPanels;
    public List<GameObject> Player1LunenButtons;
    public List<GameObject> Player2LunenButtons;
    [HideInInspector] public List<LunenButton> Player1LunenButtonScripts;
    [HideInInspector] public List<LunenButton> Player2LunenButtonScripts;
    public List<LunenActionPanel> LunenPanels;

    public GameObject Player1;
    public GameObject Player2;

    public int MenuOpen = 0;
    public int EnemyTarget = 0;

    [HideInInspector] public Player Player1Script;
    [HideInInspector] public Player Player2Script;

    [Header("Dialogue Shift")]

    public Vector3 dialogueStartPosition;
    public Vector3 dialogueEndPosition;
    public Vector3 dialogueCurrentPosition;
    public float ShiftSpeed;
    public float ShiftY;
    [HideInInspector] public float ShiftYStart;
    [HideInInspector] public float ShiftMove = -1f;
    [HideInInspector] public float YToMove;
    [HideInInspector] public float NextFrameY;

    [HideInInspector] public Component[] UIElements;

    void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();

        Player1Script = Player1.GetComponent<Player>();
        Player2Script = Player2.GetComponent<Player>();

        ShiftYStart = ShiftY;
        ShiftY = ShiftYStart;

        dialogueStartPosition = dialogueCurrentPosition = DialogueWindow.transform.localPosition;
        dialogueEndPosition = dialogueStartPosition + new Vector3(0, ShiftY, 0);

        SetState(UIState.Overworld);
    }

    void Update()
    {
        UpdateDialogueBox();
            
    }
    public void UpdateDialogueBox()
    {
        ShiftY = ShiftYStart;
        dialogueEndPosition = dialogueStartPosition + new Vector3(0, ShiftY, 0);

        if (YToMove > 0.0001)
        {
            NextFrameY = YToMove * ShiftSpeed * Time.deltaTime;
            YToMove -= NextFrameY;
        }
        else
        {
            NextFrameY = YToMove;
            YToMove -= NextFrameY;
        }
        dialogueCurrentPosition = DialogueWindow.transform.position;
        dialogueCurrentPosition.y -= NextFrameY*ShiftMove;
        DialogueWindow.transform.position = dialogueCurrentPosition;
    }

    public void OpenDialogueBox()
    {
        YToMove = ShiftY;
        ShiftMove *= -1;
        if (ShiftMove == 1)
        {
            DialogueWindow.transform.localPosition = dialogueEndPosition;
        }
        else
        {
            DialogueWindow.transform.localPosition = dialogueStartPosition;
        }
    }

    public void BattleStart()
    {
        ScanPlayer1Party();
        ScanPlayer2Party();
        Player2LunenTarget(0);
    }

    public void SetState(UIState state)
    {
        for (int i = 0; i < UIObjects.Count; i++)
        {
            if (i == (int)state)
            {
                UIPanelCollection panelCollection = UIObjects[i].GetComponent<UIPanelCollection>();
                if (panelCollection != null)
                {
                    UIObjects[i].SetActive(true);
                    panelCollection.EnableStartingPanels();
                }
                else
                {
                    UIObjects[i].SetActive(true);
                }
            }
            else if (i == (int)currentState)
            {
                UIPanelCollection panelCollection = UIObjects[i].GetComponent<UIPanelCollection>();
                if (panelCollection != null)
                {
                    panelCollection.DisableAllPanels();
                }
                else
                {
                    UIObjects[i].SetActive(false);
                }
            }
            else
            {
                UIPanelCollection panelCollection = UIObjects[i].GetComponent<UIPanelCollection>();
                
                if (panelCollection != null)
                {
                    panelCollection.GetElementCollections();
                    panelCollection.DisableAllPanelsImmediately();
                }
                else
                {
                    UIObjects[i].SetActive(false);
                }
            }
        }

        currentState = state;
    }

    public void SetState(int state)
    {
        SetState((UIState) state);
    }

    public void ScanBothParties()
    {
        ScanPlayer1Party();
        ScanPlayer2Party();
    }

    public void ScanPlayer1Party()
    {
        
        Player1Script.ReloadTeam();

        for (int i = 0; i < sr.director.MaxLunenOut; i++)
        {
            ScanPlayer1Lunen(i);
        }
    }

    public void ScanPlayer1Lunen(int i)
    {
        if (Player1Script.LunenOut.Count > i)
        {
            Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
            Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = Player1Script.LunenOut[i].Nickname;
            Player1LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + Player1Script.LunenOut[i].Level;
            Player1LunenButtons[i].SetActive(true);
            Player1Script.LunenOut[i].loopback = sr;
            Player1Script.LunenOut[i].MonsterTeam = Director.Team.PlayerTeam;
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
                    LunenPanels[i].ActionButtons[j].SetActive(true);
                    LunenPanels[i].ActionButtonScripts[j].Name.GetComponent<Text>().text = Player1Script.LunenOut[i].ActionSet[j].GetComponent<Action>().Name;
                    LunenPanels[i].ActionButtonScripts[j].Type.GetComponent<Text>().text = Types.GetTypeString(Player1Script.LunenOut[i].ActionSet[j].GetComponent<Action>().Type);
                    
                }
            }
            //LunenPanels[i].ActionButtonScripts
        }
        else
        {
            Player1LunenButtons[i].SetActive(false);
        }
    }

    public void ScanPlayer2Party()
    {
        
        Player2Script.ReloadTeam();

        for (int i = 0; i < sr.director.MaxLunenOut; i++)
        {
            if (Player2Script.LunenOut.Count > i)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = Player2Script.LunenOut[i].Nickname;
                Player2LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + Player2Script.LunenOut[i].Level;
                Player2LunenButtons[i].SetActive(true);
                Player2Script.LunenOut[i].loopback = sr;
                Player2Script.LunenOut[i].MonsterTeam = Director.Team.EnemyTeam;
                AssignPlayer2Bars(i);
            }
            else Player2LunenButtons[i].SetActive(false);
        }

        if (Player2Script.LunenAlive == 0)
        {
            sr.battleSetup.MoveToOverworld(true);
        }
    }

    public void SaveGame()
    {
        SetState(sr.battleSetup.lastUIState);
        sr.saveSystemObject.SaveGame();
    }

    public void LoadGame()
    {
        SetState(sr.battleSetup.lastUIState);
        sr.saveSystemObject.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void OpenMenuPanel(string panel)
    {
        UIObjects[3].GetComponent<UIPanelCollection>().SetPanelState(panel, UITransition.State.Enable);
    }

    public void CloseMenuPanel(string panel)
    {
        UIObjects[3].GetComponent<UIPanelCollection>().SetPanelState(panel, UITransition.State.Disable);
    }

    public void OpenMenuPanel(UIElementCollection panel)
    {
        panel.SetCollectionState(UITransition.State.Enable);
    }

    public void CloseMenuPanel(UIElementCollection panel)
    {
        panel.SetCollectionState(UITransition.State.Disable);
    }

    public void SwitchMenuPanel(string panel)
    {
        UIElementCollection uiec = UIObjects[3].GetComponent<UIPanelCollection>().GetPanelWithString(panel);
        if (uiec.currentState == UITransition.State.Enable) CloseMenuPanel(uiec); else OpenMenuPanel(uiec);
    }

    public void Player1MenuClick(int index)
    {
        if (MenuOpen == index)
        {
            if (MenuOpen < 4)
            {
                Player1LunenButtonScripts[MenuOpen - 1].isSelected = false;
            }
            DescriptionPanels[index - 1].GetComponent<UIElementCollection>().SetCollectionState(UITransition.State.Disable);
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
                        DescriptionPanels[MenuOpen - 1].GetComponent<UIElementCollection>().SetCollectionState(UITransition.State.Disable);
                        Player1LunenButtonScripts[MenuOpen - 1].isSelected = false;
                    }
                    DescriptionPanels[index - 1].GetComponent<UIElementCollection>().SetCollectionState(UITransition.State.Enable);
                    Player1LunenButtonScripts[index - 1].isSelected = true;
                    MenuOpen = index;
                }
            }
            else if (index == 5)
            {
                //TEMPORARY: Until Inventory is added
                sr.director.AttemptToCapture();
            }
            else if (index == 6)
            {
                if (sr.battleSetup.InCutscene) sr.battleSetup.cutsceneAdvance = true;
                sr.battleSetup.MoveToOverworld(true);

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

    public int GetLunenSelected(Director.Team team)
    {
        switch (team)
        {
            case Director.Team.PlayerTeam: return MenuOpen-1;
            case Director.Team.EnemyTeam: return EnemyTarget;
        }
        return -1;
    }

    public void ExecuteAction(int index)
    {
        sr.director.PerformAction(Director.Team.PlayerTeam,GetLunenSelected(Director.Team.PlayerTeam), index);
    }
}
