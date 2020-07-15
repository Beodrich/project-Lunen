using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;

[System.Serializable]
public class CutPart_SetAsCollected : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.SetAsCollected;
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

    public GuidComponent guidSet;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.battleSetup.GuidList.Add(guidSet.GetGuid());
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_SetAsCollected _cp = (CutPart_SetAsCollected)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        guidSet = _cp.guidSet;
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
            guidSet = EditorGUILayout.ObjectField("Guid Component: ", guidSet, typeof(GuidComponent)) as GuidComponent;
        }
    #endif
}
