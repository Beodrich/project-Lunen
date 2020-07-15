using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Choice : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Choice;
    public string _title = ("New " + _type.ToString());
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

    public bool useChoice1;
    public string choice1Text = "Choice 1";
    public string choice1Route;

    public bool useChoice2;
    public string choice2Text = "Choice 2";
    public string choice2Route;

    public bool useChoice3;
    public string choice3Text = "Choice 3";
    public string choice3Route;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.canvasCollection.UICollections[(int)CanvasCollection.UIState.Dialogue].SetPanelState("Choice Panel", UITransition.State.Enable);
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
        CutPart_Choice _cp = (CutPart_Choice)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        text = _cp.text;
    
        useChoice1 = _cp.useChoice1;
        choice1Text = _cp.choice1Text;
        choice1Route = _cp.choice1Route;

        useChoice2 = _cp.useChoice2;
        choice2Text = _cp.choice2Text;
        choice2Route = _cp.choice2Route;

        useChoice3 = _cp.useChoice3;
        choice3Text = _cp.choice3Text;
        choice3Route = _cp.choice3Route;
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
            GUILayout.Space(5);

            //GuiLine(2);
            GUILayout.Space(3);

            GUILayout.BeginVertical();
            useChoice1 = EditorGUILayout.Toggle("Use Choice 1: ", useChoice1);
            if (useChoice1)
            {
                choice1Text = EditorGUILayout.TextField(" Option Text:", choice1Text);
                choice1Route = EditorGUILayout.TextField(" Option Route:", choice1Route);
            }
            GUILayout.EndVertical();

            GUILayout.Space(3);
            //GuiLine(2);
            GUILayout.Space(3);

            GUILayout.BeginVertical();
            useChoice2 = EditorGUILayout.Toggle("Use Choice 2: ", useChoice2);
            if (useChoice2)
            {
                choice2Text = EditorGUILayout.TextField(" Option Text:", choice2Text);
                choice2Route = EditorGUILayout.TextField(" Option Route:", choice2Route);
            }
            GUILayout.EndVertical();

            GUILayout.Space(3);
            //GuiLine(2);
            GUILayout.Space(3);

            GUILayout.BeginVertical();
            useChoice3 = EditorGUILayout.Toggle("Use Choice 3: ", useChoice3);
            if (useChoice3)
            {
                choice3Text = EditorGUILayout.TextField(" Option Text:", choice3Text);
                choice3Route = EditorGUILayout.TextField(" Option Route:", choice3Route);
            }
            
            GUILayout.EndVertical();
        }
    #endif
}
