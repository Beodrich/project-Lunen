using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using MyBox;

public class BattleSetup : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public enum BattleType
    {
        WildEncounter,
        TrainerBattle,
        GymBattle,
        BossFight
    }
    public List<GameObject> PlayerLunenTeam;
    public List<GameObject> EnemyLunenTeam;
    public List<GameObject> RecycleBinTeam;
    
    [Space(10)]
    public bool InBattle;
    public BattleType typeOfBattle;
    public string lastOverworld;
    public Vector3 lastSceneLocation;
    public TrainerLogic lastTrainerEncounter;
    public bool lastBattleVictory;
    [Space(10)]
    public bool gamePaused;
    [Space(10)]
    public PackedCutscene lastCutscene;
    public bool InCutscene;
    private int cutscenePart;
    public int cutsceneNextRoute;
    public bool cutsceneLoopGoing;
    private bool cutsceneAdvance;
    public bool dialogueBoxOpen;
    public bool dialogueBoxNext;
    public bool choiceOpen;
    [Space(10)]
    public int nextEntrance;
    public bool loadEntrance;
    public Vector2 loadPosition;
    public MoveScripts.Direction loadDirection;
    public bool loadMoving;
    [Space(10)]
    public string respawnScene;
    public Vector3 respawnLocation;
    public MoveScripts.Direction respawnDirection;
    [Space(10)]
    public PackedCutscene cutsceneAfterBattle;
    public int cutsceneAfterBattleRoute;
    [Space(10)]
    public float waitTime;
    public float waitTimeCurrent;
    [Space(10)]
    public GameObject MonsterTemplate;
    public float SinceLastEncounter = 0f;
    public CanvasCollection.UIState lastUIState;
    public List<System.Guid> GuidList;
    public bool playerDead;
    public bool cutsceneStoppedBattle;
    [HideInInspector] public Monster attemptToCaptureMonster;

    private void Awake()
    {
        GuidList = new List<System.Guid>();
    }

    private void Update() {
        SinceLastEncounter -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            SubmitPressed();
        }

        if (Input.GetButtonDown("Submit"))
        {
            SubmitPressed();
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if (sr.playerLogic != null)
            {
                if (!sr.playerLogic.move.isMoving)
                {
                    if (!sr.canvasCollection.MenuPanelOpen)
                    {
                        if (!InBattle) OpenMainMenu();
                        else
                        {
                            if (sr.canvasCollection.InventoryPanelOpen) sr.canvasCollection.CloseInventoryWindow(InBattle);
                            if (sr.canvasCollection.PartyPanelOpen) sr.canvasCollection.ClosePartyWindow(InBattle);
                        }
                    }
                    else
                    {
                        if (sr.canvasCollection.OptionsPanelOpen)
                        {
                            sr.settingsSystem.ExitSettings();
                        }
                        else if (sr.canvasCollection.InventoryPanelOpen) sr.canvasCollection.CloseInventoryWindow(InBattle);
                        else if (sr.canvasCollection.PartyPanelOpen) sr.canvasCollection.ClosePartyWindow(InBattle);
                        else
                        {
                            CloseMainMenu();
                        }
                    }
                }
            }
        }

    }

    public void SubmitPressed()
    {
        if (dialogueBoxOpen && !choiceOpen && !gamePaused)
            {
                if (!dialogueBoxNext)
                {
                    dialogueBoxOpen = false;
                    sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Disable);
                }
                AdvanceCutscene();
            }
            if (dialogueBoxOpen && gamePaused && sr.canvasCollection.InventoryPanelOpen)
            {
                if (!dialogueBoxNext)
                {
                    dialogueBoxOpen = false;
                    sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Disable);
                }
                AdvanceCutscene();
            }
    }

    public void OpenMainMenu()
    {
        gamePaused = true;
        sr.canvasCollection.MenuPanelOpen = true;
        sr.canvasCollection.OpenState(CanvasCollection.UIState.MainMenu);
    }

    public void CloseMainMenu()
    {
        gamePaused = false;
        sr.canvasCollection.MenuPanelOpen = false;
        sr.canvasCollection.PartyPanelOpen = false;
        sr.canvasCollection.Lastuiec = null;
        if (sr.canvasCollection.PartySwapSelect != -1)
        {
            sr.canvasCollection.PartyLunenButtonScripts[sr.canvasCollection.PartySwapSelect].isSelected = false;
            sr.canvasCollection.PartySwapSelect = -1;
        }
        sr.canvasCollection.CloseState(CanvasCollection.UIState.MainMenu);
    }

    public void EnterBattle(int music = 0)
    {
        EmptyRecycleBin();
        sr.director.PrepareBattle();
        sr.canvasCollection.OpenState(CanvasCollection.UIState.Battle);
        sr.canvasCollection.OpenState(CanvasCollection.UIState.BattleCharacter);
        InBattle = true;
        sr.canvasCollection.Player1BattleFieldSprites[0].SetAnimationSet(sr.playerLogic.animationSet);
    }

    public void ExitTrainerBattle(bool win)
    {
        lastBattleVictory = win;
        lastTrainerEncounter.ExitBattle(win);
        AdvanceCutscene();
    }

    public void ExitBattleState()
    {
        sr.director.CleanUpBattle();
        sr.canvasCollection.CloseState(CanvasCollection.UIState.Battle);
        sr.canvasCollection.CloseState(CanvasCollection.UIState.BattleCharacter);
        InBattle = false;
        SinceLastEncounter = 5f;

        List<GameObject> checkToDelete = new List<GameObject>();
        checkToDelete.AddRange(gameObject.transform.Cast<Transform>().Where(c => c.gameObject.tag == "Monster").Select(c => c.gameObject).ToArray());
        foreach (GameObject monster in checkToDelete)
        {
            if (monster.GetComponent<Monster>().MonsterTeam == Director.Team.EnemyTeam)
            {
                RecycleBinTeam.Add(monster);
                //Destroy(monster);
            }
        }
    }

    public bool TryWildEncounter(GrassEncounter encounter)
    {
        float chance = Random.Range(0f, 100f);
        if (SinceLastEncounter < 0f)
        {
            if (chance < encounter.chanceModifier)
            {
                PrepareWildEncounter(encounter);
                return true;
            }
            else
            {
                SinceLastEncounter = 0.25f;
                return false;
            }
        }
        else
        {
            
            return false;
        }
    }

    public void PrepareWildEncounter(GrassEncounter encounter)
    {
        float randomChoice = Random.Range(0f, 100f);
        float searcher = 0f;
        int index = -1;

        while (searcher < randomChoice && index < encounter.possibleEncounters.Count-1)
        {
            index++;
            searcher += encounter.possibleEncounters[index].chanceWeight;
        }
        
        GenerateWildEncounter(encounter.possibleEncounters[index].lunen, Random.Range(encounter.possibleEncounters[index].LevelRange.Min, encounter.possibleEncounters[index].LevelRange.Max + 1));
        EnterBattle();
    }

    public void GenerateWildEncounter(Lunen species, int level)
    {
        EnemyLunenTeam.Clear();
        GameObject wildMonster = sr.generateMonster.GenerateLunen(species, level);
        Monster wM = wildMonster.GetComponent<Monster>();
        wM.MonsterTeam = Director.Team.EnemyTeam;
        EnemyLunenTeam.Add(wildMonster);
        wildMonster.transform.SetParent(this.transform);
        typeOfBattle = BattleType.WildEncounter;

        sr.canvasCollection.Player2BattleFieldSprites[0].DisableImage();
    }

    public void GenerateTrainerBattle(TrainerLogic encounter)
    {
        EnemyLunenTeam.Clear();
        EnemyLunenTeam.AddRange(encounter.TeamObjects);
        for (int i = 0; i < EnemyLunenTeam.Count; i++)
        {
            EnemyLunenTeam[i].transform.SetParent(this.transform);
            encounter.Team[i].MonsterTeam = Director.Team.EnemyTeam;
        }
        typeOfBattle = BattleType.TrainerBattle;
        lastTrainerEncounter = encounter;
        sr.eventLog.AddEvent("Trainer Battle Generated!");
    }

    public void EmptyRecycleBin()
    {
        foreach (GameObject monster in RecycleBinTeam)
        {
            Destroy(monster);
        }
        RecycleBinTeam.Clear();
    }

    public void NewOverworld(DoorToLocation door)
    {
        GameScene gs = door.targetScene;
        DatabaseSceneEntrance dse = gs.GuidToEntrance(door.targetGuidString);
        NewOverworldAt
        (
            gs.scene.ScenePath,
            dse.position,
            dse.facingDirection,
            true,
            false
        );
    }

    public void NewOverworldAt(string location, Vector3 position, MoveScripts.Direction direction, bool moving = false, bool openState = true)
    {
        loadEntrance = true;
        loadPosition = position;
        loadDirection = direction;
        loadMoving = moving;
        lastOverworld = location;
        if (openState) sr.canvasCollection.OpenState(CanvasCollection.UIState.SceneSwitch);
        Debug.Log("Loading Scene: " + location);
        SceneManager.LoadSceneAsync(location);
    }

    public void NewOverworldAt(string location, int entranceIndex)
    {
        nextEntrance = entranceIndex;
        lastOverworld = location;
        sr.canvasCollection.OpenState(CanvasCollection.UIState.SceneSwitch);
        Debug.Log("Loading Scene: " + location);
        SceneManager.LoadSceneAsync(location);
    }

    public void StartCutscene(PackedCutscene cutscene, string route = "")
    {
        #if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
        #endif
        if (!InCutscene)
        {
            sr.eventLog.AddEvent("Cutscene Started: \"" + cutscene.cutsceneName + "\" Route: " + route);
            lastCutscene = cutscene;
            InCutscene = true;
            CutsceneChangeInternal(CutsceneFindRoute(route));
            AdvanceCutscene();
            if (!cutsceneLoopGoing) StartCoroutine(playCutscene(transform));
        }
        
    }

    public bool PlayerCanMove()
    {
        return (!(InBattle || InCutscene || sr.canvasCollection.MenuPanelOpen || cutsceneLoopGoing));
    }

    public void DialogueBoxPrepare(CutscenePart part, bool next)
    {
        sr.canvasCollection.RefreshDialogueBox();
        dialogueBoxOpen = true;
        dialogueBoxNext = next;
        if (part.type == CutscenePart.PartType.Dialogue)
        {
            sr.canvasCollection.DialogueText.text = part.text;
        }
        else if (part.type == CutscenePart.PartType.Choice)
        {
            choiceOpen = true;

            sr.canvasCollection.DialogueText.text = part.text;
            sr.canvasCollection.Choice1Button.SetActive(part.useChoice1);
            sr.canvasCollection.Choice2Button.SetActive(part.useChoice2);
            sr.canvasCollection.Choice3Button.SetActive(part.useChoice3);

            sr.canvasCollection.Choice1Text.text = part.choice1Text;
            sr.canvasCollection.Choice2Text.text = part.choice2Text;
            sr.canvasCollection.Choice3Text.text = part.choice3Text;

            sr.canvasCollection.Choice1Route = CutsceneFindRoute(part.choice1Route);
            sr.canvasCollection.Choice2Route = CutsceneFindRoute(part.choice2Route);
            sr.canvasCollection.Choice3Route = CutsceneFindRoute(part.choice3Route);

            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Enable);
        }
    }

    public int CutsceneFindRoute(string route)
    {
        return CutsceneFindRoute(lastCutscene, route);
    }

    public int CutsceneFindRoute(PackedCutscene cutscene, string route)
    {
        //Debug.Log("Finding route in cutscene: " + cutscene.cutsceneName);
        for (int i = 0; i < cutscene.parts.Count; i++)
        {
            if (cutscene.parts[i].title == route && cutscene.parts[i].type == CutscenePart.PartType.ROUTE_START) return i;
        }
        if (route != "") Debug.Log("Unable To Find Route: " + route);
        return 0;
    }

    public IEnumerator playCutscene(Transform transform)
    {
        cutsceneLoopGoing = true;
        
        sr.canvasCollection.OpenState(CanvasCollection.UIState.Dialogue);
        while (cutscenePart < lastCutscene.parts.Count)
        {
            if (lastCutscene.stopsBattle) cutsceneStoppedBattle = true; else cutsceneStoppedBattle = false;
            while (cutsceneAdvance)
            {
                cutscenePart++;
                cutsceneAdvance = false;
                
                
                if (cutscenePart < lastCutscene.parts.Count)
                {
                    CutscenePart part = GetCurrentCutscenePart();
                    sr.eventLog.AddEvent("Cutscene Part " + (cutscenePart+1) + "/" + lastCutscene.parts.Count + " \"" + part.name + "\"");
                    
                    switch (part.type)
                    {
                        default:
                            AdvanceCutscene();
                        break;
                        case CutscenePart.PartType.Movement:
                            part.moveScript.StartCutsceneMove(part);
                        break;

                        case CutscenePart.PartType.Choice:
                            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Enable);
                            goto case CutscenePart.PartType.Dialogue;
                        case CutscenePart.PartType.Dialogue:
                            bool next = true;
                            if (cutscenePart+1 == lastCutscene.parts.Count)
                            {
                                next = false;
                            }
                            else if (lastCutscene.parts[cutscenePart+1].type != CutscenePart.PartType.Dialogue && lastCutscene.parts[cutscenePart+1].type != CutscenePart.PartType.Choice)
                            {
                                next = false;
                            }
                            DialogueBoxPrepare(part, next);
                        break;

                        case CutscenePart.PartType.Battle:
                            if (part.postBattleCutscene)
                            {
                                cutsceneAfterBattle = new PackedCutscene(part.cutsceneAfterBattle);
                                cutsceneAfterBattleRoute = CutsceneFindRoute(cutsceneAfterBattle, part.routeAfterBattle);
                            }
                            if (!part.trainerLogic.defeated && !part.trainerLogic.engaged)
                            {
                                part.trainerLogic.StartTrainerBattle();
                            }
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.Wait:
                            waitTime = part.waitTime;
                            StartCoroutine(cutsceneWait(transform));
                        break;

                        case CutscenePart.PartType.Animation:
                            part.animationActor.animHijack = true;
                            part.animationActor.animTime = 0;
                            part.animationActor.animationType = part.animationActor.animationSet.GetAnimationName(part.animationPlay);
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.HealParty:
                            for (int i = 0; i < PlayerLunenTeam.Count; i++)
                            {
                                PlayerLunenTeam[i].GetComponent<Monster>().Health.z = PlayerLunenTeam[i].GetComponent<Monster>().GetMaxHealth();
                            }
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.SetSpawn:
                            respawnScene = lastOverworld;
                            respawnLocation = sr.playerLogic.gameObject.transform.position;
                            respawnDirection = sr.playerLogic.move.lookDirection;
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.ChangeScene:
                            switch(part.newSceneType)
                            {
                                case CutscenePart.NewSceneType.Respawn:
                                    NewOverworldAt(respawnScene, respawnLocation, respawnDirection);
                                break;
                                case CutscenePart.NewSceneType.ToEntrance:
                                    NewOverworldAt(part.newScene.ScenePath, part.newSceneEntranceIndex);
                                break;
                                case CutscenePart.NewSceneType.ToPosition:
                                    NewOverworldAt(part.newScene.ScenePath, part.newScenePosition, part.newSceneDirection);
                                break;
                            }
                            //AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.NewCutscene:
                            switch(part.newCutsceneType)
                            {
                                case CutscenePart.NewCutsceneType.Global:
                                    CutsceneStartLite(new PackedCutscene(part.cutsceneGlobal), part.cutsceneRoute);
                                break;

                                case CutscenePart.NewCutsceneType.SceneBased:
                                    if (sr.sceneAttributes.sceneCutscenes.Count == 0)
                                    {
                                        AdvanceCutscene();
                                    }
                                    else
                                    {
                                        int nextcutscene = part.cutsceneIndex;
                                        if (sr.sceneAttributes.sceneCutscenes.Count <= nextcutscene) nextcutscene = 0;
                                        CutsceneStartLite(new PackedCutscene(sr.sceneAttributes.sceneCutscenes[part.cutsceneIndex]), part.cutsceneRoute);
                                    }
                                    
                                break;

                                case CutscenePart.NewCutsceneType.Local:
                                    CutsceneStartLite(new PackedCutscene(part.cutsceneLocal), part.cutsceneRoute);
                                break;

                                default:
                                    Debug.Log("[ERROR] NULL CUTSCENE");
                                    AdvanceCutscene();
                                break;
                                
                            }
                        break;

                        case CutscenePart.PartType.SetPanel:
                            if (part.panelState == UITransition.State.Enable)
                            {
                                switch(part.panelSelect)
                                {
                                    default:
                                        sr.canvasCollection.OpenState(part.panelSelect);
                                        AdvanceCutscene();
                                    break;
                                    case CanvasCollection.UIState.Party:
                                        sr.canvasCollection.partyPanelOpenForBattle = true;
                                        sr.canvasCollection.OpenPartyWindow();
                                        //CutsceneStartLite(new PackedCutscene(sr.sceneAttributes.sceneCutscenes[part.cutsceneIndex]), part.cutsceneRoute, -1);
                                    break;
                                    case CanvasCollection.UIState.Battle:
                                        sr.canvasCollection.OpenState(part.panelSelect);
                                        AdvanceCutscene();
                                    break;
                                }
                            }
                            else
                            {
                                switch(part.panelSelect)
                                {
                                    default:
                                        sr.canvasCollection.CloseState(part.panelSelect);
                                        AdvanceCutscene();
                                    break;
                                    case CanvasCollection.UIState.Battle:
                                        sr.canvasCollection.CloseState(part.panelSelect);
                                        AdvanceCutscene();
                                    break;
                                }
                            }
                            
                        break;

                        case CutscenePart.PartType.CheckBattleOver:
                            if (sr.director.EnemyLunenAlive.Count == 0) sr.battleSetup.PlayerWin();
                            else
                            {
                                sr.canvasCollection.OpenState(CanvasCollection.UIState.Battle);
                                sr.canvasCollection.EnsureValidTarget();
                                AdvanceCutscene();
                            }
                            
                            
                        break;

                        case CutscenePart.PartType.ObtainItem:
                            sr.inventory.AddItem(part.itemObtained, part.itemAmount);
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.ObtainLunen:
                            GameObject go = sr.generateMonster.GenerateLunen(part.lunenObtained, part.lunenLevel);
                            //go.GetComponent<Monster>().MonsterTeam = Director.Team.PlayerTeam;
                            PlayerLunenTeam.Add(go);
                            go.transform.SetParent(transform);
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.ChangeRoute:
                            CutsceneChangeInternal(CutsceneFindRoute(part.newRoute));
                            
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.END:
                            CutsceneChangeInternal(lastCutscene.parts.Count);
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.CaptureWildLunen:
                            sr.director.CaptureLunen(attemptToCaptureMonster);
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.SetAsCollected:
                            GuidList.Add(part.guidSet.GetGuid());
                            AdvanceCutscene();
                        break;

                        case CutscenePart.PartType.Destroy:
                            Destroy(part.destroyObject);
                            AdvanceCutscene();
                        break;
                    }
                    if (part.startNextSimultaneous)
                    {
                        AdvanceCutscene();
                    }
                }
            }
            yield return null;
        }
        EndCutscene();
        yield return 0;
    }

    private void EndCutscene()
    {
        sr.eventLog.AddEvent("Cutscene Ends");
        InCutscene = false;
        cutsceneLoopGoing = false;
        cutscenePart = 999999999;
        cutsceneStoppedBattle = false;
    }

    public void DestroyAllChildLunen()
    {
        while (PlayerLunenTeam.Count > 0)
        {
            if (PlayerLunenTeam[0] != null) Destroy(PlayerLunenTeam[0]);
            PlayerLunenTeam.RemoveAt(0);
        }

        while (EnemyLunenTeam.Count > 0)
        {
            if (EnemyLunenTeam[0] != null) Destroy(EnemyLunenTeam[0]);
            EnemyLunenTeam.RemoveAt(0);
        }

        //sr.director.PlayerScripts[0].LunenTeam.Clear();
    }

    public IEnumerator cutsceneWait(Transform transform)
    {
        waitTimeCurrent = 0;
        while (waitTimeCurrent < waitTime) {
            waitTimeCurrent += Time.deltaTime;
            yield return null;
        }
        AdvanceCutscene();
        yield return 0;
    }

    public void PartyLunenSwap(int first, int second)
    {
        GameObject lunen1 = PlayerLunenTeam[first];
        PlayerLunenTeam[first] = PlayerLunenTeam[second];
        PlayerLunenTeam[second] = lunen1;
        PlayerLunenTeam[first].GetComponent<Monster>().ResetCooldown();
        PlayerLunenTeam[second].GetComponent<Monster>().ResetCooldown();
        if (InBattle) sr.director.LoadTeams();
    }

    public void CutsceneStartLite(PackedCutscene cutscene1, int route = 0)
    {
        lastCutscene = cutscene1;
        CutsceneChangeInternal(route);
        AdvanceCutscene();
    }

    public void CutsceneStartLite(PackedCutscene cutscene1, string route = "")
    {
        
        lastCutscene = cutscene1;
        int _route = CutsceneFindRoute(route);
        CutsceneChangeInternal(_route);
        AdvanceCutscene();
    }

    public void AdvanceCutscene()
    {
        cutsceneAdvance = true;
    }

    public void CutsceneChangeInternal(int _part = 0)
    {
        cutscenePart = (_part - 1);
    }

    public void ForceEndCutscene()
    {
        cutscenePart = 99999999;
    }

    public void PlayerWin()
    {
        if (cutsceneAfterBattle != null && typeOfBattle == BattleType.TrainerBattle)
        {
            Debug.Log("Won Trainer Battle! Starting: " + cutsceneAfterBattleRoute);
            CutsceneStartLite(cutsceneAfterBattle, cutsceneAfterBattleRoute);
            Debug.Log("Won Trainer Battle! Starting: " + cutscenePart);
            cutsceneAfterBattle = null;
            cutsceneAfterBattleRoute = 0;
        }
        else
        {
            AdvanceCutscene();
        }
        sr.eventLog.AddEvent("Got To Player Win Function");
        if (typeOfBattle == BattleType.TrainerBattle) ExitTrainerBattle(true);
        ExitBattleState();
        
    }

    public void PlayerLose()
    {
        sr.eventLog.AddEvent("Got To Player Lose Function");
        if (typeOfBattle == BattleType.TrainerBattle) ExitTrainerBattle(false);
        ExitBattleState();
        playerDead = true;
        PackedCutscene newCutscene = sr.database.GetPackedCutscene(0);
        if (cutsceneLoopGoing)
        {
            CutsceneStartLite(newCutscene, 0);
        }
        else
        {
            StartCutscene(newCutscene);
        }
    }

    public void PlayerEscape()
    {
        sr.eventLog.AddEvent("Got To Player Escape Function");
        if (typeOfBattle == BattleType.WildEncounter)
        {
            ExitBattleState();
        }
    }

    public bool GuidInList(System.Guid guid)
    {
        foreach (System.Guid g in GuidList) if (g == guid) return true;
        return false;
    }

    public CutscenePart GetCurrentCutscenePart()
    {
        return lastCutscene.parts[cutscenePart];
    }
}
