using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSetup : MonoBehaviour
{
    public List<GameObject> PlayerLunenTeam;
    public List<GameObject> EnemyLunenTeam;

    public bool CanCapture;
    public bool CanRun;

    public int Backdrop;

    public GameObject MonsterTemplate;

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("BattleSetup").Length >= 2)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void GenerateWildEncounter(GameObject species, int level)
    {
        EnemyLunenTeam.Clear();
        GameObject wildMonster = Instantiate(MonsterTemplate);
        wildMonster.GetComponent<Monster>().Level = level;
        wildMonster.GetComponent<Monster>().TemplateToMonster(species.GetComponent<Lunen>());
        wildMonster.GetComponent<Monster>().Enemy = true;
        EnemyLunenTeam.Add(wildMonster);
        wildMonster.transform.SetParent(this.transform);
        CanRun = true;
        CanCapture = true;
    }

    public void MoveToBattle(int backdrop, int musicTrack)
    {
        SceneManager.LoadScene(0);
    }

    public void MoveToOverworld(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
