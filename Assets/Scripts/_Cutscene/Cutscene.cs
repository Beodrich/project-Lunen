using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Malee.List;

public class Cutscene : MonoBehaviour
{
    [Space(10)]
    [Header("Edit Cutscenes With Window->Cutscene Editor!")]
    
    public string cutsceneName;
    public bool stopsBattle;
    public bool showAllData;
    public List<CutscenePart> parts;
}

[System.Serializable]
public class PackedCutscene
{
    public string cutsceneName;
    public bool stopsBattle;
    public bool showAllData;
    public List<CutscenePart> parts;

    public PackedCutscene(string _cutsceneName, List<CutscenePart> _parts)
    {
        cutsceneName = _cutsceneName;
        parts = _parts;
    }

    public PackedCutscene(Cutscene cutscene)
    {
        cutsceneName = cutscene.cutsceneName;
        parts = cutscene.parts;
    }

    public PackedCutscene(CutsceneScript cutscene)
    {
        cutsceneName = cutscene.name;
        parts = cutscene.parts;
        stopsBattle = cutscene.stopsBattle;
    }
}

[System.Serializable]
public class CutscenePart
{
    public enum PartType
    {
        Null,
        Dialogue,
        Choice,
        ROUTE_START,
        END,
        Movement,
        Animation,
        Battle,
        Wait,
        HealParty,
        BLANK,
        SetSpawn,
        ChangeRoute,
        ChangeScene,
        ChangeCameraFollow,
        NewCutscene,
        ObtainItem,
        ObtainLunen,
        SetAsCollected,
        SetPanel,
        CheckBattleOver,
        
        
    }
    public enum MoveType
    {
        Null,
        ToColliderTag,
        ToSpaces,
        ToAnyCollider
    }
    public enum TriggerType
    {
        Null,
        Battle
    }
    public enum NewSceneType
    {
        Null,
        ToEntrance,
        ToPosition,
        Respawn
    }
    public enum NewCutsceneType
    {
        Null,
        Global,
        SceneBased,
        Local
    }

    public string name;
    public string title;
    
    public PartType type;

    //Simultaneous Start
    public bool startNextSimultaneous;

    //Type: Movement
    public Move moveScript;
    public bool chooseMoveDirection;
    public MoveScripts.Direction movementDirection;
    public MoveType moveType;
    public string colliderTag;
    public int spacesToMove;

    //Type: Battle
    public TrainerLogic trainerLogic;
    public bool postBattleCutscene;
    public Cutscene cutsceneAfterBattle;
    public string routeAfterBattle;

    //Type: Dialogue
    [TextArea(5,10)] public string text;

    //Type: Choice
    public bool useChoice1;
    public string choice1Text = "Choice 1";
    public string choice1Route;

    public bool useChoice2;
    public string choice2Text = "Choice 2";
    public string choice2Route;

    public bool useChoice3;
    public string choice3Text = "Choice 3";
    public string choice3Route;

    //Type: Wait
    public float waitTime;

    //Type: Change Scene
    public NewSceneType newSceneType;
    public SceneReference newScene;
    public int newSceneEntranceIndex;
    public Vector2 newScenePosition;
    public MoveScripts.Direction newSceneDirection;

    //Type: New Cutscene
    public NewCutsceneType newCutsceneType;
    public int cutsceneIndex;
    public string cutsceneRoute;
    public CutsceneScript cutsceneGlobal;
    public Cutscene cutsceneLocal;

    //Type: Set Panel
    public CanvasCollection.UIState panelSelect;
    public UITransition.State panelState;

    //Type: Obtain Item
    public Item itemObtained;
    public int itemAmount;

    //Type: Obtain Lunen
    public Lunen lunenObtained;
    public int lunenLevel;

    [HideInInspector] public TrainerLogic trainerEncounter;
}