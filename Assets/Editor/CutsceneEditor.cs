using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    Cutscene cutscene;
    public ReorderableList list = null;

    public bool foundCutscene;
    public bool lockEditor;

    private void OnEnable()
    {
        cutscene = (Cutscene)target;
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("parts"), true, false, true, true);
    }

    private void OnCutsceneSwitch()
    {
        //list = new ReorderableList(so, serializedObject.FindPro)
    }


    public override void OnInspectorGUI()
    {
        
        if (cutscene.showAllData)
        {
            base.OnInspectorGUI();
        }
        else
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            GUIStyle fontStyle = new GUIStyle( GUI.skin.button );
            fontStyle.alignment = TextAnchor.MiddleLeft;

            

            if (cutscene.parts.Count > 0)
            {
                
                int index = list.index;
                if (index < cutscene.parts.Count && index >= 0)
                {
                    GUILayout.Space(10);
                    GuiLine(3);
                    GUILayout.Space(5);
                    DrawPart(cutscene.parts[index]);
                    GUILayout.Space(5);
                    GuiLine(3);
                    GUILayout.Space(20);

                }
                
            }

            GUILayout.Label("Other Cutscene Settings: ", EditorStyles.boldLabel);
            cutscene.stopsBattle = EditorGUILayout.Toggle("Cutscene Stops Battle ", cutscene.stopsBattle);
            cutscene.showAllData = EditorGUILayout.Toggle("Show Original Data ", cutscene.showAllData);
            GUILayout.Space(20);
            //selectedPart = GUILayout.SelectionGrid(selectedPart, partNames.ToArray(), 1, fontStyle);
            
            

            if (GUI.changed)
            {
                
            }
        }
        
    }

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
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.Animation:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.Battle:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
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
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.ChangeCameraFollow:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.NewCutscene:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.ObtainItem:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.SetAsCollected:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.OpenPanel:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.ClosePanel:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.CheckBattleOver:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.ObtainLunen:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;

            case CutscenePart.PartType.ChangeRoute:
                //TODO
                GUILayout.Label("TODO", EditorStyles.boldLabel);
            break;
        }
    }

    void GuiLine( int i_height = 1 )

   {

       Rect rect = EditorGUILayout.GetControlRect(false, i_height );

       rect.height = i_height;

       EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

   }
}
