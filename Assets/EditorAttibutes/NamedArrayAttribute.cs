 
using UnityEngine;
using System.Text.RegularExpressions;
 
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
 
// Defines an attribute that makes the array use enum values as labels.
// Use like this:
//      [NamedArray(typeof(eDirection))] public GameObject[] m_Directions;
 
public class NamedArrayAttribute : PropertyAttribute {
    public Type TargetEnum;
    public NamedArrayAttribute(Type TargetEnum) {
        this.TargetEnum = TargetEnum;
    }
}
 
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        // Properly configure height for expanded contents.
        return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Replace label with enum name if possible.
        try
        {
            var config = attribute as NamedArrayAttribute;
            var enum_names = Enum.GetNames(config.TargetEnum);
 
            var match = Regex.Match(property.propertyPath, "\\[(\\d)\\]", RegexOptions.RightToLeft);
            int pos = int.Parse(match.Groups[1].Value);
 
            // Make names nicer to read (but won't exactly match enum definition).
            var enum_label = ObjectNames.NicifyVariableName(enum_names[pos].ToLower());
            label = new GUIContent(enum_label);
        }
        catch
        {
            // keep default label
        }
        EditorGUI.PropertyField(position, property, label, property.isExpanded);
    }
}
#endif