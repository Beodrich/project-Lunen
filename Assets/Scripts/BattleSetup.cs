using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSetup : MonoBehaviour
{
    [HideInInspector]
    public LunaDex referenceDex;
    [HideInInspector]
    public ListOfScenes sceneReference;
    public enum BattleType
    {
        WildEncounter,
        TrainerBattle,
        GymBattle,
        BossFight
    }
    public List<GameObject> PlayerLunenTeam;
    public List<GameObject> EnemyLunenTeam;
    [Space(10)]
    public BattleType typeOfBattle;
    public ListOfScenes.LocationEnum lastOverworld;
    public Vector3 lastSceneLocation;
    [Space(10)]
    public GameObject MonsterTemplate;
    public float SinceLastEncounter = 0f;

    void Awake()
    {
        referenceDex = GetComponent<LunaDex>();
        sceneReference = GetComponent<ListOfScenes>();
        if (GameObject.FindGameObjectsWithTag("BattleSetup").Length >= 2)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update() {
        SinceLastEncounter -= Time.deltaTime;
    }

    public void GenerateWildEncounter(GameObject species, int level)
    {
        EnemyLunenTeam.Clear();
        GameObject wildMonster = Instantiate(MonsterTemplate);
        Monster wM = wildMonster.GetComponent<Monster>();
        wM.battleSetup = this;
        wM.lunaDex = GetComponent<LunaDex>();
        wM.Level = level;
        wM.TemplateToMonster(species.GetComponent<Lunen>());
        wM.MonsterTeam = Director.Team.EnemyTeam;
        EnemyLunenTeam.Add(wildMonster);
        wildMonster.transform.SetParent(this.transform);
        for (int i = 1; i <= level; i++)
        {
            wM.GetLevelUpMove(i);
        }
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
    }

    public void MoveToBattle(int backdrop, int musicTrack)
    {

        sceneReference.LoadScene(ListOfScenes.LocationEnum.BattleScene);
    }

    public void MoveToOverworld()
    {
        sceneReference.LoadScene(lastOverworld);
        List<GameObject> checkToDelete = new List<GameObject>();
        checkToDelete.AddRange(gameObject.transform.Cast<Transform>().Where(c => c.gameObject.tag == "Monster").Select(c => c.gameObject).ToArray());
        foreach (GameObject monster in checkToDelete)
        {
            if (monster.GetComponent<Monster>().MonsterTeam == Director.Team.EnemyTeam)
            {
                Destroy(monster);
            }
        }
        SinceLastEncounter = 5f;
    }

    public void NewOverworld(DoorToLocation door)
    {
        lastSceneLocation = door.SpawnLocation;
        sceneReference.LoadScene(door.TargetLocation);
    }
}
