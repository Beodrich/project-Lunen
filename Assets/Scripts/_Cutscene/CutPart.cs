using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum CutPartType
{
    Animation,
    Blank,
    Battle,
    CaptureWildLunen,
    ChangeCameraFollow,
    ChangeRoute,
    ChangeScene,
    CheckBattleOver,
    Choice,
    Destroy,
    Dialogue,
    End,
    HealParty,
    Movement,
    NewCutscene,
    ObtainItem,
    ObtainLunen,
    RouteStart,
    SetAsCollected,
    SetNewSprite,
    SetPanel,
    SetSpawn,
    ShowEmote,
    StoryTriggerBranch,
    StoryTriggerSet,
    Wait,
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

public interface CutPart
{
    string listDisplay {get;}
    string partTitle {get; set;}
    CutPartType cutPartType {get;}

    bool startNextSimultaneous {get; set;}

    void PlayPart(SetupRouter sr);
    void GetTitle();

    #if UNITY_EDITOR
        void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null);
    #endif

    void Duplicate(CutPart cp);
}
