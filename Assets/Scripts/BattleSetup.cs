using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public TrainerEncounter lastTrainerEncounter;
    [Space(10)]
    public int nextEntrance;
    [Space(10)]
    public GameObject MonsterTemplate;
    public float SinceLastEncounter = 0f;

    private void Update() {
        SinceLastEncounter -= Time.deltaTime;
    }

    public void EnterBattle()
    {
        sr.director.PrepareBattle();
        sr.canvasCollection.SetState(CanvasCollection.UIState.Battle);
        InBattle = true;
    }

    public void ExitBattle()
    {
        if (typeOfBattle == BattleType.TrainerBattle) lastTrainerEncounter.defeated = true;
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

        GenerateWildEncounter(sr.lunaDex.GetLunenObject(encounter.possibleEncounters[index].lunen), Random.Range(encounter.possibleEncounters[index].LevelRange.Min, encounter.possibleEncounters[index].LevelRange.Max + 1));
        MoveToBattle(0,0);
    }

    public void GenerateWildEncounter(GameObject species, int level)
    {
        EnemyLunenTeam.Clear();
        GameObject wildMonster = Instantiate(MonsterTemplate);
        Monster wM = wildMonster.GetComponent<Monster>();
        wM.loopback = sr;
        wM.Level = level;
        wM.TemplateToMonster(species.GetComponent<Lunen>());
        wM.MonsterTeam = Director.Team.EnemyTeam;
        EnemyLunenTeam.Add(wildMonster);
        wildMonster.transform.SetParent(this.transform);
        wM.GetPreviousMoves();
        typeOfBattle = BattleType.WildEncounter;
    }

    public void GenerateTrainerBattle(TrainerEncounter encounter)
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

    public void MoveToOverworld()
    {
        //sceneReference.LoadScene(lastOverworld);
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
        sr.listOfScenes.LoadScene(door.TargetLocation);
    }
}
