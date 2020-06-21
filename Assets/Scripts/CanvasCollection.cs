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
        Dialogue,
        Inventory,
        Lunen
    }

    [System.Serializable]
    public enum PartyAction
    {
        Null,
        Swap,
        UseItem,
        ViewStats
    }
    [EnumNamedArray(typeof(UIState))]
    public List<GameObject> UIObjects;
    [HideInInspector] public List<UIPanelCollection> UICollections;
    public Text DialogueText;
    public UIState currentState;
    public UIState lastState;

    [Header("Battle Elements")]
    public List<GameObject> DescriptionPanels;
    public List<GameObject> Player1LunenButtons;
    public List<GameObject> Player2LunenButtons;
    [HideInInspector] public List<LunenButton> Player1LunenButtonScripts;
    [HideInInspector] public List<LunenButton> Player2LunenButtonScripts;
    public List<GameObject> PartyLunenButtons;
    [HideInInspector] public List<LunenButton> PartyLunenButtonScripts;
    public List<LunenActionPanel> LunenPanels;

    public GameObject Player1;
    public GameObject Player2;

    public int MenuOpen = 0;
    public int EnemyTarget = 0;

    [HideInInspector] public Player Player1Script;
    [HideInInspector] public Player Player2Script;

    [HideInInspector] public Component[] UIElements;

    [HideInInspector] public int Choice1Route;
    [HideInInspector] public int Choice2Route;
    [HideInInspector] public int Choice3Route;

    public GameObject Choice1Button;
    public GameObject Choice2Button;
    public GameObject Choice3Button;

    public Text Choice1Text;
    public Text Choice2Text;
    public Text Choice3Text;

    public Text ResolutionText1;
    public Text ResolutionText2;
    public Text TestOnText;

    public bool MenuPanelOpen;
    public bool OptionsPanelOpen;

    [HideInInspector] public UIElementCollection Lastuiec;
    public bool PartyPanelOpen;
    [HideInInspector] public int PartySwapSelect = -1;
    public PartyAction partyAction = PartyAction.Swap;

    [HideInInspector] public string currentOptionsPanel = "Game Settings Panel";
    [HideInInspector] public List<Monster> PartyTeam;

    void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();

        Player1Script = Player1.GetComponent<Player>();
        Player2Script = Player2.GetComponent<Player>();

        UICollections = new List<UIPanelCollection>();
        foreach (GameObject go in UIObjects) UICollections.Add(go.GetComponent<UIPanelCollection>());
        foreach (UIPanelCollection panel in UICollections) panel.GetElementCollections();

        SetState(UIState.Overworld);
    }
    void Update()
    {
        if (sr.settingsSystem.TestOn)
        {
            TestOnText.text = "Screen has been set to the new resolution. Click Apply to confirm these changes. Screen will revert in " + Mathf.Ceil(sr.settingsSystem.TestTimeCurrent) + " second" + (Mathf.Ceil(sr.settingsSystem.TestTimeCurrent) == 1 ? "." : "s.");
        }
    }

    public void SetDialogueBox(string text)
    {
        DialogueText.text = text;
    }

    public void BattleStart()
    {
        ScanPlayer1Party();
        ScanPlayer2Party();
        Player2LunenTarget(0);
    }

    public void ChangeResolution(int scale)
    {
        sr.settingsSystem.ResolutionX.x = 640*scale;
        sr.settingsSystem.ResolutionY.x = 360*scale;
        ResolutionText1.text = ResolutionText2.text = "Resolution: " + (640*scale) + "x" + (360*scale);
    }

    public void RevertState()
    {
        SetState(lastState);
    }

    public void SetState(UIState state, UITransition.State openState = UITransition.State.Enable, UITransition.State closeState = UITransition.State.Disable)
    {
        for (int i = 0; i < UIObjects.Count; i++)
        {
            if (i == (int)state)
            {
                if (UICollections[i] != null)
                {
                    UIObjects[i].SetActive(true);
                    UICollections[i].EnableStartingPanels(openState);
                }
            }
            else if (i == (int)currentState)
            {
                if (UICollections[i] != null)
                {
                    UICollections[i].DisableAllPanels(closeState);
                }
            }
        }
        lastState = currentState;
        currentState = state;
    }

    public void OpenState(UIState state, UITransition.State openState = UITransition.State.Enable)
    {
        for (int i = 0; i < UIObjects.Count; i++)
        {
            if (i == (int)state)
            {
                if (UICollections[i] != null)
                {
                    UIObjects[i].SetActive(true);
                    UICollections[i].EnableStartingPanels(openState);
                    return;
                }
            }
        }
    }

    public void CloseState(UIState state, UITransition.State closeState = UITransition.State.Disable)
    {
        for (int i = 0; i < UIObjects.Count; i++)
        {
            if (i == (int)state)
            {
                if (UICollections[i] != null)
                {
                    UICollections[i].DisableAllPanels(closeState);
                    return;
                }
            }
        }
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
        sr.battleSetup.CloseMainMenu();
        sr.saveSystemObject.SaveGame();
    }

    public void LoadGame()
    {
        sr.battleSetup.CloseMainMenu();
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

    public void OpenOptionsWindow()
    {
        OptionsPanelOpen = true;
        OpenState(UIState.Options);
    }

    public void SwitchMenuPanel(string panel)
    {
        UIElementCollection uiec = UIObjects[3].GetComponent<UIPanelCollection>().GetPanelWithString(panel);
        if (uiec == Lastuiec)
        {
            
        }
        else
        {
            PartyPanelOpen = (panel == "Party Panel");
            if (PartyPanelOpen) UpdatePartyPanelLunen();
            OpenMenuPanel(uiec);
            if (Lastuiec != null) CloseMenuPanel(Lastuiec);
        }
        Lastuiec = uiec;
    }

    public void UpdatePartyPanelLunen()
    {
        //public List<GameObject> PartyLunenButtons;
        //public List<LunenButton> PartyLunenButtonScripts;
        PartyLunenButtonScripts = new List<LunenButton>();
        foreach (GameObject go in PartyLunenButtons) PartyLunenButtonScripts.Add(go.GetComponent<LunenButton>());

        PartyTeam = new List<Monster>();
        foreach (GameObject go in sr.battleSetup.PlayerLunenTeam) PartyTeam.Add(go.GetComponent<Monster>());
        for (int i = 0; i < PartyLunenButtonScripts.Count; i++)
        {
            if (i < PartyTeam.Count)
            {
                PartyLunenButtonScripts[i].Text.GetComponent<Text>().text = PartyTeam[i].Nickname;
                PartyLunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + PartyTeam[i].Level;
                PartyLunenButtonScripts[i].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = PartyTeam[i];
                //PartyLunenButtonScripts[i].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = PartyTeam[i];
            }
            else
            {
                PartyLunenButtonScripts[i].Text.GetComponent<Text>().text = "";
                PartyLunenButtonScripts[i].LevelText.GetComponent<Text>().text = "";
                PartyLunenButtonScripts[i].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = null;
                //PartyLunenButtonScripts[i].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = null;
            }
        }
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

    public void SelectChoice1()
    {
        sr.battleSetup.choiceOpen = false;
        sr.battleSetup.cutscenePart = -1;
        sr.battleSetup.cutsceneRoute = Choice1Route;
        sr.battleSetup.cutsceneAdvance = true;
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Disable);
    }

    public void SelectChoice2()
    {
        sr.battleSetup.choiceOpen = false;
        sr.battleSetup.cutscenePart = -1;
        sr.battleSetup.cutsceneRoute = Choice2Route;
        sr.battleSetup.cutsceneAdvance = true;
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Disable);
    }
    
    public void SelectChoice3()
    {
        sr.battleSetup.choiceOpen = false;
        sr.battleSetup.cutscenePart = -1;
        sr.battleSetup.cutsceneRoute = Choice3Route;
        sr.battleSetup.cutsceneAdvance = true;
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Disable);
    }

    public void OpenOptionsPanel(string panelName)
    {
        UICollections[(int)CanvasCollection.UIState.Options].SetPanelState(currentOptionsPanel, UITransition.State.Disable);
        currentOptionsPanel = panelName;
        UICollections[(int)CanvasCollection.UIState.Options].SetPanelState(currentOptionsPanel, UITransition.State.Enable);
    }

    public void PartyAccess(int index)
    {
        if (index < PartyTeam.Count) //Make sure selected lunen is within team.
        {
            switch (partyAction)
            {
                default:

                break;

                case PartyAction.Swap:
                    if (PartySwapSelect != -1)
                    {
                        //Player intends swap now
                        PartyLunenButtonScripts[PartySwapSelect].isSelected = false;
                        sr.battleSetup.PartyLunenSwap(PartySwapSelect, index);
                        UpdatePartyPanelLunen();
                        Player1Script.LunenTeam = sr.battleSetup.PlayerLunenTeam;
                        if (sr.battleSetup.InBattle) ScanPlayer1Party();
                        PartySwapSelect = -1;
                    }
                    else
                    {
                        PartySwapSelect = index;
                        PartyLunenButtonScripts[PartySwapSelect].isSelected = true;
                    }
                break;
            }
            
        }
    }
}
