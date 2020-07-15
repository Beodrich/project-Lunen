using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Wait : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Wait;
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

    public float waitTime;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.battleSetup.waitTime = waitTime;
        sr.battleSetup.StartCoroutine(sr.battleSetup.cutsceneWait(sr.battleSetup.transform));
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Wait _cp = (CutPart_Wait)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        waitTime = _cp.waitTime;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart()
        {
            waitTime = EditorGUILayout.FloatField("Wait Time (In Seconds): ", waitTime);
        }
    #endif
}
