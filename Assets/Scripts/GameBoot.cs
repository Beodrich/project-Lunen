using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoot : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public ListOfScenes.LocationEnum bootScene;
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
        sr.listOfScenes.LoadScene(bootScene);
    }
}
