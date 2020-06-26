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
    public int cutscenePart;
    public int cutsceneRoute;
    public int cutsceneNextRoute;
    public bool cutsceneLoopGoing;
    public bool cutsceneAdvance;
    public bool dialogueBoxOpen;
    public bool dialogueBoxNext;
    public bool choiceOpen;
    [Space(10)]
    public int nextEntrance;
    public bool loadEntrance;
    public Vector2 loadPosition;
    public MoveScripts.Direction loadDirection;
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

    private void Awake()
    {
        GuidList = new List<System.Guid>();
    }

    private void Update() {
        SinceLastEncounter -= Time.deltaTime;

        if (Input.GetButtonDown("Submit"))
        {
            if (dialogueBoxOpen && !choiceOpen && !gamePaused)
            {
                if (!dialogueBoxNext)
                {
                    dialogueBoxOpen = false;
                    sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Disable);
                }
                cutsceneAdvance = true;
            }
            if (dialogueBoxOpen && gamePaused && sr.canvasCollection.InventoryPanelOpen)
            {
                if (!dialogueBoxNext)
                {
                    dialogueBoxOpen = false;
                    sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Disable);
                }
                cutsceneAdvance = true;
            }
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
        InBattle = true;
    }

    public void ExitTrainerBattle(bool win)
    {
        lastBattleVictory = win;
        lastTrainerEncounter.ExitBattle(win);
        cutsceneAdvance = true;
    }

    public void ExitBattleState()
    {
        sr.director.CleanUpBattle();
        sr.canvasCollection.CloseState(CanvasCollection.UIState.Battle);
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
        nextEntrance = door.TargetEntrance;
        lastOverworld = door.TargetLocation.ScenePath;
        SceneManager.LoadScene(lastOverworld);
    }

    public void NewOverworldAt(string location, Vector3 position, MoveScripts.Direction direction)
    {
        loadEntrance = true;
        loadPosition = position;
        loadDirection = direction;
        SceneManager.LoadScene(location);
    }

    public void NewOverworldAt(string location, int entranceIndex)
    {
        nextEntrance = entranceIndex;
        lastOverworld = location;
        SceneManager.LoadScene(location);
    }

    public void StartCutscene(PackedCutscene cutscene, int route = 0)
    {
        #if UNITY_EDITOR
            if (EditorApplication.isPlaying && !InCutscene)
            {
                sr.eventLog.AddEvent("Cutscene Started: \"" + cutscene.cutsceneName + "\"");
                lastCutscene = cutscene;
                InCutscene = true;
                cutsceneRoute = route;
                cutscenePart = -1;
                cutsceneAdvance = true;
                if (!cutsceneLoopGoing) StartCoroutine(playCutscene(transform));
            }
        #else
            if (!InCutscene)
            {
                sr.eventLog.AddEvent("Cutscene Started: \"" + cutscene.cutsceneName + "\"");
                lastCutscene = cutscene;
                InCutscene = true;
                cutsceneRoute = route;
                cutscenePart = -1;
                cutsceneAdvance = true;
                if (!cutsceneLoopGoing) StartCoroutine(playCutscene(transform));
            }
        #endif
        
    }

    public bool PlayerCanMove()
    {
        return (!(InBattle || InCutscene || sr.canvasCollection.MenuPanelOpen || cutsceneLoopGoing));
    }

    public void DialogueBoxPrepare(CutscenePart part, bool next)
    {
        if (!dialogueBoxNext)
        {
            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Enable);
        }
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

            sr.canvasCollection.Choice1Route = part.choice1Route;
            sr.canvasCollection.Choice2Route = part.choice2Route;
            sr.canvasCollection.Choice3Route = part.choice3Route;

            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Enable);
        }
    }

    public IEnumerator playCutscene(Transform transform)
    {
        cutsceneLoopGoing = true;
        
        sr.canvasCollection.OpenState(CanvasCollection.UIState.Dialogue);
        while (cutscenePart < lastCutscene.routes[cutsceneRoute].parts.Count)
        {
            if (lastCutscene.stopsBattle) cutsceneStoppedBattle = true; else cutsceneStoppedBattle = false;
            while (cutsceneAdvance)
            {
                cutscenePart++;
                cutsceneAdvance = false;
                
                
                if (cutscenePart < lastCutscene.routes[cutsceneRoute].parts.Count)
                {
                    CutscenePart part = lastCutscene.routes[cutsceneRoute].parts[cutscenePart];
                    sr.eventLog.AddEvent("Cutscene Route " + cutsceneRoute + " Part " + (cutscenePart+1) + "/" + lastCutscene.routes[cutsceneRoute].parts.Count + " \"" + part.name + "\"");
                    
                    switch (part.type)
                    {
                        default:
                            cutsceneAdvance = true;
                        break;
                        case CutscenePart.PartType.Movement:
                            part.moveScript.StartCutsceneMove(part);
                        break;

                        case CutscenePart.PartType.Choice:
                            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Enable);
                            goto case CutscenePart.PartType.Dialogue;
                        case CutscenePart.PartType.Dialogue:
                            bool next = true;
                            if (cutscenePart+1 == lastCutscene.routes[cutsceneRoute].parts.Count)
                            {
                                next = false;
                            }
                            else if (lastCutscene.routes[cutsceneRoute].parts[cutscenePart+1].type != CutscenePart.PartType.Dialogue && lastCutscene.routes[cutsceneRoute].parts[cutscenePart+1].type != CutscenePart.PartType.Choice)
                            {
                                next = false;
                            }
                            DialogueBoxPrepare(part, next);
                        break;

                        case CutscenePart.PartType.Battle:
                            if (part.postBattleCutscene)
                            {
                                cutsceneAfterBattle = new PackedCutscene(part.cutsceneAfterBattle);
                                cutsceneAfterBattleRoute = part.routeAfterBattle;
                            }
                            if (!part.trainerLogic.defeated && !part.trainerLogic.engaged)
                            {
                                part.trainerLogic.StartTrainerBattle();
                            }
                            cutsceneAdvance = true;
                        break;

                        case CutscenePart.PartType.Wait:
                            waitTime = part.waitTime;
                            StartCoroutine(cutsceneWait(transform));
                        break;

                        case CutscenePart.PartType.HealParty:
                            for (int i = 0; i < PlayerLunenTeam.Count; i++)
                            {
                                PlayerLunenTeam[i].GetComponent<Monster>().Health.z = PlayerLunenTeam[i].GetComponent<Monster>().GetMaxHealth();
                            }
                            cutsceneAdvance = true;
                        break;

                        case CutscenePart.PartType.SetSpawn:
                            respawnScene = lastOverworld;
                            respawnLocation = sr.playerLogic.gameObject.transform.position;
                            respawnDirection = sr.playerLogic.move.lookDirection;
                            cutsceneAdvance = true;
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
                            cutsceneAdvance = true;
                        break;

                        case CutscenePart.PartType.NewCutscene:
                            switch(part.newCutsceneType)
                            {
                                case CutscenePart.NewCutsceneType.SceneBased:
                                    CutsceneStartLite(new PackedCutscene(sr.sceneAttributes.sceneCutscenes[part.cutsceneIndex]), part.cutsceneRoute, -1);
                                break;
                            }
                        break;

                        case CutscenePart.PartType.OpenPanel:
                            switch(part.openPanel)
                            {
                                case CanvasCollection.UIState.Party:
                                    sr.canvasCollection.partyPanelOpenForBattle = true;
                                    sr.canvasCollection.OpenPartyWindow();
                                    //CutsceneStartLite(new PackedCutscene(sr.sceneAttributes.sceneCutscenes[part.cutsceneIndex]), part.cutsceneRoute, -1);
                                break;
                                case CanvasCollection.UIState.Battle:
                                    sr.canvasCollection.OpenState(part.openPanel);
                                    cutsceneAdvance = true;
                                break;
                            }
                        break;

                        case CutscenePart.PartType.ClosePanel:
                            switch(part.closePanel)
                            {
                                case CanvasCollection.UIState.Battle:
                                    sr.canvasCollection.CloseState(part.closePanel);
                                    cutsceneAdvance = true;
                                break;
                            }
                        break;

                        case CutscenePart.PartType.CheckBattleOver:
                            if (sr.director.EnemyLunenAlive.Count == 0) sr.battleSetup.PlayerWin();
                            else
                            {
                                sr.canvasCollection.OpenState(CanvasCollection.UIState.Battle);
                                sr.canvasCollection.EnsureValidTarget();
                                cutsceneAdvance = true;
                            }
                            
                            
                        break;
                    }
                    if (part.startNextSimultaneous)
                    {
                        cutsceneAdvance = true;
                    }
                }
            }
            yield return null;
        }
        sr.eventLog.AddEvent("Cutscene Ends");
        InCutscene = false;
        cutsceneLoopGoing = false;
        cutsceneRoute = 0;
        cutscenePart = 0;
        cutsceneStoppedBattle = false;
        yield return 0;
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
        cutsceneAdvance = true;
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

    public void CutsceneStartLite(PackedCutscene cutscene1, int route, int part = -1)
    {
        lastCutscene = cutscene1;
        cutscenePart = part;
        cutsceneRoute = route;
        cutsceneAdvance = true;
    }

    public void PlayerWin()
    {
        if (cutsceneAfterBattle != null)
        {
            CutsceneStartLite(cutsceneAfterBattle, cutsceneAfterBattleRoute);
            cutsceneAfterBattle = null;
            cutsceneAfterBattleRoute = 0;
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
            CutsceneStartLite(newCutscene, 0, -1);
        }
        else
        {
            StartCutscene(newCutscene, 0);
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
}
