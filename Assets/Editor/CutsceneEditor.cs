//REMEMBER TO ADD TO BOTH EDITORS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using MyBox;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    Cutscene cutscene;
    CutPartType createCutType;
    public ReorderableList list = null;
    SerializedProperty scene;

    public bool foundCutscene;
    public bool lockEditor;
    public int index;

    private void OnEnable()
    {
        cutscene = (Cutscene)target;
        scene = serializedObject.FindProperty("parts");
        list = new ReorderableList(serializedObject, scene)
        {
            displayAdd = false,
            displayRemove = false,
            draggable = true,
            drawHeaderCallback = rect =>
            {
                cutscene.cutsceneName = EditorGUI.TextField(rect, cutscene.cutsceneName);
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

                DrawPart(cutscene.parts[index]);
                GUILayout.Space(5);
                GuiLine(3);
                GUILayout.Space(5);
                cutscene.stopsBattle = EditorGUILayout.Toggle("Cutscene Stops Battle: ", cutscene.stopsBattle);
                
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
            EditorSceneManager.MarkSceneDirty(cutscene.gameObject.scene);
        }
    }

    private void AddPart(int _index)
    {
        CutPart newPart = GetNewPart(createCutType);
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
        CutPart newPart = GetNewPart(cutscene.parts[_index].cutPartType);
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
        GuiLine(3);
        GUILayout.Space(5);
        part.partTitle = EditorGUILayout.TextField("Part Title: ", part.partTitle);

        GUILayout.Space(10);

        part.DrawInspectorPart();

        GUILayout.Space(5);
        part.startNextSimultaneous = EditorGUILayout.Toggle("Start Next Part Alongside: ", part.startNextSimultaneous);
    }

    private CutPart GetNewPart(CutPartType type)
    {
        switch (type)
        {
            default: return new CutPart_Dialogue();
            case CutPartType.Dialogue: return new CutPart_Dialogue();
            case CutPartType.Choice: return new CutPart_Choice();
            case CutPartType.ROUTE_START: return new CutPart_ROUTE_START();
            case CutPartType.END: return new CutPart_END();
            case CutPartType.Movement: return new CutPart_Movement();
            case CutPartType.Animation: return new CutPart_Animation();
            case CutPartType.Battle: return new CutPart_Battle();
            case CutPartType.Wait: return new CutPart_Wait();
            case CutPartType.HealParty: return new CutPart_HealParty();
            case CutPartType.BLANK: return new CutPart_BLANK();
            case CutPartType.SetSpawn: return new CutPart_SetSpawn();
            case CutPartType.ChangeRoute: return new CutPart_ChangeRoute();
            case CutPartType.ChangeScene: return new CutPart_ChangeScene();
            case CutPartType.ChangeCameraFollow: return new CutPart_ChangeCameraFollow();
            case CutPartType.NewCutscene: return new CutPart_NewCutscene();
            case CutPartType.ObtainItem: return new CutPart_ObtainItem();
            case CutPartType.ObtainLunen: return new CutPart_ObtainLunen();
            case CutPartType.SetAsCollected: return new CutPart_SetAsCollected();
            case CutPartType.SetPanel: return new CutPart_SetPanel();
            case CutPartType.CheckBattleOver: return new CutPart_CheckBattleOver();
            case CutPartType.CaptureWildLunen: return new CutPart_CaptureWildLunen();
            case CutPartType.Destroy: return new CutPart_Destroy();
            case CutPartType.SetNewSprite: return new CutPart_SetNewSprite();
        }
    }

    void GuiLine( int i_height = 1 )
    {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height );

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
    }

}
