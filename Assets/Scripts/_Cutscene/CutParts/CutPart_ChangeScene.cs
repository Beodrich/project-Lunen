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

    public Database database;
    public NewSceneType newSceneType;
    public SceneReference TargetScene;
    public string newSceneEntranceGuid;
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
                sr.battleSetup.NewOverworldAt(TargetScene.ScenePath, newScenePosition, newSceneDirection, true);
            break;
            case NewSceneType.ToPosition:
            
                sr.battleSetup.NewOverworldAt(TargetScene.ScenePath, newScenePosition, newSceneDirection);
            break;
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_ChangeScene _cp = (CutPart_ChangeScene)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        newSceneType = _cp.newSceneType;
        TargetScene = _cp.TargetScene;
        newSceneEntranceGuid = _cp.newSceneEntranceGuid;
        newScenePosition = _cp.newScenePosition;
        newSceneDirection = _cp.newSceneDirection;
        database = _cp.database;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            database = (Database)EditorGUILayout.ObjectField("Database: ", database, typeof(Database), true);
            if (database != null)
            {
                int sceneIndex = -1;
                
                newSceneType = (NewSceneType)EditorGUILayout.EnumPopup("Scene Change Type: ", newSceneType);
                if (newSceneType == NewSceneType.ToEntrance || newSceneType == NewSceneType.ToPosition)
                {
                    GUILayout.Space(10);
                    //newScene = (GameScene)EditorGUILayout.ObjectField("New Scene: ", newScene, typeof(GameScene), true);
                    EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("TargetScene"), true);
                    sceneIndex = database.ScenePathToIndex(TargetScene.ScenePath);
                }
                if (sceneIndex >= 0 && sceneIndex < database.AllScenes.Count)
                {
                    if (newSceneType == NewSceneType.ToEntrance)
                    {
                        GUILayout.Space(5);
                        string startingGuid = newSceneEntranceGuid;
                        int newIndex = database.AllScenes[sceneIndex].EntranceGuidToInt(newSceneEntranceGuid);
                        if (newIndex == -1)
                        {
                            newIndex = 0;
                            newSceneEntranceGuid =  database.AllScenes[sceneIndex].IntToGuid(newIndex);
                        }
                        newSceneEntranceGuid =  database.AllScenes[sceneIndex].IntToGuid(EditorGUILayout.Popup("Entrance: ", newIndex,  database.AllScenes[sceneIndex].GetEntrancesArray()));

                        if (newSceneEntranceGuid != startingGuid)
                        {
                            DatabaseSceneEntrance dse =  database.AllScenes[sceneIndex].GuidToEntrance(newSceneEntranceGuid);
                            newScenePosition = new Vector2(dse.position.x, dse.position.y);
                            newSceneDirection = dse.facingDirection;
                        }
                    }
                    else if (newSceneType == NewSceneType.ToPosition)
                    {
                        GUILayout.Space(5);
                        newScenePosition = EditorGUILayout.Vector2Field("New Position: ", newScenePosition);
                        newSceneDirection = (MoveScripts.Direction)EditorGUILayout.EnumPopup("New Direction: ",newSceneDirection);
                    }
                }
            } 
            
        }
    #endif
}
