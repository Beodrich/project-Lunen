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
    public Cutscene lastCutscene;
    public bool InCutscene;
    public int cutscenePart;
    public bool cutsceneAdvance;
    public bool dialogueBoxOpen;
    public bool dialogueBoxNext;
    [Space(10)]
    public int nextEntrance;
    public bool loadEntrance;
    public Vector2 loadPosition;
    public MoveScripts.Direction loadDirection;
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

        if (dialogueBoxOpen)
        {
            if (Input.GetButtonDown("Submit"))
            {
                if (!dialogueBoxNext)
                {
                    dialogueBoxOpen = false;
                    sr.canvasCollection.OpenDialogueBox();
                }
                cutsceneAdvance = true;
            }
        }
        else
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (sr.canvasCollection.currentState == CanvasCollection.UIState.MainMenu)
                {
                    sr.canvasCollection.SetState(lastUIState);
                }
                else
                {
                    lastUIState = sr.canvasCollection.currentState;
                    sr.canvasCollection.SetState(CanvasCollection.UIState.MainMenu);
                }
            }
        }

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
                sr.eventLog.AddEvent("Started Cutscene!");
                lastCutscene = cutscene;
                InCutscene = true;
                cutscenePart = -1;
                cutsceneAdvance = true;
                StartCoroutine(playCutscene(transform));
            }
        #else
            if (!InCutscene)
            {
                sr.eventLog.AddEvent("Started Cutscene!");
                lastCutscene = cutscene;
                InCutscene = true;
                cutscenePart = -1;
                cutsceneAdvance = true;
                StartCoroutine(playCutscene(transform));
            }
        #endif
        
    }

    public bool PlayerCanMove()
    {
        return (!(InBattle || InCutscene || sr.canvasCollection.currentState == CanvasCollection.UIState.MainMenu));
    }

    public void DialogueBoxPrepare(Cutscene.Part part, bool next)
    {
        if (!dialogueBoxNext)
        {
            sr.canvasCollection.OpenDialogueBox();
        }
        dialogueBoxOpen = true;
        dialogueBoxNext = next;
        sr.canvasCollection.DialogueText.text = part.text;
    }

    public IEnumerator playCutscene(Transform transform)
    {
        while (cutscenePart < lastCutscene.parts.Length)
        {
            while (cutsceneAdvance)
            {
                cutscenePart++;
                cutsceneAdvance = false;
                
                if (cutscenePart < lastCutscene.parts.Length)
                {
                    Cutscene.Part part = lastCutscene.parts[cutscenePart];
                    switch (part.type)
                    {
                        case Cutscene.PartType.Movement:
                            part.moveScript.StartCutsceneMove(part);
                        break;

                        case Cutscene.PartType.Dialogue:
                            bool next = true;
                            if (cutscenePart+1 == lastCutscene.parts.Length)
                            {
                                next = false;
                            }
                            else if (lastCutscene.parts[cutscenePart+1].type != Cutscene.PartType.Dialogue)
                            {
                                next = false;
                            }
                            DialogueBoxPrepare(part, next);
                        break;

                        case Cutscene.PartType.Trigger:
                            if (part.triggerType == Cutscene.TriggerType.Battle)
                            {
                                if (!part.trainerLogic.defeated) part.trainerLogic.StartTrainerBattle(); else cutsceneAdvance = true;
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
}
