using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Destroy : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Destroy;
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

    public GameObject destroyObject;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.battleSetup.DestroyAnObject(destroyObject);
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Destroy _cp = (CutPart_Destroy)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        destroyObject = _cp.destroyObject;
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
            destroyObject = (GameObject)EditorGUILayout.ObjectField("Destroy This Object: ", destroyObject, typeof(GameObject), true);
        }
    #endif
}
