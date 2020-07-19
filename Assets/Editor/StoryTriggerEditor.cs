//REMEMBER TO ADD TO BOTH EDITORS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using MyBox;

[CustomEditor(typeof(StoryTrigger))]
public class StoryTriggerEditor : Editor
{
    StoryTrigger storyTrigger;
    public ReorderableList list = null;
    SerializedProperty scene;
    public int index;

    private void OnEnable()
    {
        storyTrigger = (StoryTrigger)target;
        scene = serializedObject.FindProperty("triggerParts");
        list = new ReorderableList(serializedObject, scene)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true,
            drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, storyTrigger.name);
            },
            drawElementCallback = DrawListItems,
            elementHeightCallback = ElementHeightCallback
        };
        index = 0;
        //scene = new PropertyField(property.FindPropertyRelative("newScene");
    }


    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        list.DoLayoutList();
        EditorStyles.textField.wordWrap = true;

        GUIStyle fontStyle = new GUIStyle( GUI.skin.button );
        fontStyle.alignment = TextAnchor.MiddleLeft;

        GUILayout.Space(20);
        serializedObject.ApplyModifiedProperties();
        //selectedPart = GUILayout.SelectionGrid(selectedPart, partNames.ToArray(), 1, fontStyle);
        
        

        if (GUI.changed)
        {
            EditorUtility.SetDirty(storyTrigger);
        }
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
            
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

        SerializedProperty elementName = element.FindPropertyRelative("title");
        string elementTitle = string.IsNullOrEmpty(elementName.stringValue)
            ? "New Trigger Variable"
            : "" + $"{elementName.stringValue}" + " (" + storyTrigger.GetTriggerValue(index).ToString() + ")";

        EditorGUI.PropertyField(
        new Rect(rect.x + 10f, rect.y, (EditorGUIUtility.currentViewWidth-75f), EditorGUIUtility.singleLineHeight), 
        element,
        label: new GUIContent(elementTitle),
        includeChildren: true
        );
    }

    private float ElementHeightCallback(int index)
    {
        //Gets the height of the element. This also accounts for properties that can be expanded, like structs.
        float propertyHeight =
            EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index), true);

        float spacing = EditorGUIUtility.singleLineHeight / 4;

        return propertyHeight + spacing;
    }

}


