using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateMonster))]
public class LunenToMonsterGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateMonster newMonster = (GenerateMonster)target;

        if (GUILayout.Button("Generate Monster"))
        {
            newMonster.Generate();
        }
    }
}
