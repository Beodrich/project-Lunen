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
    public float loadAnimTime;
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
                        if (sr.canvasCollection.StoragePanelOpen) sr.canvasCollection.CloseStorageWindow(InBattle);
                        else if (!InBattle) OpenMainMenu();
                        else
                        {
                            if (sr.canvasCollection.InventoryPanelOpen) sr.canvasCollection.CloseInventoryWindow(InBattle);
                            else if (sr.canvasCollection.StoragePanelOpen) sr.canvasCollection.CloseStorageWindow(InBattle);
                            else if (sr.canvasCollection.PartyPanelOpen) sr.canvasCollection.ClosePartyWindow(InBattle);
                        }
                    }
                    else
                    {
                        if (sr.canvasCollection.OptionsPanelOpen)
                        {
                            sr.settingsSystem.ExitSettings();
                        }
                        else if (sr.canvasCollection.InventoryPanelOpen) sr.canvasCollection.CloseInventoryWindow(InBattle);
                        else if (sr.canvasCollection.StoragePanelOpen) sr.canvasCollection.CloseStorageWindow(InBattle);
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
        sr.canvasCollection.Player1BattleFieldSprites[0].SetAnimationSet(sr.playerLogic.move.animationSet);
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
        if (sr.playerLogic != null)
        {
            loadAnimTime = sr.playerLogic.move.animTime;
        }
        else
        {
            loadAnimTime = 0f;
        }
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

    public void DialogueBoxPrepare(CutPart part, bool next)
    {
        sr.canvasCollection.RefreshDialogueBox();
        dialogueBoxOpen = true;
        dialogueBoxNext = next;
        if (part.cutPartType == CutPartType.Dialogue)
        {
            CutPart_Dialogue part_D = (CutPart_Dialogue)part;

            string textInput = part_D.text;
            textInput = sr.database.DialogueReplace(textInput);
            sr.canvasCollection.DialogueText.text = textInput;
        }
        else if (part.cutPartType == CutPartType.Choice)
        {
            CutPart_Choice part_C = (CutPart_Choice)part;
            choiceOpen = true;

            string textInput = part_C.text;
            textInput = sr.database.DialogueReplace(textInput);
            sr.canvasCollection.DialogueText.text = textInput;

            sr.canvasCollection.Choice1Button.SetActive(part_C.useChoice1);
            sr.canvasCollection.Choice2Button.SetActive(part_C.useChoice2);
            sr.canvasCollection.Choice3Button.SetActive(part_C.useChoice3);

            sr.canvasCollection.Choice1Text.text = part_C.choice1Text;
            sr.canvasCollection.Choice2Text.text = part_C.choice2Text;
            sr.canvasCollection.Choice3Text.text = part_C.choice3Text;

            sr.canvasCollection.Choice1Route = CutsceneFindRoute(part_C.choice1Route);
            sr.canvasCollection.Choice2Route = CutsceneFindRoute(part_C.choice2Route);
            sr.canvasCollection.Choice3Route = CutsceneFindRoute(part_C.choice3Route);

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
            if (cutscene.parts[i].partTitle == route && cutscene.parts[i].cutPartType == CutPartType.RouteStart) return i;
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
                    CutPart part = GetCurrentCutscenePart();
                    sr.eventLog.AddEvent("Cutscene Part " + (cutscenePart+1) + "/" + lastCutscene.parts.Count + " \"" + part.listDisplay + "\"");

                    part.PlayPart(sr);
                    
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

    public CutPart GetCurrentCutscenePart()
    {
        return lastCutscene.parts[cutscenePart];
    }

    public void DestroyAnObject(GameObject _index)
    {
        Destroy(_index);
    }

    public void CreateEmote(EmoteAnim emote, GameObject source)
    {
        waitTime = emote.duration;
        StartCoroutine(cutsceneWait(transform));
        GameObject newEmote = Instantiate(sr.database.EmoteTemplate, source.transform.position + new Vector3(0.5f, 1f, 0), source.transform.rotation);
        newEmote.GetComponent<DestroySelf>().SetDieTime(emote.duration);
        newEmote.GetComponent<SpriteRenderer>().sprite = emote.emoteIcon;
    }

}
