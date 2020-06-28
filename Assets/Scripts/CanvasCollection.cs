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
        Party,
        YesNo,
        Inventory,
        Lunen
    }

    [System.Serializable]
    public enum PartyAction
    {
        Null,
        ViewStats,
        SwapLunen,
        SwapMoves,
        UseItem,
        Release
    }

    [System.Serializable]
    public enum InventoryAction
    {
        Null,
        LunenCapture,
        HealingItems,
        BuffItems,
        KeyItems,
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
    public List<LunenActionButton> PartyActionButtonScripts;
    public List<LunenActionPanel> LunenPanels;

    public GameObject Player1;
    public GameObject Player2;

    public int MenuOpen = 0;
    public int EnemyTarget = 0;

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

    public Text YNDialogueText;
    public Text YNYesText;
    public Text YNNoText;

    public bool MenuPanelOpen;
    public bool OptionsPanelOpen;
    public bool PartyPanelOpen;
    public bool InventoryPanelOpen;

    [HideInInspector] public UIElementCollection Lastuiec;
    
    [HideInInspector] public int PartySwapSelect = -1;
    [HideInInspector] public int ActionSwapSelect = -1;
    [HideInInspector] public int ItemIndexSelect = -1;
    [HideInInspector] public int InventoryModeSelect = 0;
    public PartyAction partyAction = PartyAction.Null;

    [HideInInspector] public string currentOptionsPanel = "Game Settings Panel";
    [HideInInspector] public List<Monster> PartyTeam;

    [EnumNamedArray(typeof(PartyAction))]
    public List<PartySelectScript> PartyModeSelectButtons;
    public List<PartySelectScript> InventoryModeSelectButtons;
    public List<ItemButtonScript> ItemButtonScripts;

    public UIState openedFirst;

    public delegate void YesNoFunction();
    public YesNoFunction yesNoFunction;

    public bool actionModify;

    public bool partyPanelOpenForBattle;

    void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();

        UICollections = new List<UIPanelCollection>();
        foreach (GameObject go in UIObjects) UICollections.Add(go.GetComponent<UIPanelCollection>());

        SetState(UIState.Overworld);
    }
    void Update()
    {
        if (sr.settingsSystem.TestOn)
        {
            TestOnText.text = "Screen has been set to the new resolution. Click Apply to confirm these changes. Screen will revert in " + Mathf.Ceil(sr.settingsSystem.TestTimeCurrent) + " second" + (Mathf.Ceil(sr.settingsSystem.TestTimeCurrent) == 1 ? "." : "s.");
        }
    }

    public void YesNoPrompt(YesNoFunction newFunction, string dialogueText, string yesText, string noText)
    {
        yesNoFunction = newFunction;
        YNDialogueText.text = dialogueText;
        YNYesText.text = yesText;
        YNNoText.text = noText;
        OpenState(UIState.YesNo);
    }

    public void SetDialogueBox(string text)
    {
        
        DialogueText.text = text;
    }

    public void RefreshDialogueBox()
    {
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.HalfDisable);
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Enable);
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

    public void SetPartyViewState(int index)
    {
        int lastParty = (int)partyAction;
        if (index != lastParty)
        {
            partyAction = (PartyAction)index;
            if ((PartyAction)lastParty != PartyAction.Null) PartyModeSelectButtons[lastParty].SetSelected(false);
            if ((PartyAction)index != PartyAction.Null) PartyModeSelectButtons[index].SetSelected(true);
            EnablePartyViewState((PartyAction)index);
            DisablePartyViewState((PartyAction)lastParty);
            
        }
        else
        {
            partyAction = PartyAction.Null;
            if ((PartyAction)index != PartyAction.Null) PartyModeSelectButtons[index].SetSelected(false);
            DisablePartyViewState((PartyAction)lastParty);
            
            
        }
        
    }

    public void AskToQuit()
    {
        YesNoPrompt(QuitGame, "Are you sure you want to quit?", "Yes, Quit", "No, Don't");
    }

    public void CallYesFunction()
    {
        yesNoFunction();
    }

    public void CallNoFunction()
    {
        CloseState(UIState.YesNo);
    }

    public void HandleBackOrder()
    {

    }

    public void EnablePartyViewState(PartyAction action)
    {
        switch (action)
        {
            default: break;
            case PartyAction.SwapMoves:
                
                UICollections[(int)UIState.Party].SetPanelState("Action Panel", UITransition.State.Enable);
                if (PartySwapSelect == -1) PartyAccess(0);
            break;
            case PartyAction.UseItem:
                OpenInventoryWindow();
                if (PartySwapSelect == -1) PartyAccess(0);
            break;
        }
    }

    public void DisablePartyViewState(PartyAction action)
    {
        switch (action)
        {
            default: break;
            case PartyAction.SwapMoves:
                UICollections[(int)UIState.Party].SetPanelState("Action Panel", UITransition.State.Disable);
            break;
            case PartyAction.UseItem:
                CloseInventoryWindow(sr.battleSetup.InBattle);
            break;
        }
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
        for (int i = 0; i < sr.director.MaxLunenOut; i++)
        {
            if (sr.director.GetLunenCountOut(Director.Team.PlayerTeam) > i)
            {
                Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
                Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = sr.director.PlayerLunenAlive[i].Nickname;
                Player1LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + sr.director.PlayerLunenAlive[i].Level;
                Player1LunenButtons[i].SetActive(true);
                sr.director.PlayerLunenAlive[i].MonsterTeam = Director.Team.PlayerTeam;
                AssignPlayer1Bars(i);
                LunenPanels[i] = DescriptionPanels[i].GetComponent<LunenActionPanel>();
                LunenPanels[i].FindScripts();
                for (int j = 0; j < 4; j++)
                {
                    if (j >= sr.director.PlayerLunenAlive[i].ActionSet.Count)
                    {
                        LunenPanels[i].ActionButtons[j].SetActive(false);
                    }
                    else
                    {
                        LunenPanels[i].ActionButtons[j].SetActive(true);
                        LunenPanels[i].ActionButtonScripts[j].Name.GetComponent<Text>().text = sr.director.PlayerLunenAlive[i].ActionSet[j].Name;
                        //LunenPanels[i].ActionButtonScripts[j].Type.GetComponent<Text>().text = sr.director.PlayerLunenAlive[i].ActionSet[j].Type.name;
                        
                    }
                }
            }
            else
            {
                Player1LunenButtons[i].SetActive(false);
            }
        }
    }

    public void ScanPlayer2Party()
    {
        
        for (int i = 0; i < sr.director.MaxLunenOut; i++)
        {
            if (sr.director.GetLunenCountOut(Director.Team.EnemyTeam) > i)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = sr.director.EnemyLunenAlive[i].Nickname;
                Player2LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + sr.director.EnemyLunenAlive[i].Level;
                Player2LunenButtons[i].SetActive(true);
                sr.director.EnemyLunenAlive[i].MonsterTeam = Director.Team.EnemyTeam;
                AssignPlayer2Bars(i);
            }
            else Player2LunenButtons[i].SetActive(false);
        }
    }

    public void SaveGame()
    {
        sr.saveSystemObject.SaveGame();
        sr.battleSetup.CloseMainMenu();
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
        Player1LunenButtonScripts[index].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = sr.director.GetMonsterOut(Director.Team.PlayerTeam, index);
        Player1LunenButtonScripts[index].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = sr.director.GetMonsterOut(Director.Team.PlayerTeam, index);
        Player1LunenButtonScripts[index].ExperienceSlider.GetComponent<DrawHealthbar>().targetMonster = sr.director.GetMonsterOut(Director.Team.PlayerTeam, index);
    }

    public void AssignPlayer2Bars(int index)
    {
        Player2LunenButtonScripts[index].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = sr.director.GetMonsterOut(Director.Team.EnemyTeam, index);
        Player2LunenButtonScripts[index].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = sr.director.GetMonsterOut(Director.Team.EnemyTeam, index);
        Player2LunenButtonScripts[index].ExperienceSlider.GetComponent<DrawHealthbar>().targetMonster = sr.director.GetMonsterOut(Director.Team.EnemyTeam, index);
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
        CloseState(UIState.MainMenu);
        OpenState(UIState.Options);
    }

    public void OpenPartyWindow()
    {
        UpdatePartyPanelLunen();
        PartyPanelOpen = true;
        CloseState(UIState.MainMenu);
        OpenState(UIState.Party);
        if (sr.battleSetup.InBattle) SetPartyViewState(2);
        else SetPartyViewState(1);
    }

    public void ClosePartyWindow(bool battle)
    {
        PartyPanelOpen = false;
        SetPartyViewState((int)PartyAction.Null);
        if (partyPanelOpenForBattle)
        {
            partyPanelOpenForBattle = false;
            sr.battleSetup.AdvanceCutscene();
        }
        if (!battle) OpenState(UIState.MainMenu);
        CloseState(UIState.Party);
    }

    public void OpenInventoryWindow()
    {
        InventoryModeSelect = 0;
        InventoryPanelOpen = true;
        CloseState(UIState.MainMenu);
        OpenState(UIState.Inventory);
        SetInventoryWindow(0);
        
    }

    public void CloseInventoryWindow(bool battle)
    {
        InventoryPanelOpen = false;
        //SetPartyViewState((int)PartyAction.Null);
        if (!battle && !PartyPanelOpen) OpenState(UIState.MainMenu);
        CloseState(UIState.Inventory);
        InventoryModeSelectButtons[InventoryModeSelect].SetSelected(false);
    }

    public void SetInventoryWindow(int index)
    {
        InventoryModeSelectButtons[InventoryModeSelect].SetSelected(false);
        InventoryModeSelect = index;
        InventoryModeSelectButtons[InventoryModeSelect].SetSelected(true);
        sr.inventory.NewInventoryType((Item.ItemType) index);
        RefreshInventoryButtons();
    }

    public void RefreshInventoryButtons()
    {
        for (int i = 0; i < ItemButtonScripts.Count; i++)
        {
            if (sr.inventory.requestedItems.Count > i)
            {
                ItemButtonScripts[i].SetInventoryEntry(sr.inventory.requestedItems[i]);
            }
            else
            {
                ItemButtonScripts[i].SetInventoryEntry(null);
            }
        }
    }

    public void SwitchMenuPanel(string panel)
    {
        UIElementCollection uiec = UIObjects[3].GetComponent<UIPanelCollection>().GetPanelWithString(panel);
        if (uiec == Lastuiec)
        {
            if (Lastuiec != null) CloseMenuPanel(Lastuiec);
            Lastuiec = null;
        }
        else
        {
            PartyPanelOpen = (panel == "Party Panel");
            if (PartyPanelOpen) UpdatePartyPanelLunen();
            OpenMenuPanel(uiec);
            if (Lastuiec != null) CloseMenuPanel(Lastuiec);
            Lastuiec = uiec;
        }
        
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
                PartyLunenButtonScripts[i].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = PartyTeam[i];
                PartyLunenButtonScripts[i].ExperienceSlider.GetComponent<DrawHealthbar>().targetMonster = PartyTeam[i];
                PartyLunenButtonScripts[i].button.interactable = true;
            }
            else
            {
                PartyLunenButtonScripts[i].Text.GetComponent<Text>().text = "";
                PartyLunenButtonScripts[i].LevelText.GetComponent<Text>().text = "";
                PartyLunenButtonScripts[i].HealthSlider.GetComponent<DrawHealthbar>().targetMonster = null;
                PartyLunenButtonScripts[i].CooldownSlider.GetComponent<DrawHealthbar>().targetMonster = null;
                PartyLunenButtonScripts[i].ExperienceSlider.GetComponent<DrawHealthbar>().targetMonster = null;
                PartyLunenButtonScripts[i].button.interactable = false;
                
            }
        }
    }

    public void UpdatePartyPanelAction(int index)
    {
        Monster BaseMonster = PartyTeam[index];
        for (int i = 0; i < PartyActionButtonScripts.Count; i++)
        {
            
            if (i < BaseMonster.ActionSet.Count)
            {
                PartyActionButtonScripts[i].Name.GetComponent<Text>().text = BaseMonster.ActionSet[i].Name;
                PartyActionButtonScripts[i].button.interactable = true;
                if (partyAction == PartyAction.SwapMoves) PartyActionButtonScripts[i].GetComponent<UITransition>().SetState(UITransition.State.Enable);
                //PartyActionButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + PartyTeam[i].Level;
            }
            //else if (i < BaseMonster.SourceLunen.LearnedActions.Count)
            else
            {
                PartyActionButtonScripts[i].Name.GetComponent<Text>().text = "";
                PartyActionButtonScripts[i].button.interactable = false;
                if (partyAction == PartyAction.SwapMoves) PartyActionButtonScripts[i].GetComponent<UITransition>().SetState(UITransition.State.Enable);
            }
            /*
            else
            {
                PartyActionButtonScripts[i].Name.GetComponent<Text>().text = "";
                PartyActionButtonScripts[i].button.interactable = false;
                PartyActionButtonScripts[i].GetComponent<UITransition>().SetState(UITransition.State.Disable);
            }
            */
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
                if (sr.director.GetMonsterOut(Director.Team.PlayerTeam, index - 1).CurrCooldown <= 0f)
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
                //if (sr.battleSetup.InCutscene) sr.battleSetup.AdvanceCutscene();
                sr.battleSetup.PlayerEscape();

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
        sr.director.GetMonsterOut(Director.Team.PlayerTeam, GetLunenSelected(Director.Team.PlayerTeam)).PerformAction(index);
        //sr.director.PerformAction(Director.Team.PlayerTeam,GetLunenSelected(Director.Team.PlayerTeam), index);
    }

    public void SelectChoice1()
    {
        sr.battleSetup.choiceOpen = false;
        sr.battleSetup.CutsceneChangeInternal(Choice1Route);
        sr.battleSetup.AdvanceCutscene();
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Disable);
    }

    public void SelectChoice2()
    {
        sr.battleSetup.choiceOpen = false;
        sr.battleSetup.CutsceneChangeInternal(Choice2Route);
        sr.battleSetup.AdvanceCutscene();
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Disable);
    }
    
    public void SelectChoice3()
    {
        sr.battleSetup.choiceOpen = false;
        sr.battleSetup.CutsceneChangeInternal(Choice3Route);
        sr.battleSetup.AdvanceCutscene();
        UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Disable);
    }

    public void OpenOptionsPanel(string panelName)
    {
        UICollections[(int)CanvasCollection.UIState.Options].SetPanelState(currentOptionsPanel, UITransition.State.Disable);
        currentOptionsPanel = panelName;
        UICollections[(int)CanvasCollection.UIState.Options].SetPanelState(currentOptionsPanel, UITransition.State.Enable);
    }

    public void ToggleActionModify()
    {
        actionModify = !actionModify;
    }

    public void PartyAccess(int index)
    {
        if (index < PartyTeam.Count) //Make sure selected lunen is within team.
        {
            switch (partyAction)
            {
                default:
                    PartySwitchMode(index);
                    UpdatePartyPanelAction(PartySwapSelect);
                break;

                case PartyAction.SwapLunen:
                    if (PartySwapSelect != -1)
                    {
                        //Player intends swap now
                        PartyLunenButtonScripts[PartySwapSelect].isSelected = false;
                        sr.battleSetup.PartyLunenSwap(PartySwapSelect, index);
                        UpdatePartyPanelLunen();
                        if (sr.battleSetup.InBattle) ScanPlayer1Party();
                        PartySwapSelect = -1;
                    }
                    else
                    {
                        PartySwitchMode(index);
                        UpdatePartyPanelAction(PartySwapSelect);
                    }
                    
                break;

                case PartyAction.SwapMoves:
                    PartySwitchMode(index);
                    UpdatePartyPanelAction(PartySwapSelect);
                break;
            }
            
        }
    }

    public void ActionAccess(int index)
    {
        if (index < PartyTeam[PartySwapSelect].ActionSet.Count) //Make sure selected lunen is within team.
        {
            if (actionModify)
            {
                if (ActionSwapSelect != -1)
                {
                    PartyActionButtonScripts[ActionSwapSelect].scs.enabled = false;
                    PartyActionButtonScripts[ActionSwapSelect].RestoreOriginalColor();
                    PartyTeam[PartySwapSelect].ActionSwap(ActionSwapSelect, index);
                    UpdatePartyPanelAction(PartySwapSelect);
                    ActionSwapSelect = -1;
                }
                else
                {
                    ActionSwitchMode(index);
                }
            }
            else
            {
                if (ActionSwapSelect != index)
                {
                    ActionSwitchMode(index);
                }
                else
                {
                    ActionSwitchMode(-1);
                }
            }
            
        }
    }
    
    public void PartySwitchMode(int index)
    {
        if (PartySwapSelect != -1) PartyLunenButtonScripts[PartySwapSelect].isSelected = false;
        PartySwapSelect = index;
        PartyLunenButtonScripts[PartySwapSelect].isSelected = true;
    }

    public void ActionSwitchMode(int index)
    {
        if (ActionSwapSelect != -1)
        {
            PartyActionButtonScripts[ActionSwapSelect].scs.enabled = false;
            PartyActionButtonScripts[ActionSwapSelect].RestoreOriginalColor();
        }
        ActionSwapSelect = index;
        if (ActionSwapSelect != -1) PartyActionButtonScripts[ActionSwapSelect].scs.enabled = true;
    }

    public void EnsureValidTarget()
    {
        if (sr.director.GetLunenCountOut(Director.Team.EnemyTeam) <= EnemyTarget)
        {
            Player2LunenTarget(sr.director.GetLunenCountOut(Director.Team.EnemyTeam) - 1);
        }
    }

    public void UseItem(int index)
    {
        bool itemUseSuccess = true;
        Item item = sr.inventory.requestedItems[index].item;
        switch (item.itemType)
        {
            case Item.ItemType.Capture:
                if (sr.battleSetup.InBattle)
                {
                    if (sr.battleSetup.typeOfBattle == BattleSetup.BattleType.TrainerBattle)
                    {
                        sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Cannot Use Capture In Trainer Battle"));
                        itemUseSuccess = false;
                        
                    }
                    else
                    {
                        sr.director.AttemptToCapture();
                        CloseInventoryWindow(true);
                    }
                }
                else
                {
                    sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Cannot Use Item Now"));
                    itemUseSuccess = false;
                }
                
            break;
        }
        if (itemUseSuccess) sr.inventory.RemoveItem(item, 1);
    }
}
