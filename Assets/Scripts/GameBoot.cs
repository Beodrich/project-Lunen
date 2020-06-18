using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GameBoot : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    [Header("Settings")]

    public bool vsync;

    [Header("On Boot")]

    public bool loadSaveFile;
    [ConditionalField(nameof(loadSaveFile), true)] public bool changeBoot;
    [ConditionalField(nameof(changeBoot))] public ListOfScenes.LocationEnum bootScene;
    [ConditionalField(nameof(changeBoot))] public int bootEntrance;
    public List<GameObject> keepLoaded;
    // Start is called before the first frame update
    private void Awake() {
        if (vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
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
        sr.canvasCollection.SetState(CanvasCollection.UIState.MainMenu);
        if (loadSaveFile)
        {
            if (!sr.saveSystemObject.LoadGame())
            {
                sr.battleSetup.nextEntrance = 1;
                sr.listOfScenes.LoadScene(ListOfScenes.LocationEnum.Debug000);
            }
        }
        else if (changeBoot)
        {
            sr.battleSetup.nextEntrance = bootEntrance;
            sr.listOfScenes.LoadScene(bootScene);
        }
        else
        {
            sr.battleSetup.nextEntrance = 1;
            sr.listOfScenes.LoadScene(ListOfScenes.LocationEnum.Debug000);
        }
    }
}
