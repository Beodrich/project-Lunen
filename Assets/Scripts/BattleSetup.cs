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
    public ListOfScenes.LocationEnum lastOverworld;
    public Vector3 lastSceneLocation;
    public TrainerLogic lastTrainerEncounter;
    public bool lastBattleVictory;
    [Space(10)]
    public bool gamePaused;
    [Space(10)]
    public Cutscene lastCutscene;
    public bool InCutscene;
    public int cutscenePart;
    public int cutsceneRoute;
    public int cutsceneNextRoute;
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
    public ListOfScenes.LocationEnum respawnScene;
    public Vector3 respawnLocation;
    public MoveScripts.Direction respawnDirection;
    [Space(10)]
    public float waitTime;
    public float waitTimeCurrent;
    [Space(10)]
    public GameObject MonsterTemplate;
    public float SinceLastEncounter = 0f;
    public CanvasCollection.UIState lastUIState;
    public List<System.Guid> TrainersDefeated;
    private int lunenIndex;

    private void Awake()
    {
        TrainersDefeated = new List<System.Guid>();
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
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if (!sr.canvasCollection.MenuPanelOpen)
            {
                OpenMainMenu();
            }
            else
            {
                if (sr.canvasCollection.OptionsPanelOpen)
                {
                    sr.settingsSystem.ExitSettings();
                }
                else
                {
                    CloseMainMenu();
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

    public void EnterBattle()
    {
        sr.director.PrepareBattle();
        sr.canvasCollection.SetState(CanvasCollection.UIState.Battle);
        InBattle = true;
    }

    public void ExitBattle()
    {
        if (typeOfBattle == BattleType.TrainerBattle)
        {
            TrainersDefeated.Add(lastTrainerEncounter.GetComponent<GuidComponent>().GetGuid());
            lastTrainerEncounter.ExitBattle(lastBattleVictory);
            //sr.eventLog.AddEvent("GOT HERE");
            if (InCutscene) cutsceneAdvance = true;
        }
        sr.canvasCollection.SetState(CanvasCollection.UIState.Overworld);
        InBattle = false;
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
        
        lunenIndex = (int)encounter.possibleEncounters[index].lunen;
        GenerateWildEncounter(encounter.possibleEncounters[index].lunen, Random.Range(encounter.possibleEncounters[index].LevelRange.Min, encounter.possibleEncounters[index].LevelRange.Max + 1));
        MoveToBattle(0,0);
    }

    public void GenerateWildEncounter(LunaDex.LunenEnum species, int level)
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

    public void MoveToBattle(int backdrop, int musicTrack)
    {
        sr.director.PlayerScripts[0].LunenTeam = PlayerLunenTeam;
        sr.director.PlayerScripts[1].LunenTeam = EnemyLunenTeam;
        foreach (GameObject monster in RecycleBinTeam)
        {
            Destroy(monster);
        }
        EnterBattle();
        //sceneReference.LoadScene(ListOfScenes.LocationEnum.BattleScene);
    }

    public void MoveToOverworld(bool playerVictory)
    {
        //sceneReference.LoadScene(lastOverworld);
        lastBattleVictory = playerVictory;
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
        SinceLastEncounter = 5f;
        ExitBattle();
    }

    public void NewOverworld(DoorToLocation door)
    {
        nextEntrance = door.entranceIndex;
        lastOverworld = door.TargetLocation;
        sr.listOfScenes.LoadScene(door.TargetLocation);
    }

    public void StartCutscene(Cutscene cutscene)
    {
        #if UNITY_EDITOR
            if (EditorApplication.isPlaying && !InCutscene)
            {
                sr.eventLog.AddEvent("Cutscene Started: \"" + cutscene.cutsceneName + "\"");
                lastCutscene = cutscene;
                InCutscene = true;
                cutsceneRoute = 0;
                cutscenePart = -1;
                cutsceneAdvance = true;
                StartCoroutine(playCutscene(transform));
            }
        #else
            if (!InCutscene)
            {
                sr.eventLog.AddEvent("Cutscene Started: \"" + cutscene.cutsceneName + "\"");
                lastCutscene = cutscene;
                InCutscene = true;
                cutsceneRoute = 0;
                cutscenePart = -1;
                cutsceneAdvance = true;
                StartCoroutine(playCutscene(transform));
            }
        #endif
        
    }

    public bool PlayerCanMove()
    {
        return (!(InBattle || InCutscene || sr.canvasCollection.MenuPanelOpen));
    }

    public void DialogueBoxPrepare(Cutscene.Part part, bool next)
    {
        if (!dialogueBoxNext)
        {
            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Dialogue Panel", UITransition.State.Enable);
        }
        dialogueBoxOpen = true;
        dialogueBoxNext = next;
        if (part.type == Cutscene.PartType.Dialogue)
        {
            sr.canvasCollection.DialogueText.text = part.text;
        }
        else if (part.type == Cutscene.PartType.Choice)
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
        sr.canvasCollection.SetState(CanvasCollection.UIState.Dialogue);
        while (cutscenePart < lastCutscene.routes[cutsceneRoute].parts.Count)
        {
            while (cutsceneAdvance)
            {
                cutscenePart++;
                cutsceneAdvance = false;
                
                
                if (cutscenePart < lastCutscene.routes[cutsceneRoute].parts.Count)
                {
                    Cutscene.Part part = lastCutscene.routes[cutsceneRoute].parts[cutscenePart];
                    sr.eventLog.AddEvent("Cutscene Route " + cutsceneRoute + " Part " + (cutscenePart+1) + "/" + lastCutscene.routes[cutsceneRoute].parts.Count + " \"" + part.name + "\"");
                    
                    switch (part.type)
                    {
                        default:
                            cutsceneAdvance = true;
                        break;
                        case Cutscene.PartType.Movement:
                            part.moveScript.StartCutsceneMove(part);
                        break;

                        case Cutscene.PartType.Choice:
                            sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Enable);
                            goto case Cutscene.PartType.Dialogue;
                        case Cutscene.PartType.Dialogue:
                            bool next = true;
                            if (cutscenePart+1 == lastCutscene.routes[cutsceneRoute].parts.Count)
                            {
                                next = false;
                            }
                            else if (lastCutscene.routes[cutsceneRoute].parts[cutscenePart+1].type != Cutscene.PartType.Dialogue && lastCutscene.routes[cutsceneRoute].parts[cutscenePart+1].type != Cutscene.PartType.Choice)
                            {
                                next = false;
                            }
                            DialogueBoxPrepare(part, next);
                        break;

                        case Cutscene.PartType.Battle:
                            if (!part.trainerLogic.defeated)
                            {
                                part.trainerLogic.StartTrainerBattle();
                            }
                            else cutsceneAdvance = true;
                        break;

                        case Cutscene.PartType.Wait:
                            waitTime = part.waitTime;
                            StartCoroutine(cutsceneWait(transform));
                        break;

                        case Cutscene.PartType.HealParty:
                            for (int i = 0; i < PlayerLunenTeam.Count; i++)
                            {
                                PlayerLunenTeam[i].GetComponent<Monster>().Health.z = PlayerLunenTeam[i].GetComponent<Monster>().GetMaxHealth();
                            }
                            cutsceneAdvance = true;
                        break;

                        case Cutscene.PartType.SetSpawn:
                            respawnScene = lastOverworld;
                            respawnLocation = sr.playerLogic.gameObject.transform.position;
                            respawnDirection = sr.playerLogic.move.lookDirection;
                            cutsceneAdvance = true;
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
 
        InCutscene = false;
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

        sr.director.PlayerScripts[0].LunenTeam.Clear();
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
    }
}
