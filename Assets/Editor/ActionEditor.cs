using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Action))]
public class ActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        /*
        Action nA = (Action)target;

        EditorGUILayout.LabelField("Basic Action Info", EditorStyles.boldLabel);
        nA.Name = EditorGUILayout.DelayedTextField("Name: ", nA.Name );
        nA.Type = (Types.Element)EditorGUILayout.EnumPopup("Type: ", nA.Type);
        nA.Turns = EditorGUILayout.IntField("Turns To Charge: ", nA.Turns);
        nA.Animation = (GameObject)EditorGUILayout.ObjectField("Animation: ", nA.Animation, typeof(GameObject), true);
        */
        base.OnInspectorGUI();
    }
}
