/*
//REMEMBER TO ADD TO BOTH EDITORS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using MyBox;

[CustomEditor(typeof(DialogueReplace))]
public class DialogueReplaceEditor : Editor
{
    DialogueReplace replace;

    private void OnEnable()
    {
        replace = (DialogueReplace)target;
    }


    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        EditorStyles.textField.wordWrap = true;
        

        GUIStyle fontStyle = new GUIStyle( GUI.skin.button );
        fontStyle.alignment = TextAnchor.MiddleLeft;

        replace.searchString = EditorGUILayout.TextField("Search String: ", replace.searchString);

        replace.trigger = (StoryTrigger)EditorGUILayout.ObjectField("Story Trigger: ", replace.trigger, typeof(StoryTrigger), false);
        if (replace.trigger != null)
        {
            int part = replace.trigger.GetTriggerPartIndex(replace.triggerPart);
            if (part == -1) part = 0;
            string lastTriggerPart = replace.triggerPart;
            replace.triggerPart = replace.trigger.triggerParts[EditorGUILayout.Popup(part, replace.trigger.GetTriggerPartList())].title;
            if (lastTriggerPart != replace.triggerPart)
            {
                part = replace.trigger.GetTriggerPartIndex(replace.triggerPart);
            }
            
        }


        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(replace);
        }
        
    }

    void GuiLine( int i_height = 1 )

   {

       Rect rect = EditorGUILayout.GetControlRect(false, i_height );

       rect.height = i_height;

       EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

   }
}
*/