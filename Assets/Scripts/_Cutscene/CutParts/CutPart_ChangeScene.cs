using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_ChangeScene : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.ChangeScene;
    public string _title = ("New " + _type.ToString());
    public bool _startNextSimultaneous;

    public bool startNextSimultaneous
    {
        get => _startNextSimultaneous;
        set => _startNextSimultaneous = value;
    }

    public string listDisplay
    {
        get => _name;
    }

    public string partTitle
    {
        get => _title;
        set => _title = value;
    }

    public CutPartType cutPartType
    {
        get => _type;
    }

    //Unique Values

    public NewSceneType newSceneType;
    public GameScene newScene;
    public int newSceneEntranceIndex;
    public Vector2 newScenePosition;
    public MoveScripts.Direction newSceneDirection;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        switch(newSceneType)
        {
            case NewSceneType.Respawn:
                sr.battleSetup.playerDead = false;
                sr.battleSetup.NewOverworldAt(sr.battleSetup.respawnScene, sr.battleSetup.respawnLocation, sr.battleSetup.respawnDirection);
            break;
            case NewSceneType.ToEntrance:
                sr.battleSetup.NewOverworldAt(newScene.scene.ScenePath, newSceneEntranceIndex);
            break;
            case NewSceneType.ToPosition:
                sr.battleSetup.NewOverworldAt(newScene.scene.ScenePath, newScenePosition, newSceneDirection);
            break;
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_ChangeScene _cp = (CutPart_ChangeScene)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        newSceneType = _cp.newSceneType;
        newScene = _cp.newScene;
        newSceneEntranceIndex = _cp.newSceneEntranceIndex;
        newScenePosition = _cp.newScenePosition;
        newSceneDirection = _cp.newSceneDirection;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart()
        {
            newSceneType = (NewSceneType)EditorGUILayout.EnumPopup("Scene Change Type: ", newSceneType);
            if (newSceneType == NewSceneType.ToEntrance || newSceneType == NewSceneType.ToPosition)
            {
                GUILayout.Space(10);
                newScene = EditorGUILayout.ObjectField("New Scene: ", newScene, typeof(GameScene)) as GameScene;
            }
            if (newSceneType == NewSceneType.ToEntrance)
            {
                GUILayout.Space(5);
                newSceneEntranceIndex = EditorGUILayout.IntField("Entrance ID: ", newSceneEntranceIndex);
            }
            else if (newSceneType == NewSceneType.ToPosition)
            {
                GUILayout.Space(5);
                newScenePosition = EditorGUILayout.Vector2Field("New Position: ", newScenePosition);
                newSceneDirection = (MoveScripts.Direction)EditorGUILayout.EnumPopup("New Direction: ",newSceneDirection);
            }
        }
    #endif
}
