using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_ChangeRoute : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.ChangeRoute;
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

    public string newRoute;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.battleSetup.CutsceneChangeInternal(sr.battleSetup.CutsceneFindRoute(newRoute));
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_ChangeRoute _cp = (CutPart_ChangeRoute)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        newRoute = _cp.newRoute;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            newRoute = EditorGUILayout.TextField("Change Route To: ", newRoute);
        }
    #endif
}
