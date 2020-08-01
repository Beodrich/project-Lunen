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
        LoadIntoCurrentScene,
        LoadIntoPosition
    }
    [HideInInspector] public SetupRouter sr;

    [Header("Settings")]

    public bool vsync;

    [Header("On Boot")]

    public BootBehaviour bootBehaviour;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoEntrance, BootBehaviour.LoadIntoPosition)] public SceneReference bootScene;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoEntrance, BootBehaviour.LoadIntoPosition)] public int bootEntrance;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoPosition)] public Vector3 bootPosition;
    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoPosition)] public MoveScripts.Direction bootDirection;

    [ConditionalField(nameof(bootBehaviour), false, BootBehaviour.LoadIntoEntrance, BootBehaviour.LoadIntoCurrentScene, BootBehaviour.LoadIntoPosition)] 
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
        sr = GetComponent<SetupRouter>();
        if (vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        if (GameObject.FindGameObjectsWithTag("BattleSetup").Length >= 2)
        {
            for (int i = 0; i < keepLoaded.Count; i++)
            {
                Destroy(keepLoaded[i]);
            }
            Destroy(gameObject);
        }
        if ((string)sr.database.GetTriggerValue("DEBUGTRIGGERS/FallbackPath") != "")
        {
            Debug.Log("Started Game From Game Scene!");

            bootBehaviour = BootBehaviour.LoadIntoPosition;
            bootScene = new SceneReference();
            bootScene.ScenePath = (string)sr.database.GetTriggerValue("DEBUGTRIGGERS/FallbackPath");
            
            bootPosition = new Vector3((float)sr.database.GetTriggerValue("DEBUGTRIGGERS/FallbackX"), (float)sr.database.GetTriggerValue("DEBUGTRIGGERS/FallbackY"), (float)sr.database.GetTriggerValue("DEBUGTRIGGERS/FallbackZ"));
            bootDirection = MoveScripts.Direction.South;
        }
        DontDestroyOnLoad(this.gameObject);
        for (int i = 0; i < keepLoaded.Count; i++)
        {
            DontDestroyOnLoad(keepLoaded[i]);
        }
        
    }
    void Start()
    {
        sr.database.OnGameStart();
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
            case BootBehaviour.LoadIntoPosition:
                GivePlayerLunen(TestLunenParty.Value);
                sr.battleSetup.NewOverworldAt(bootScene.ScenePath, bootPosition, bootDirection);
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
        sr.director.LoadTeams();
    }
}
