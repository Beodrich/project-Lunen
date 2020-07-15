using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum CutPartType
{
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
    CaptureWildLunen,
    Destroy,
    SetNewSprite
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
        void DrawInspectorPart();
    #endif

    void Duplicate(CutPart cp);
}
