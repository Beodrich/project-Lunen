using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Dialogue : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Dialogue;
    public string _title = ("");
    public bool _startNextSimultaneous;

    public bool startNextSimultaneous
    {
        get => _startNextSimultaneous;
        set => _startNextSimultaneous = value;
    }

    public string listDisplay
    {
        get => _name;
    }

    public string partTitle
    {
        get => _title;
        set => _title = value;
    }

    public CutPartType cutPartType
    {
        get => _type;
    }

    //Unique Values

    public string text;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        bool next = true;
        if (sr.battleSetup.cutscenePart+1 == sr.battleSetup.lastCutscene.parts.Count)
        {
            next = false;
        }
        else if (sr.battleSetup.lastCutscene.parts[sr.battleSetup.cutscenePart+1].cutPartType != CutPartType.Dialogue && sr.battleSetup.lastCutscene.parts[sr.battleSetup.cutscenePart+1].cutPartType != CutPartType.Choice)
        {
            next = false;
        }
        sr.battleSetup.DialogueBoxPrepare(this, next);
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Dialogue _cp = (CutPart_Dialogue)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        text = _cp.text;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + text);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart()
        {
            GUILayout.Label("Cutscene Text: ");
            text = EditorGUILayout.TextArea(text);
        }
    #endif
}
