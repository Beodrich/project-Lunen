using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GameBoot : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public bool changeBoot;
    [ConditionalField(nameof(changeBoot))] public ListOfScenes.LocationEnum bootScene;
    [ConditionalField(nameof(changeBoot))] public int bootEntrance;
    public List<GameObject> keepLoaded;
    // Start is called before the first frame update
    private void Awake() {
        if (GameObject.FindGameObjectsWithTag("BattleSetup").Length >= 2)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        for (int i = 0; i < keepLoaded.Count; i++)
        {
            DontDestroyOnLoad(keepLoaded[i]);
        }
        sr = GetComponent<SetupRouter>();
    }
    void Start()
    {

        if (changeBoot)
        {
            sr.battleSetup.nextEntrance = bootEntrance;
            sr.listOfScenes.LoadScene(bootScene);
        }
    }
}
