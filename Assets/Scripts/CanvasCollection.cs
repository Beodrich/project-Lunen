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
        BattleCharacter,
        SceneSwitch,
        LunenStorage
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
    public List<BattleFieldAnims> Player1BattleFieldSprites;
    public List<BattleFieldAnims> Player2BattleFieldSprites;
    public List<GameObject> PartyLunenButtons;
    [HideInInspector] public List<LunenButton> PartyLunenButtonScripts;
    public List<LunenActionButton> PartyActionButtonScripts;
    public List<LunenActionPanel> LunenPanels;

    public GameObject Player1;
    public GameObject Player2;

    public int MenuOpen = 0;
    public int EnemySelfTarget = 0;
    public int EnemyOtherTarget = 0;
    public int PlayerSelfTarget = 0;
    public int PlayerOtherTarget = 0;

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

    public Text ActionDescription;
    public Text StorageLunenDescription;
    public Text PartyLunenDescription;

    public bool MenuPanelOpen;
    public bool OptionsPanelOpen;
    public bool PartyPanelOpen;
    public bool InventoryPanelOpen;
    public bool StoragePanelOpen;

    [HideInInspector] public UIElementCollection Lastuiec;
    
    [HideInInspector] public int PartySwapSelect = -1;
    [HideInInspector] public int ActionSwapSelect = -1;
    [HideInInspector] public int ItemIndexSelect = -1;
    [HideInInspector] public int InventoryModeSelect = 0;
    public PartyAction partyAction = PartyAction.Null;

    [HideInInspector] public int StorageLunenIndexSelect = -1;
    [HideInInspector] public int StorageLunenPageSelect = 0;

    [HideInInspector] public string currentOptionsPanel = "Game Settings Panel";
    [HideInInspector] public List<Monster> PartyTeam;

    [EnumNamedArray(typeof(PartyAction))]
    public List<PartySelectScript> PartyModeSelectButtons;
    public List<PartySelectScript> InventoryModeSelectButtons;
    public List<PartySelectScript> StoragePageSelectButtons;
    public List<ItemButtonScript> ItemButtonScripts;
    public List<ItemButtonScript> ShopButtonScripts;
    public List<ItemButtonScript> LunenStorageButtonScripts;

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
        ActionSwitchMode(-1);
        SetActionDescriptionText(null);
    }

    public void AskToQuit()
    {
        YesNoPrompt(QuitGame, "Are you sure you want to quit?", "Yes, Quit", "No, Don't");
    }

    public void CallYesFunction()
    {
        yesNoFunction();
        CloseState(UIState.YesNo);
    }

    public void CallNoFunction()
    {
        CloseState(UIState.YesNo);
    }

    public void HandleBackOrder()
    {

    }

    public void GetShopStats(UI_Shop shop)
    {
        for (int i = 0; i < ShopButtonScripts.Count; i++)
        {
            if (i < shop.sellItems.Count)
            {
                ShopButtonScripts[i].SetShopEntry(shop.sellItems[i]);
            }
            else ShopButtonScripts[i].SetShopEntry(null);

        }

    }

    public void EnablePartyViewState(PartyAction action)
    {
        switch (action)
        {
            default: break;
            case PartyAction.ViewStats:
                
                UICollections[(int)UIState.Party].SetPanelState("Lunen Info Panel", UITransition.State.Enable);
                if (PartySwapSelect == -1) PartyAccess(0);
                SetActionDescriptionText(null);
                PartyLunenDescription.text = GetLunenInfo(sr.battleSetup.PlayerLunenTeam[PartySwapSelect].GetComponent<Monster>());
            break;
            case PartyAction.SwapMoves:
                
                UICollections[(int)UIState.Party].SetPanelState("Action Panel", UITransition.State.Enable);
                if (PartySwapSelect == -1) PartyAccess(0);
                SetActionDescriptionText(null);
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
            case PartyAction.ViewStats:
                UICollections[(int)UIState.Party].SetPanelState("Lunen Info Panel", UITransition.State.Disable);
            break;
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

    public void ScanParty(Director.Team team)
    {
        if (team == Director.Team.PlayerTeam) ScanPlayer1Party();
        else if (team == Director.Team.EnemyTeam) ScanPlayer2Party();
    }

    public void ScanBothParties()
    {
        ScanPlayer1Party();
        ScanPlayer2Party();
    }

    public void ScanPlayer1Party()
    {
        for (int i = 0; i < sr.director.MaxLunenPossible; i++)
        {
            if (sr.director.GetLunenCountOut(Director.Team.PlayerTeam) > i && i < sr.director.MaxLunenOut)
            {
                Player1LunenButtonScripts[i] = Player1LunenButtons[i].GetComponent<LunenButton>();
                Player1LunenButtonScripts[i].Text.GetComponent<Text>().text = sr.director.PlayerLunenAlive[i].Nickname;
                Player1LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + sr.director.PlayerLunenAlive[i].Level;
                Player1LunenButtonScripts[i].LunenType1.color = sr.director.PlayerLunenAlive[i].SourceLunen.Elements[0].typeColor;
                if (sr.director.PlayerLunenAlive[i].SourceLunen.Elements.Count > 1)
                {
                    Player1LunenButtonScripts[i].LunenType2.color = sr.director.PlayerLunenAlive[i].SourceLunen.Elements[1].typeColor;
                }
                else
                {
                    Player1LunenButtonScripts[i].LunenType2.color = Color.clear;
                }
                Player1BattleFieldSprites[i+1].SetAnimationSet(sr.director.PlayerLunenAlive[i].SourceLunen.animationSet);
                sr.director.PlayerLunenAlive[i].currentuiec = DescriptionPanels[i].GetComponent<UIElementCollection>();
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
                        LunenPanels[i].ActionButtonScripts[j].Name.text = sr.director.PlayerLunenAlive[i].ActionSet[j].Name;
                        switch (sr.director.CanUseMove(sr.director.PlayerLunenAlive[i], sr.director.PlayerLunenAlive[i].ActionSet[j], j))
                        {
                            case 1: //Success
                                LunenPanels[i].ActionButtonScripts[j].button.interactable = true;
                                LunenPanels[i].ActionButtonScripts[j].TypeText.text = " ";
                                LunenPanels[i].ActionButtonScripts[j].image.color = Color.white;
                                LunenPanels[i].ActionButtonScripts[j].TypeText.color = Color.black;
                                LunenPanels[i].ActionButtonScripts[j].Name.color = Color.black;
                                LunenPanels[i].ActionButtonScripts[j].TypeRibbon.color = sr.director.PlayerLunenAlive[i].ActionSet[j].GetMoveType(sr.director.PlayerLunenAlive[i]).typeColor;
                                if (sr.director.PlayerLunenAlive[i].ActionSet[j].ComboMove) LunenPanels[i].ActionButtonScripts[j].ComboRibbon.color = sr.director.PlayerLunenAlive[i].ActionSet[j].ComboType.typeColor;
                                LunenPanels[i].ActionButtonScripts[j].MonsterIndex = i;
                                LunenPanels[i].ActionButtonScripts[j].ActionIndex = j;
                            break;
                            case 2: //No Type For Combo Move
                                LunenPanels[i].ActionButtonScripts[j].image.color = Color.white;
                                LunenPanels[i].ActionButtonScripts[j].button.interactable = false;
                                LunenPanels[i].ActionButtonScripts[j].TypeText.text = "N/C";
                                LunenPanels[i].ActionButtonScripts[j].TypeText.color = Color.black;
                                LunenPanels[i].ActionButtonScripts[j].Name.color = Color.black;
                                LunenPanels[i].ActionButtonScripts[j].TypeRibbon.color = Color.clear;
                                LunenPanels[i].ActionButtonScripts[j].ComboRibbon.color = Color.clear;
                            break;
                            case 3: //Move Recharging
                                LunenPanels[i].ActionButtonScripts[j].image.color = Color.white;
                                LunenPanels[i].ActionButtonScripts[j].button.interactable = false;
                                LunenPanels[i].ActionButtonScripts[j].TypeText.text = (sr.director.PlayerLunenAlive[i].ActionCooldown[j]) + "/" + sr.director.PlayerLunenAlive[i].ActionSet[j].Turns;
                                LunenPanels[i].ActionButtonScripts[j].TypeText.color = Color.black;
                                LunenPanels[i].ActionButtonScripts[j].Name.color = Color.black;
                                LunenPanels[i].ActionButtonScripts[j].TypeRibbon.color = Color.clear;
                                LunenPanels[i].ActionButtonScripts[j].ComboRibbon.color = Color.clear;
                                
                            break;
                        }
                    }
                }
            }
            else
            {
                Player1LunenButtons[i].SetActive(false);
                Player1BattleFieldSprites[i+1].DisableImage();
                DescriptionPanels[i].GetComponent<UIElementCollection>().SetCollectionState(UITransition.State.Disable);
            }
        }
    }

    public void ScanPlayer2Party()
    {
        
        for (int i = 0; i < sr.director.MaxLunenPossible; i++)
        {
            if (sr.director.GetLunenCountOut(Director.Team.EnemyTeam) > i && i < sr.director.MaxLunenOut)
            {
                Player2LunenButtonScripts[i] = Player2LunenButtons[i].GetComponent<LunenButton>();
                Player2LunenButtonScripts[i].Text.GetComponent<Text>().text = sr.director.EnemyLunenAlive[i].Nickname;
                Player2LunenButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + sr.director.EnemyLunenAlive[i].Level;
                Player2LunenButtonScripts[i].LunenType1.color = sr.director.EnemyLunenAlive[i].SourceLunen.Elements[0].typeColor;
                if (sr.director.EnemyLunenAlive[i].SourceLunen.Elements.Count > 1)
                {
                    Player2LunenButtonScripts[i].LunenType2.color = sr.director.EnemyLunenAlive[i].SourceLunen.Elements[1].typeColor;
                }
                else
                {
                    Player2LunenButtonScripts[i].LunenType2.color = Color.clear;
                }
                Player2BattleFieldSprites[i+1].SetAnimationSet(sr.director.EnemyLunenAlive[i].SourceLunen.animationSet);
                Player2LunenButtons[i].SetActive(true);
                sr.director.EnemyLunenAlive[i].MonsterTeam = Director.Team.EnemyTeam;
                AssignPlayer2Bars(i);
            }
            else
            {
                Player2LunenButtons[i].SetActive(false);
                Player2BattleFieldSprites[i+1].DisableImage();
            }
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
        UIObjects[(int)UIState.MainMenu].GetComponent<UIPanelCollection>().SetPanelState(panel, UITransition.State.Enable);
    }

    public void CloseMenuPanel(string panel)
    {
        UIObjects[(int)UIState.MainMenu].GetComponent<UIPanelCollection>().SetPanelState(panel, UITransition.State.Disable);
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
        Lastuiec = null;
        CloseState(UIState.MainMenu);
        OpenState(UIState.Options);
    }

    public void OpenPartyWindow()
    {
        OpenPartyWindow(true);
    }

    public void OpenPartyWindow(bool showModify = true)
    {
        UpdatePartyPanelLunen();
        PartyPanelOpen = true;
        Lastuiec = null;
        CloseState(UIState.MainMenu);
        OpenState(UIState.Party);
        

        if (!showModify)
        {
            UIObjects[(int)UIState.Party].GetComponent<UIPanelCollection>().SetPanelState("Modify Panel", UITransition.State.Disable);
        }
        else
        {
            if (sr.battleSetup.InBattle) SetPartyViewState(2);
            else SetPartyViewState(1);
        }
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
        Lastuiec = null;
        CloseState(UIState.MainMenu);
        OpenState(UIState.Inventory);
        SetInventoryWindow(0);
        
    }

    public void OpenStorageWindow()
    {
        StorageLunenPageSelect = 0;
        StorageLunenIndexSelect = -1;
        StoragePanelOpen = true;
        SetStorageWindow(StorageLunenPageSelect);
        OpenState(UIState.LunenStorage);
    }

    public void SetStorageWindow(int index)
    {
        StoragePageSelectButtons[StorageLunenPageSelect].SetSelected(false);
        StorageLunenPageSelect = index;
        StoragePageSelectButtons[StorageLunenPageSelect].SetSelected(true);
        SelectStorageLunen(-1);
        RefreshStorageButtons();
    }

    public void RefreshStorageButtons()
    {
        for (int i = 0; i < LunenStorageButtonScripts.Count; i++)
        {
            int pageOffset = i + (15*StorageLunenPageSelect);
            if (sr.storageSystem.StoredLunen.Count > pageOffset)
            {
                LunenStorageButtonScripts[i].SetLunenEntry(sr.storageSystem.StoredLunen[pageOffset]);
            }
            else
            {
                LunenStorageButtonScripts[i].SetLunenEntry(null);
            }
        }
    }

    public void CloseStorageWindow(bool battle)
    {
        StoragePanelOpen = false;
        //SetPartyViewState((int)PartyAction.Null);
        SelectStorageLunen(-1);
        if (!battle && !PartyPanelOpen) OpenState(UIState.MainMenu);
        CloseState(UIState.LunenStorage);
        CloseState(UIState.Party);
        PartyPanelOpen = false;
        sr.battleSetup.AdvanceCutscene();
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
        sr.director.LoadTeams();
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
                PartyLunenButtonScripts[i].LunenType1.color = sr.director.PlayerLunenMonsters[i].SourceLunen.Elements[0].typeColor;
                if (sr.director.PlayerLunenMonsters[i].SourceLunen.Elements.Count > 1)
                {
                    PartyLunenButtonScripts[i].LunenType2.color = sr.director.PlayerLunenMonsters[i].SourceLunen.Elements[1].typeColor;
                }
                else
                {
                    PartyLunenButtonScripts[i].LunenType2.color = Color.clear;
                }
                PartyLunenButtonScripts[i].button.interactable = true;
            }
            else
            {
                PartyLunenButtonScripts[i].Text.GetComponent<Text>().text = "";
                PartyLunenButtonScripts[i].LevelText.GetComponent<Text>().text = "";
                PartyLunenButtonScripts[i].HealthSlider.GetComponent<DrawHealthbar>().SetNull();
                PartyLunenButtonScripts[i].CooldownSlider.GetComponent<DrawHealthbar>().SetNull();
                PartyLunenButtonScripts[i].ExperienceSlider.GetComponent<DrawHealthbar>().SetNull();
                PartyLunenButtonScripts[i].LunenType1.color = Color.clear;
                PartyLunenButtonScripts[i].LunenType2.color = Color.clear;
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
                PartyActionButtonScripts[i].Name.text = BaseMonster.ActionSet[i].Name;
                PartyActionButtonScripts[i].button.interactable = true;
                PartyActionButtonScripts[i].TypeText.text = " ";
                PartyActionButtonScripts[i].image.color = Color.white;
                PartyActionButtonScripts[i].TypeText.color = Color.black;
                PartyActionButtonScripts[i].Name.color = Color.black;
                PartyActionButtonScripts[i].TypeRibbon.color = BaseMonster.ActionSet[i].GetMoveType(BaseMonster).typeColor;
                if (BaseMonster.ActionSet[i].ComboMove) PartyActionButtonScripts[i].ComboRibbon.color = BaseMonster.ActionSet[i].ComboType.typeColor;
                else PartyActionButtonScripts[i].ComboRibbon.color = Color.clear;
                if (partyAction == PartyAction.SwapMoves) PartyActionButtonScripts[i].GetComponent<UITransition>().SetState(UITransition.State.Enable);
                //PartyActionButtonScripts[i].LevelText.GetComponent<Text>().text = "LV " + PartyTeam[i].Level;
            }
            //else if (i < BaseMonster.SourceLunen.LearnedActions.Count)
            else
            {
                PartyActionButtonScripts[i].Name.text = " ";
                PartyActionButtonScripts[i].button.interactable = false;
                PartyActionButtonScripts[i].TypeText.text = " ";
                PartyActionButtonScripts[i].TypeText.color = Color.black;
                PartyActionButtonScripts[i].Name.color = Color.black;
                PartyActionButtonScripts[i].TypeRibbon.color = Color.clear;
                PartyActionButtonScripts[i].ComboRibbon.color = Color.clear;
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
        if (sr.director.PlayerLunenAlive.Count > index)
        {
            Player1LunenButtonScripts[PlayerSelfTarget].isSelected = false;
            Player1LunenButtonScripts[index].isSelected = true;
            PlayerSelfTarget = index;
        }
    }

    public void Player2LunenTarget(int index)
    {
        Player2LunenButtonScripts[PlayerOtherTarget].isSelected = false;
        Player2LunenButtonScripts[index].isSelected = true;
        PlayerOtherTarget = index;
    }

    public Monster GetTargetMonster(Director.Team actingTeam, Director.Team targetTeam)
    {
        switch (actingTeam)
        {
            case Director.Team.PlayerTeam:
                if (targetTeam == actingTeam) return sr.director.PlayerLunenAlive[PlayerSelfTarget];
                else return sr.director.EnemyLunenAlive[PlayerOtherTarget];
            case Director.Team.EnemyTeam:
                if (targetTeam == actingTeam) return sr.director.EnemyLunenAlive[EnemySelfTarget];
                else return sr.director.PlayerLunenAlive[EnemyOtherTarget];
        }
        return null;
    }

    public void ExecuteAction(int monster, int index)
    {
        if (!(bool)sr.database.GetTriggerValue("BattleVars/LunenAttacking") && !PartyPanelOpen && !InventoryPanelOpen)
        {
            
            sr.director.GetMonsterOut(Director.Team.PlayerTeam, monster).PerformAction(index);
        }
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
        if (!StoragePanelOpen)
        {
            if (index < PartyTeam.Count) //Make sure selected lunen is within team.
            {
                switch (partyAction)
                {
                    default:
                        PartySwitchMode(index);
                        UpdatePartyPanelAction(PartySwapSelect);
                    break;

                    case PartyAction.ViewStats:
                        PartySwitchMode(index);
                        UpdatePartyPanelAction(PartySwapSelect);
                        PartyLunenDescription.text = GetLunenInfo(sr.battleSetup.PlayerLunenTeam[PartySwapSelect].GetComponent<Monster>());
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
                        
                        ActionSwitchMode(-1);
                        SetActionDescriptionText(null);
                    break;
                }
                
            }
            
        }
        else
        {
            PartySwitchMode(index);
            //UpdatePartyPanelAction(PartySwapSelect);
            if (index != -1)
            {
                SelectStorageLunen(-1);
                UICollections[(int)UIState.LunenStorage].SetPanelState("LunenInfoPanel", UITransition.State.Enable);
                StorageLunenDescription.text = GetLunenInfo(sr.battleSetup.PlayerLunenTeam[PartySwapSelect].GetComponent<Monster>());
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
                    PartyActionButtonScripts[ActionSwapSelect].scs.SetColorState(false);
                    PartyTeam[PartySwapSelect].ActionSwap(ActionSwapSelect, index);
                    UpdatePartyPanelAction(PartySwapSelect);
                    ActionSwapSelect = -1;
                    SetActionDescriptionText(null);
                }
                else
                {
                    ActionSwitchMode(index);
                    SetActionDescriptionText(null);
                }
            }
            else
            {
                if (ActionSwapSelect != index)
                {
                    ActionSwitchMode(index);
                    SetActionDescriptionText(PartyTeam[PartySwapSelect].ActionSet[index], PartyTeam[PartySwapSelect]);
                }
                else
                {
                    ActionSwitchMode(-1);
                    SetActionDescriptionText(null);
                }
            }
            
        }
    }
    
    public void PartySwitchMode(int index)
    {
        if (PartySwapSelect != -1) PartyLunenButtonScripts[PartySwapSelect].isSelected = false;
        PartySwapSelect = index;
        if (index != -1) PartyLunenButtonScripts[PartySwapSelect].isSelected = true;
    }

    public void ActionSwitchMode(int index)
    {
        if (ActionSwapSelect != -1)
        {
            PartyActionButtonScripts[ActionSwapSelect].scs.SetColorState(false);
            PartyActionButtonScripts[ActionSwapSelect].RestoreOriginalColor();
        }
        ActionSwapSelect = index;
        if (ActionSwapSelect != -1) PartyActionButtonScripts[ActionSwapSelect].scs.SetColorState(true);
    }

    public void SetActionDescriptionText(Action action, Monster sourceMonster = null)
    {
        string setValue = " ";
        if (action != null)
        {
            setValue = "Name: " + action.Name;
            setValue += "\nType: " + action.GetMoveType(sourceMonster);
            setValue += "\nCooldown: " + action.Turns + " Turn" + (action.Turns != 1 ? "s" : "");
            if (action.ComboMove)
            {
                setValue += "\nCombo: " + action.ComboType.name;
            }
            setValue += "\n\n" + action.MoveDescription;
            UICollections[(int)UIState.Party].SetPanelState("Action Description Panel", UITransition.State.Enable);
        }
        else
        {
            UICollections[(int)UIState.Party].SetPanelState("Action Description Panel", UITransition.State.Disable);
        }
        ActionDescription.text = setValue;
    }

    public void EnsureValidTarget()
    {
        if (sr.director.GetLunenCountOut(Director.Team.PlayerTeam) <= PlayerSelfTarget)
        {
            Player1MenuClick(sr.director.GetLunenCountOut(Director.Team.PlayerTeam) - 1);
        }
        if (sr.director.GetLunenCountOut(Director.Team.EnemyTeam) <= PlayerOtherTarget)
        {
            Player2LunenTarget(sr.director.GetLunenCountOut(Director.Team.EnemyTeam) - 1);
        }
    }

    public void UseItem(int index)
    {
        Monster monster;
        if (!(bool)sr.database.GetTriggerValue("BattleVars/LunenAttacking"))
        {
            bool itemUseSuccess = false;
            Item item = sr.inventory.requestedItems[index].item;
            switch (item.itemType)
            {
                case Item.ItemType.Capture:
                    if (sr.battleSetup.InBattle)
                    {
                        if (sr.battleSetup.typeOfBattle == BattleSetup.BattleType.TrainerBattle)
                        {
                            sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Cannot Use Capture In Trainer Battle"));
                            
                        }
                        else if (!PartyPanelOpen)
                        {
                            sr.director.AttemptToCapture(item.CatchRate);
                            CloseInventoryWindow(true);
                            itemUseSuccess = true;
                        }
                    }
                    else
                    {
                        sr.battleSetup.StartCutscene(sr.database.GetPackedCutscene("Cannot Use Item Now"));
                    }
                    
                    
                break;
                case Item.ItemType.Healing:
                     monster = GetTargetMonster(Director.Team.PlayerTeam, Director.Team.PlayerTeam);
                    if (sr.battleSetup.InBattle) {
                        if (monster.Health.z >= monster.GetMaxHealth())
                        {
                            Debug.Log("Stop you can't do that :(");
                        }
                        else {
                            int healNumber = Item.GetHealValue(Item.ItemType.Healing);
                            monster.Heal(healNumber);
                            CloseInventoryWindow(true);
                            itemUseSuccess = true;
                        }
                       
                    
                    }

                    break;


                case Item.ItemType.GreatHeal://name can change or usage can change, same with the next case
                    monster = GetTargetMonster(Director.Team.PlayerTeam, Director.Team.PlayerTeam);
                    if (sr.battleSetup.InBattle)
                    {
                        if (monster.Health.z >= monster.GetMaxHealth())
                        {
                            Debug.Log("Stop you can't do that :(");
                        }
                        else
                        {
                            int healNumber = Item.GetHealValue(Item.ItemType.GreatHeal);
                            monster.Heal(healNumber);
                            CloseInventoryWindow(true);
                            itemUseSuccess = true;
                        }


                    }

                    break;

                case Item.ItemType.UltraHeal:
                    monster = GetTargetMonster(Director.Team.PlayerTeam, Director.Team.PlayerTeam);
                    if (sr.battleSetup.InBattle)
                    {
                        if (monster.Health.z >= monster.GetMaxHealth())
                        {
                            Debug.Log("Stop you can't do that :(");
                        }
                        else
                        {
                            int healNumber = Item.GetHealValue(Item.ItemType.UltraHeal);
                            monster.Heal(healNumber);
                            CloseInventoryWindow(true);
                            itemUseSuccess = true;
                        }


                    }

                    break;
                case Item.ItemType.MaxHeal:
                     monster = GetTargetMonster(Director.Team.PlayerTeam, Director.Team.PlayerTeam);
                    if (sr.battleSetup.InBattle)
                    {
                        if (monster.Health.z >= monster.GetMaxHealth())
                        {
                            Debug.Log("Stop you can't do that :(");
                        }
                        else
                        {
                          
                            monster.Heal(monster.GetMaxHealth());
                            CloseInventoryWindow(true);
                            itemUseSuccess = true;
                        }


                    }

                    break;
            }
            if (itemUseSuccess) sr.inventory.RemoveItem(item, 1);
        }
    }

    public void ReleaseLunen()
    {
        if (PartySwapSelect == -1)
        {
            if (StorageLunenIndexSelect != -1)
            {
                YesNoPrompt(ActuallyReleaseLunen, "Are you sure you want to release " + sr.storageSystem.StoredLunen[StorageLunenIndexSelect + (15*StorageLunenPageSelect)].nickname + "?", "Yes", "NO");
            }
        }
        else
        {
            if (sr.battleSetup.PlayerLunenTeam.Count > 1)
            {
                YesNoPrompt(ActuallyReleaseLunen, "Are you sure you want to release " + sr.battleSetup.PlayerLunenTeam[PartySwapSelect].GetComponent<Monster>().Nickname + "?", "Yes", "NO");
            }
        }
        
    }

    public void ActuallyReleaseLunen()
    {
        if (PartySwapSelect == -1)
        {
            sr.storageSystem.StoredLunen.RemoveAt(StorageLunenIndexSelect + (15*StorageLunenPageSelect));
            SelectStorageLunen(-1);
            RefreshStorageButtons();
        }
        else
        {
            Destroy(sr.battleSetup.PlayerLunenTeam[PartySwapSelect]);
            sr.battleSetup.PlayerLunenTeam.RemoveAt(PartySwapSelect);
            PartyAccess(0);
            UpdatePartyPanelLunen();
        }
        
    }

    public void SelectStorageLunen(int index)
    {
        if (StorageLunenIndexSelect != -1)
        {
            LunenStorageButtonScripts[StorageLunenIndexSelect].scs.SetColorState(false);
            LunenStorageButtonScripts[StorageLunenIndexSelect].RestoreOriginalColor();
        }
        
        if (StorageLunenIndexSelect != index)
        {
            StorageLunenIndexSelect = index;
            if (index != -1)
            {
                int pageOffset = index + (15*StorageLunenPageSelect);
                UICollections[(int)UIState.LunenStorage].SetPanelState("LunenInfoPanel", UITransition.State.Enable);
                StorageLunenDescription.text = GetLunenInfo(null, sr.storageSystem.StoredLunen[pageOffset]);
                LunenStorageButtonScripts[StorageLunenIndexSelect].scs.SetColorState(true);
                PartyAccess(-1);
            }
            else
            {
                StorageLunenIndexSelect = -1;
                UICollections[(int)UIState.LunenStorage].SetPanelState("LunenInfoPanel", UITransition.State.Disable);
            }
            
        }
        else
        {
            StorageLunenIndexSelect = -1;
            UICollections[(int)UIState.LunenStorage].SetPanelState("LunenInfoPanel", UITransition.State.Disable);
        }
    }

    public string GetLunenInfo(Monster monster = null, GameData.PlayerLunen gdMonster = null)
    {
        string setValue = "";
        if (monster != null)
        {
            setValue = "Species: " + monster.SourceLunen.name;
            setValue += "\nNickname: " + monster.Nickname;
            setValue += "\nLevel: " + monster.Level;
            if (monster.Level >= sr.database.LevelCap)
            {
                setValue += "\nEXP: Max Level";
            }
            else
            {
                setValue += "\nEXP: " + (monster.Exp.x-monster.Exp.y) + "/" + (monster.Exp.z-monster.Exp.y);
            }
            
            setValue += "\n";
            setValue += "\nHealth: " + monster.Health.z + "/" + monster.GetMaxHealth();
            setValue += "\nAttack: " + monster.Attack.z;
            setValue += "\nDefense: " + monster.Defense.z;
            setValue += "\nSpeed: " + monster.Speed.z;
            setValue += "\n";
            for (int i = 0; i < monster.ActionSet.Count; i++)
            {
                setValue += "\nMove " + (i+1) + ": " + monster.ActionSet[i].Name;
            }
        }
        else if (gdMonster != null)
        {
            Lunen thisLunen = sr.database.IndexToLunen(gdMonster.species);
            setValue = "Species: " + thisLunen.name;
            setValue += "\nNickname: " + gdMonster.nickname;
            setValue += "\nLevel: " + gdMonster.level;
            if (gdMonster.level >= sr.database.LevelCap)
            {
                setValue += "\nEXP: Max Level";
            }
            else
            {
                setValue += "\nEXP: " + gdMonster.exp;
            }
            setValue += "\n";
            setValue += "\nHealth: " + (gdMonster.currentHealth) + "/" + (gdMonster.currentHealth);
            setValue += "\nAttack: " + (thisLunen.Attack.x + thisLunen.Attack.y * gdMonster.level);
            setValue += "\nDefense: " + (thisLunen.Defense.x + thisLunen.Defense.y * gdMonster.level);
            setValue += "\nSpeed: " + (thisLunen.Speed.x + thisLunen.Speed.y * gdMonster.level);
            setValue += "\n";
            
            for (int i = 0; i < gdMonster.learnedMoves.Count; i++)
            {
                setValue += "\nMove " + (i+1) + ": " + sr.database.IndexToAction(gdMonster.learnedMoves[i]).Name;
            }
        }
        return setValue;
    }

    public void StorageMoveBetween()
    {
        if (PartySwapSelect != -1)
        {
            if (sr.battleSetup.PlayerLunenTeam.Count > 1)
            {
                GameObject monsterObjectSelected = sr.battleSetup.PlayerLunenTeam[PartySwapSelect];
                Monster monsterSelected = monsterObjectSelected.GetComponent<Monster>();
                monsterSelected.Heal(monsterSelected.GetMaxHealth());
                sr.storageSystem.StoreLunen(monsterSelected);
                Destroy(monsterObjectSelected);
                sr.battleSetup.PlayerLunenTeam.RemoveAt(PartySwapSelect);
                PartyAccess(-1);
                UpdatePartyPanelLunen();
                RefreshStorageButtons();
                UICollections[(int)UIState.LunenStorage].SetPanelState("LunenInfoPanel", UITransition.State.Disable);
            }
        }
        else if (StorageLunenIndexSelect != -1)
        {
            if (sr.battleSetup.PlayerLunenTeam.Count < 7)
            {
                int pageOffset = StorageLunenIndexSelect + (15*StorageLunenPageSelect);
                GameData.PlayerLunen pl = sr.storageSystem.StoredLunen[pageOffset];
                sr.battleSetup.PlayerLunenTeam.Add(sr.saveSystemObject.GeneratePlayerLunen(pl));
                sr.storageSystem.StoredLunen.RemoveAt(pageOffset);
                RefreshStorageButtons();
                SelectStorageLunen(-1);
                UpdatePartyPanelLunen();
                UICollections[(int)UIState.LunenStorage].SetPanelState("LunenInfoPanel", UITransition.State.Disable);
            }

        }
    }
}
