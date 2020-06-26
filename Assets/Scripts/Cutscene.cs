using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Malee.List;

public class Cutscene : MonoBehaviour
{
    public string cutsceneName;
    public bool stopsBattle;
    public List<CutsceneRoute> routes;
}

[System.Serializable]
public class PackedCutscene
{
    public string cutsceneName;
    public bool stopsBattle;
    public List<CutsceneRoute> routes;

    public PackedCutscene(string _cutsceneName, List<CutsceneRoute> _routes)
    {
        cutsceneName = _cutsceneName;
        routes = _routes;
    }

    public PackedCutscene(Cutscene cutscene)
    {
        cutsceneName = cutscene.cutsceneName;
        routes = cutscene.routes;
    }

    public PackedCutscene(CutsceneScript cutscene)
    {
        cutsceneName = cutscene.name;
        routes = cutscene.routes;
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
        Movement,
        Animation,
        Battle,
        Choice,
        Wait,
        HealParty,
        SetSpawn,
        ChangeScene,
        ChangeCameraFollow,
        NewCutscene,
        ObtainItem,
        SetAsCollected,
        OpenPanel,
        ClosePanel,
        CheckBattleOver,
    }
    public enum MoveType
    {
        Null,
        ToCollider,
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
    
    public PartType type;

    //Simultaneous Start
    [ConditionalField(nameof(type), false, PartType.Movement)] public bool startNextSimultaneous;

    //Type: Movement
    [ConditionalField(nameof(type), false, PartType.Movement)] public Move moveScript;
    [ConditionalField(nameof(type), false, PartType.Movement)] public bool chooseMoveDirection;
    [ConditionalField(nameof(chooseMoveDirection))] public MoveScripts.Direction movementDirection;
    [ConditionalField(nameof(type), false, PartType.Movement)] public MoveType moveType;
    [ConditionalField(nameof(moveType), false, MoveType.ToCollider)] public string colliderTag;
    [ConditionalField(nameof(moveType), false, MoveType.ToSpaces)] public int spacesToMove;

    //Type: Battle
    [ConditionalField(nameof(type), false, PartType.Battle)] public TrainerLogic trainerLogic;
    [ConditionalField(nameof(type), false, PartType.Battle)] public bool postBattleCutscene;
    [ConditionalField(nameof(postBattleCutscene))] public Cutscene cutsceneAfterBattle;
    [ConditionalField(nameof(postBattleCutscene))] public int routeAfterBattle;

    //Type: Dialogue
    [ConditionalField(nameof(type), false, PartType.Dialogue, PartType.Choice)] [TextArea(5,10)] public string text;

    //Type: Choice
    [ConditionalField(nameof(type), false, PartType.Choice)] public bool useChoice1;
    [ConditionalField(nameof(useChoice1))] public string choice1Text = "Choice 1";
    [ConditionalField(nameof(useChoice1))] public int choice1Route;

    [ConditionalField(nameof(type), false, PartType.Choice)] public bool useChoice2;
    [ConditionalField(nameof(useChoice2))] public string choice2Text = "Choice 2";
    [ConditionalField(nameof(useChoice2))] public int choice2Route;

    [ConditionalField(nameof(type), false, PartType.Choice)] public bool useChoice3;
    [ConditionalField(nameof(useChoice3))] public string choice3Text = "Choice 3";
    [ConditionalField(nameof(useChoice3))] public int choice3Route;

    //Type: Wait
    [ConditionalField(nameof(type), false, PartType.Wait)] public float waitTime;

    //Type: Change Scene
    [ConditionalField(nameof(type), false, PartType.ChangeScene)] public NewSceneType newSceneType;
    [ConditionalField(nameof(newSceneType), false, NewSceneType.ToEntrance, NewSceneType.ToPosition)] public SceneReference newScene;
    [ConditionalField(nameof(newSceneType), false, NewSceneType.ToEntrance)] public int newSceneEntranceIndex;
    [ConditionalField(nameof(newSceneType), false, NewSceneType.ToPosition)] public Vector2 newScenePosition;
    [ConditionalField(nameof(newSceneType), false, NewSceneType.ToPosition)] public MoveScripts.Direction newSceneDirection;

    //Type: New Cutscene
    [ConditionalField(nameof(type), false, PartType.NewCutscene)] public NewCutsceneType newCutsceneType;
    [ConditionalField(nameof(newCutsceneType), false, NewCutsceneType.SceneBased)] public int cutsceneIndex;
    [ConditionalField(nameof(newCutsceneType), false, NewCutsceneType.SceneBased)] public int cutsceneRoute;

    //Type: Open Panel
    [ConditionalField(nameof(type), false, PartType.OpenPanel)] public CanvasCollection.UIState openPanel;

    //Type: Close Panel
    [ConditionalField(nameof(type), false, PartType.ClosePanel)] public CanvasCollection.UIState closePanel;

    [HideInInspector] public TrainerLogic trainerEncounter;
}

[System.Serializable]
public class CutsceneRoute
{
    public string name;
    [Reorderable(paginate = true, pageSize = 0, elementNameProperty = "myString")]
    public MyList parts;

    [System.Serializable]
    public class MyList : ReorderableArray<CutscenePart> {
    }
}