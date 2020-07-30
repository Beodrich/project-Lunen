using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupRouter : MonoBehaviour
{
    public Database database;
    
    [Space(10)]
    
    public GameBoot gameBoot;
    public BattleSetup battleSetup;
    public Director director;
    public GenerateMonster generateMonster;
    public CameraFollow cameraFollow;
    public CanvasCollection canvasCollection;
    public PlayerLogic playerLogic;
    public EventLog eventLog;
    public SceneAttributes sceneAttributes;
    public SaveSystemObject saveSystemObject;
    public SettingsSystem settingsSystem;
    public Inventory inventory;
    public StorageSystem storageSystem;
    public SoundManager soundManager;

    public void Awake()
    {
        gameBoot.sr = battleSetup.sr = director.sr = generateMonster.sr = cameraFollow.sr = canvasCollection.sr = saveSystemObject.sr = this;
    }
}
