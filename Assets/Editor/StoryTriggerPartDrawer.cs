using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TriggerPart))]
public class StoryTriggerPartDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUIUtility.labelWidth = 70;
            label.text = "Title: ";
            EditorGUI.PropertyField(new Rect(position.x,position.y+20,position.width,position.height), property.FindPropertyRelative("title"), label, true);
            label.text = "Type: ";
            EditorGUI.PropertyField(new Rect(position.x,position.y+40,position.width,position.height), property.FindPropertyRelative("type"), label, true);

            TriggerTypes type = (TriggerTypes)property.FindPropertyRelative("type").enumValueIndex;

            switch (type)
            {
                case TriggerTypes.Bool:
                    label.text = "Default: "; EditorGUI.PropertyField(new Rect(position.x,position.y+60,position.width,position.height), property.FindPropertyRelative("defaultBool"), label, true); EditorGUI.LabelField(new Rect(position.x+100,position.y+60, position.width/2, position.height), "(" + property.FindPropertyRelative("defaultBool").boolValue.ToString() + ")"); 
                    label.text = "Current: "; EditorGUI.PropertyField(new Rect(position.x,position.y+80,position.width,position.height), property.FindPropertyRelative("triggerBool"), label, true); EditorGUI.LabelField(new Rect(position.x+100,position.y+80, position.width/2, position.height), "(" + property.FindPropertyRelative("triggerBool").boolValue.ToString() + ")"); 
                break;
                case TriggerTypes.Int:
                    label.text = "Default: "; EditorGUI.PropertyField(new Rect(position.x,position.y+60,position.width,position.height), property.FindPropertyRelative("defaultInt"), label, true);
                    label.text = "Current: "; EditorGUI.PropertyField(new Rect(position.x,position.y+80,position.width,position.height), property.FindPropertyRelative("triggerInt"), label, true);
                break;
                case TriggerTypes.Float:
                    label.text = "Default: "; EditorGUI.PropertyField(new Rect(position.x,position.y+60,position.width,position.height), property.FindPropertyRelative("defaultFloat"), label, true);
                    label.text = "Current: "; EditorGUI.PropertyField(new Rect(position.x,position.y+80,position.width,position.height), property.FindPropertyRelative("triggerFloat"), label, true);
                break;
                case TriggerTypes.Double:
                    label.text = "Default: "; EditorGUI.PropertyField(new Rect(position.x,position.y+60,position.width,position.height), property.FindPropertyRelative("defaultDouble"), label, true);
                    label.text = "Current: "; EditorGUI.PropertyField(new Rect(position.x,position.y+80,position.width,position.height), property.FindPropertyRelative("triggerDouble"), label, true);
                break;
                case TriggerTypes.String:
                    label.text = "Default: "; EditorGUI.PropertyField(new Rect(position.x,position.y+60,position.width,position.height), property.FindPropertyRelative("defaultString"), label, true);
                    label.text = "Current: "; EditorGUI.PropertyField(new Rect(position.x,position.y+80,position.width,position.height), property.FindPropertyRelative("triggerString"), label, true);
                break;
            }
            EditorGUI.LabelField(new Rect(position.x,position.y+110, position.width, position.height), "Notes");
            EditorGUI.PropertyField(new Rect(position.x,position.y+110, position.width, position.height*5), property.FindPropertyRelative("notes"), GUIContent.none);
            EditorGUIUtility.labelWidth = 0;
        }
        
        EditorGUI.EndProperty();
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return ((property.isExpanded) ? 200 : 16);
    }
}
