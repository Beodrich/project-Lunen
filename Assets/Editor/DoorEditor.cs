﻿//REMEMBER TO ADD TO BOTH EDITORS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using MyBox;

[CustomEditor(typeof(DoorToLocation))]
public class DoorEditor : Editor
{
    DoorToLocation door;
    SerializedProperty scene;
    bool showPosition = false;
    int fallbackIndex = -1;

    private void OnEnable()
    {
        door = (DoorToLocation)target;
        GameObject sceneatt = GameObject.Find("SceneAttributes");
        if (sceneatt != null)
        {
            door.attributes = sceneatt.GetComponent<SceneAttributes>();
            door.attributes.RefreshDoors();
            if (door.targetScene != null) fallbackIndex = door.targetScene.EntranceGuidToInt(door.targetGuidString);
        }
        
    }

    private void OnCutsceneSwitch()
    {
        //list = new ReorderableList(so, serializedObject.FindPro)
    }


    public override void OnInspectorGUI()
    {
        
        //if (cutscene.showAllData)
        if (door.attributes == null)
        {
            base.OnInspectorGUI();
        }
        else
        {
            serializedObject.Update();
            EditorStyles.textField.wordWrap = true;
            

            GUIStyle fontStyle = new GUIStyle( GUI.skin.button );
            fontStyle.alignment = TextAnchor.MiddleLeft;

            GUILayout.Label("Door: " + door.name, EditorStyles.boldLabel);
            GUILayout.Space(20);

            door.doorSize = EditorGUILayout.Vector2Field("Size: ", door.doorSize);

            door.targetScene = (GameScene)EditorGUILayout.ObjectField("Scene: ", door.targetScene, typeof(GameScene), true);
            
            if (door.targetScene != null)
            {
                EnsureEntranceGuid();
            }
            
            GUILayout.Space(10);

            door.exitDirection = (MoveScripts.Direction)EditorGUILayout.EnumPopup("Exit Direction: ", door.exitDirection);

            GUILayout.Space(10);

            door.fadeOutOnTransition = EditorGUILayout.Toggle("Fade To Black: ", door.fadeOutOnTransition);
            door.stopOnTransition = EditorGUILayout.Toggle("Stop On Transition: ", door.stopOnTransition);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Go To Door Destination"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    string currentScene = EditorSceneManager.GetActiveScene().path;

                    GameScene nextScene = door.targetScene;
                    DatabaseSceneEntrance dse = nextScene.GuidToEntrance(door.targetGuidString);

                    EditorSceneManager.OpenScene(nextScene.scene);

                    GameObject scene = GameObject.Find("SceneAttributes");
                    if (scene != null)
                    {
                        SceneAttributes newSceneAttributes = scene.GetComponent<SceneAttributes>();
                        if (newSceneAttributes != null)
                        {
                            SceneView.lastActiveSceneView.LookAt(dse.position);
                        }
                        else
                        {
                            Debug.LogError("[ERROR] Unable to find SceneAttributes Class!");
                            EditorSceneManager.OpenScene(currentScene);
                        }
                    }
                    else
                    {
                        Debug.LogError("[ERROR] Unable to find SceneAttributes Object!");
                        EditorSceneManager.OpenScene(currentScene);
                    }
                }


                
            }
            GUILayout.EndHorizontal();

            showPosition = EditorGUILayout.Foldout(showPosition, "Debug Variables");
            if (showPosition)
            {
                EditorGUILayout.LabelField("Target Info: ");
                EditorGUILayout.LabelField("    GUID String: " + door.targetGuidString);
                DatabaseSceneEntrance dse = door.targetScene.GuidToEntrance(door.targetGuidString);
                if (dse != null)
                {
                    EditorGUILayout.LabelField("    Name: " + dse.name);
                    EditorGUILayout.LabelField("    Exit Direction: " + dse.facingDirection.ToString());
                }
                else
                {
                    EditorGUILayout.LabelField("    DSE NOT FOUND");
                }
                
            }
            
