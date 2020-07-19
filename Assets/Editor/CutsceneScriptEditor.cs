//REMEMBER TO ADD TO BOTH EDITORS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using MyBox;

[CustomEditor(typeof(CutsceneScript))]
public class CutsceneScriptEditor : Editor
{
    CutsceneScript cutscene;
    CutPartType createCutType;
    public ReorderableList list = null;
    SerializedProperty scene;

    public bool foundCutscene;
    public bool lockEditor;
    public int index;

    private void OnEnable()
    {
        cutscene = (CutsceneScript)target;
        scene = serializedObject.FindProperty("parts");
        list = new ReorderableList(serializedObject, scene)
        {
            displayAdd = false,
            displayRemove = false,
            draggable = true,
            drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Cutscene Parts");
            }
        };
        index = 0;
        //scene = serializedObject.FindProperty("newScene");
    }


    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        list.DoLayoutList();
        EditorStyles.textField.wordWrap = true;

        GUIStyle fontStyle = new GUIStyle( GUI.skin.button );
        fontStyle.alignment = TextAnchor.MiddleLeft;

        if (cutscene != null)
        {
            if (cutscene.parts == null) cutscene.parts = new List<CutPart>();

        if (cutscene.parts.Count > 0)
        {
            index = list.index;
            if (index < cutscene.parts.Count && index >= 0)
            {
                GUILayout.Space(-10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("New Part"))
                {
                    AddPart(index+1);
                }
                createCutType = (CutPartType)EditorGUILayout.EnumPopup(createCutType);
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Duplicate"))
                {
                    DuplicatePart(index);
                }
                if (GUILayout.Button("Delete"))
                {
                    DeletePart(index);
                }
                GUILayout.EndHorizontal();

                //Draw Part Starts Here

                if (index < cutscene.parts.Count) DrawPart(cutscene.parts[index]);
                
                
            }
            else
            {
                GUILayout.Space(-10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("New Part"))
                {
                    AddPart(index+1);
                }
                createCutType = (CutPartType)EditorGUILayout.EnumPopup(createCutType);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            
        }
        else
        {
            GUILayout.Space(-10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New Part"))
            {
                AddPart(index+1);
            }
            createCutType = (CutPartType)EditorGUILayout.EnumPopup(createCutType);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        GUILayout.Space(5);
        GUIScripts.GuiLine(3);
        GUILayout.Space(5);
        EditorGUILayout.LabelField("--- Cutscene Settings", EditorStyles.boldLabel);
        cutscene.stopsBattle = EditorGUILayout.Toggle("Cutscene Stops Battle: ", cutscene.stopsBattle);

        GUILayout.Space(20);
        serializedObject.ApplyModifiedProperties();
        //selectedPart = GUILayout.SelectionGrid(selectedPart, partNames.ToArray(), 1, fontStyle);
        
        

        if (GUI.changed)
        {
            if (index >= 0 && index < cutscene.parts.Count)
            {
                cutscene.parts[index].GetTitle();
            }
            EditorUtility.SetDirty(cutscene);
            //EditorSceneManager.MarkSceneDirty(cutscene.gameObject.scene);
        }
        }

        
    }

    private void AddPart(int _index)
    {
        CutPart newPart = Cutscene.GetNewPart(createCutType);
        AddPart(newPart, _index);
    }

    private void AddPart(CutPart _newPart, int _index)
    {
        _newPart.GetTitle();
        //Debug.Log("_Index = " + _index + " | Parts = " + cutscene.parts.Count );
        if (_index >= cutscene.parts.Count || _index < 0)
        {
            cutscene.parts.Add(_newPart);
        }
        else
        {
            cutscene.parts.Insert(_index, _newPart);
        }
    }

    private void DuplicatePart(int _index)
    {
        CutPart newPart = Cutscene.GetNewPart(cutscene.parts[_index].cutPartType);
        newPart.Duplicate(cutscene.parts[_index]);
        AddPart(newPart, _index + 1);
    }

    private void DeletePart(int _index)
    {
        cutscene.parts.RemoveAt(_index);
        if (index != 0) index--;
    }

    private void DrawPart(CutPart part)
    {
        GUILayout.Space(10);
        GUIScripts.GuiLine(3);
        GUILayout.Space(5);
        part.partTitle = GUIScripts.TextField(part.partTitle, "Part Title Here (Blank Fills Automatically)");

        GUILayout.Space(5);
        GUIScripts.GuiLine(1);
        GUILayout.Space(5);

        part.DrawInspectorPart(null, cutscene);
    }

}
