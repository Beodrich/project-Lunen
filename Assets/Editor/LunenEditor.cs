//REMEMBER TO ADD TO BOTH EDITORS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using MyBox;

[CustomEditor(typeof(Lunen))]
public class LunenEditor : Editor
{
    Lunen lunen;
    SerializedProperty scene;
    public ReorderableList list = null;

    private void OnEnable()
    {
        lunen = (Lunen)target;
        scene = serializedObject.FindProperty("LearnedActions");
        list = new ReorderableList(serializedObject, scene)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true,
            drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Learned Actions");
            },
            drawElementCallback = DrawListItems,
        };
    }


    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("animationSet"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Elements"), true);
        GUILayout.Space(5);
        list.DoLayoutList();
        GUILayout.Space(5);

        //EditorGUILayout.LabelField();
        lunen.Evolves = EditorGUILayout.Toggle("Evolves?", lunen.Evolves);
        if (lunen.Evolves)
        {
            GUILayout.BeginHorizontal();
            lunen.EvolutionLevel = EditorGUILayout.IntField("Level: ", lunen.EvolutionLevel);
            lunen.EvolutionLunen = (Lunen)EditorGUILayout.ObjectField(lunen.EvolutionLunen, typeof(Lunen), false);
            GUILayout.EndHorizontal();
        }
        

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Health"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Attack"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Defense"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Speed"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AffinityCost"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CatchRate"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CooldownTime"), true);


        GUILayout.Space(20);

        
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(lunen);
        }
        
    }

     void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
            
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

        // Create a property field and label field for each property. 

        // The 'mobs' property. Since the enum is self-evident, I am not making a label field for it. 
        // The property field for mobs (width 100, height of a single line)

        EditorGUI.LabelField(new Rect(rect.x + 0, rect.y, 40, EditorGUIUtility.singleLineHeight), "Level");

        EditorGUI.PropertyField(
            new Rect(rect.x + 80, rect.y+1, 200, EditorGUIUtility.singleLineHeight), 
            element.FindPropertyRelative("action"),
            GUIContent.none
        ); 
        
        EditorGUI.PropertyField(
            new Rect(rect.x + 40, rect.y+1, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("level"),
            GUIContent.none
        ); 
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
