using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class GameBoot : MonoBehaviour
{
    public enum BootBehaviour
    {
        LoadSaveFile,
        LoadIntoFirstLaunch,
        LoadIntoEntrance,
        LoadIntoCurrentScene
    }
    [HideInInspector] public SetupRouter sr;

    [Header("Settings")]

    public bool vsync;

    [Header("On Boot")]

    public BootBehaviour bootBehaviour;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoEntrance)] public SceneReference bootScene;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoEntrance)] public int bootEntrance;

    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoEntrance, BootBehaviour.LoadIntoCurrentScene)] 
    public LunenParty1 TestLunenParty;

    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoFirstLaunch)] 
    public LunenParty2 FirstLunenParty;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoFirstLaunch)]
    public CutsceneScript FirstLaunchCutscene;

    [System.Serializable]
    public class LunenParty1 : CollectionWrapper<GenerateMonster.LunenSetup> {}

    [System.Serializable]
    public class LunenParty2 : CollectionWrapper<GenerateMonster.LunenSetup> {}

    [Space(10)]
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
        sr.canvasCollection.SetState(CanvasCollection.UIState.Overworld);
        switch (bootBehaviour)
        {
            case BootBehaviour.LoadSaveFile:
                if (!sr.saveSystemObject.LoadGame())
                {
                    goto case BootBehaviour.LoadIntoFirstLaunch;
                }
            break;
            case BootBehaviour.LoadIntoFirstLaunch:
                GivePlayerLunen(FirstLunenParty.Value);
                sr.battleSetup.StartCutscene(new PackedCutscene(FirstLaunchCutscene));
            break;
            case BootBehaviour.LoadIntoEntrance:
                GivePlayerLunen(TestLunenParty.Value);
                sr.battleSetup.nextEntrance = bootEntrance;
                sr.battleSetup.lastOverworld = bootScene.ScenePath;
                SceneManager.LoadScene(bootScene.ScenePath);
            break;
            case BootBehaviour.LoadIntoCurrentScene:
                GivePlayerLunen(TestLunenParty.Value);
            break;
        }
    }
    public void GivePlayerLunen(GenerateMonster.LunenSetup[] party)
    {
        foreach (GenerateMonster.LunenSetup setup in party)
        {
            GameObject monster = sr.generateMonster.GenerateLunen(setup.species, setup.level);
            sr.battleSetup.PlayerLunenTeam.Add(monster);
            monster.transform.parent = this.transform;
        }
    }
}