            if (door != null)
            {
                serializedObject.ApplyModifiedProperties();
                if (GUI.changed)
                {
                    
                    door.attributes.RefreshDoors();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    EditorUtility.SetDirty(door);
                    EditorUtility.SetDirty(door.attributes);
                    EditorUtility.SetDirty(door.attributes.thisScene);

                    AssetDatabase.SaveAssets();
                }
            }
        }
        
    }

    public void EnsureEntranceGuid()
    {
        int newIndex = fallbackIndex;

        newIndex = EditorGUILayout.Popup("Entrance: ", newIndex, door.targetScene.GetEntrancesArray());

        if (newIndex != fallbackIndex)
        {
            fallbackIndex = newIndex;
            door.targetGuidString = door.targetScene.IntToGuid(fallbackIndex);
            
        }
    }

    public void OnDestroy()
    {
        if (Application.isEditor)
        {
            if (door == null)
            {
                //Debug.Log("Test!");
            }
        }
    }

    /*
    private void DrawPart(CutscenePart part)
    {
        part.title = EditorGUILayout.TextField("Part Title: ", part.title);
        switch (part.type)
        {
            default:
                part.name = " [" + part.type.ToString() + "] " + part.title;
            break;

            case CutscenePart.PartType.BLANK:
                part.name = " ";
            break;

            case CutscenePart.PartType.Dialogue:
            case CutscenePart.PartType.Choice:
                if (part.title == "") part.name = " [" + part.type.ToString() + "] " + part.text;
                else goto default;
            break;
        }
            
        part.type = (CutscenePart.PartType)EditorGUILayout.EnumPopup("Cutscene Part Type: ", part.type);
        if (part.type == CutscenePart.PartType.Movement || part.type == CutscenePart.PartType.Dialogue)
        {
            part.startNextSimultaneous = EditorGUILayout.Toggle("Start Next Part Too ", part.startNextSimultaneous);
            if (part.startNextSimultaneous && list.index < cutscene.parts.Count -1)
            {
                GUILayout.Label("Will Play This Part With " + cutscene.parts[list.index+1].name, EditorStyles.boldLabel);
            }
        }
        else
        {
            part.startNextSimultaneous = false;
        }
        
        GUILayout.Space(5);
        GuiLine(2);
        GUILayout.Space(5);
        switch(part.type)
        {
            default:
                GUILayout.Label("[ERROR]", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.Dialogue:
                GUILayout.Label("Cutscene Text: ");
                part.text = EditorGUILayout.TextArea(part.text);
            break;

            case CutscenePart.PartType.Choice:
                GUILayout.Label("Cutscene Text: ");
                part.text = EditorGUILayout.TextArea(part.text);

                EditorGUIUtility.labelWidth = 100;
                EditorGUIUtility.fieldWidth = 100;
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                part.useChoice1 = EditorGUILayout.Toggle("Choice 1: ", part.useChoice1);
                GUI.enabled = part.useChoice1;
                part.choice1Text = EditorGUILayout.TextField(" Option Text:", part.choice1Text);
                part.choice1Route = EditorGUILayout.TextField(" Option Route:", part.choice1Route);
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                part.useChoice2 = EditorGUILayout.Toggle("Choice 2: ", part.useChoice2);
                GUI.enabled = part.useChoice2;
                part.choice2Text = EditorGUILayout.TextField(" Option Text:", part.choice2Text);
                part.choice2Route = EditorGUILayout.TextField(" Option Route:", part.choice2Route);
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                part.useChoice3 = EditorGUILayout.Toggle("Choice 3: ", part.useChoice3);
                GUI.enabled = part.useChoice3;
                part.choice3Text = EditorGUILayout.TextField(" Option Text:", part.choice3Text);
                part.choice3Route = EditorGUILayout.TextField(" Option Route:", part.choice3Route);
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.fieldWidth = 0;
            break;

            case CutscenePart.PartType.Movement:
                part.moveScript = EditorGUILayout.ObjectField("Move Script: ", part.moveScript, typeof(Move)) as Move;

                GUILayout.BeginHorizontal();
                part.chooseMoveDirection = EditorGUILayout.Toggle("Change Direction", part.chooseMoveDirection);
                if (part.chooseMoveDirection)
                {
                    part.movementDirection = (MoveScripts.Direction)EditorGUILayout.EnumPopup(part.movementDirection);
                }
                GUILayout.EndHorizontal();

                
                EditorGUIUtility.fieldWidth = 120;
                GUILayout.BeginHorizontal();
                part.moveType = (CutscenePart.MoveType)EditorGUILayout.EnumPopup("Movement Type: ", part.moveType);
                if (part.moveType == (CutscenePart.MoveType.ToColliderTag))
                {
                    part.colliderTag = EditorGUILayout.TextField(part.colliderTag);
                }
                else if (part.moveType == (CutscenePart.MoveType.ToSpaces))
                {
                    part.spacesToMove = EditorGUILayout.IntField(part.spacesToMove);
                }
                GUILayout.EndHorizontal();
                EditorGUIUtility.fieldWidth = 0;
            break;

            case CutscenePart.PartType.Animation:
                part.animationActor = EditorGUILayout.ObjectField("Animation Actor Move Script: ", part.animationActor, typeof(Move)) as Move;
                if (part.animationActor != null)
                {
                    part.animationPlay = EditorGUILayout.Popup("Animation To Play: ", part.animationPlay, part.animationActor.animationSet.GetAnimList());
                }
            break;

            case CutscenePart.PartType.Battle:
                //TODO
                part.trainerLogic = EditorGUILayout.ObjectField("Trainer Logic Script: ", part.trainerLogic, typeof(TrainerLogic)) as TrainerLogic;
                
                part.postBattleCutscene = EditorGUILayout.Toggle("Post Battle Cutscene: ", part.postBattleCutscene);
                EditorGUIUtility.fieldWidth = 120;
                EditorGUIUtility.labelWidth = 80;
                GUILayout.BeginHorizontal();
                if (part.postBattleCutscene)
                {
                    part.cutsceneAfterBattle = EditorGUILayout.ObjectField("Cutscene: ", part.cutsceneAfterBattle, typeof(Cutscene)) as Cutscene;
                    EditorGUIUtility.labelWidth = 60;
                    part.routeAfterBattle = EditorGUILayout.TextField("Route: ", part.routeAfterBattle);
                }
                GUILayout.EndHorizontal();
                EditorGUIUtility.fieldWidth = 0;
                EditorGUIUtility.labelWidth = 0;
                if (GUILayout.Button("Get Trainer Info"))
                {
                    part.trainerLogic = cutscene.GetComponent<TrainerLogic>();
                    part.postBattleCutscene = true;
                    part.cutsceneAfterBattle = cutscene;

                }
            break;
            
            case CutscenePart.PartType.Wait:
                part.waitTime = EditorGUILayout.FloatField("Wait Time (In Seconds): ", part.waitTime);
            break;

            case CutscenePart.PartType.HealParty:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.SetSpawn:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.ChangeScene:
                SerializedProperty partSelected = serializedObject.FindProperty("parts");
                part.newSceneType = (CutscenePart.NewSceneType)EditorGUILayout.EnumPopup("Scene Change Type: ", part.newSceneType);
                if (part.newSceneType == CutscenePart.NewSceneType.ToEntrance || part.newSceneType == CutscenePart.NewSceneType.ToPosition)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.PropertyField(partSelected.GetArrayElementAtIndex(list.index).FindPropertyRelative("newScene"), true);
                }
                if (part.newSceneType == CutscenePart.NewSceneType.ToEntrance)
                {
                    GUILayout.Space(5);
                    part.newSceneEntranceIndex = EditorGUILayout.IntField("Entrance ID: ", part.newSceneEntranceIndex);
                }
                else if (part.newSceneType == CutscenePart.NewSceneType.ToPosition)
                {
                    GUILayout.Space(5);
                    part.newScenePosition = EditorGUILayout.Vector2Field("New Position: ", part.newScenePosition);
                    part.newSceneDirection = (MoveScripts.Direction)EditorGUILayout.EnumPopup("New Direction: ",part.newSceneDirection);
                }
            break;

            case CutscenePart.PartType.ChangeCameraFollow:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.NewCutscene:
                //TODO
                part.newCutsceneType = (CutscenePart.NewCutsceneType)EditorGUILayout.EnumPopup("Cutscene Type: ", part.newCutsceneType);
                switch (part.newCutsceneType)
                {
                    case CutscenePart.NewCutsceneType.Global:
                        //part.cutsceneGlobalFind = EditorGUILayout.TextField(" ", part.routeAfterBattle);
                        part.cutsceneGlobal = EditorGUILayout.ObjectField("Cutscene Script: ", part.cutsceneGlobal, typeof(CutsceneScript)) as CutsceneScript;
                        part.cutsceneRoute = EditorGUILayout.TextField("Cutscene Route: ", part.cutsceneRoute);
                    break;
                    
                    case CutscenePart.NewCutsceneType.SceneBased:
                        part.cutsceneIndex = EditorGUILayout.IntField("SceneAttributes Index: ", part.cutsceneIndex);
                        part.cutsceneRoute = EditorGUILayout.TextField("Cutscene Route: ", part.cutsceneRoute);
                    break;

                    case CutscenePart.NewCutsceneType.Local:
                        //part.cutsceneGlobalFind = EditorGUILayout.TextField(" ", part.routeAfterBattle);
                        part.cutsceneLocal = EditorGUILayout.ObjectField("Cutscene: ", part.cutsceneLocal, typeof(Cutscene)) as Cutscene;
                        part.cutsceneRoute = EditorGUILayout.TextField("Cutscene Route: ", part.cutsceneRoute);
                    break;
                }
            break;

            case CutscenePart.PartType.ObtainItem:
                EditorGUIUtility.fieldWidth = 120;
                EditorGUIUtility.labelWidth = 80;
                GUILayout.BeginHorizontal();
                part.itemObtained = EditorGUILayout.ObjectField("Item: ", part.itemObtained, typeof(Item)) as Item;
                part.itemAmount = EditorGUILayout.IntField("Amount: ", part.itemAmount);
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.fieldWidth = 0;
            break;

            case CutscenePart.PartType.ObtainLunen:
                EditorGUIUtility.fieldWidth = 120;
                EditorGUIUtility.labelWidth = 80;
                GUILayout.BeginHorizontal();
                part.lunenObtained = EditorGUILayout.ObjectField("Lunen: ", part.lunenObtained, typeof(Lunen)) as Lunen;
                part.lunenLevel = EditorGUILayout.IntField("Level: ", part.lunenLevel);
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.fieldWidth = 0;
            break;

            case CutscenePart.PartType.SetAsCollected:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.SetPanel:
                part.panelSelect = (CanvasCollection.UIState)EditorGUILayout.EnumPopup("Panel: ",part.panelSelect);
                part.panelState = (UITransition.State)EditorGUILayout.EnumPopup("Set To: ", part.panelState);
            break;

            case CutscenePart.PartType.CheckBattleOver:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            

            case CutscenePart.PartType.ChangeRoute:
                part.newRoute = EditorGUILayout.TextField("Change Route To: ", part.newRoute);
            break;
        }
    }
    */

    void GuiLine( int i_height = 1 )

   {

       Rect rect = EditorGUILayout.GetControlRect(false, i_height );

       rect.height = i_height;

       EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

   }
}
