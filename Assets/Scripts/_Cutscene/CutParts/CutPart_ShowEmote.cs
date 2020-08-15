using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_ShowEmote : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.ShowEmote;
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

    public EmoteAnim emote;
    public GameObject sourceObject;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.battleSetup.CreateEmote(emote, sourceObject);
        
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_ShowEmote _cp = (CutPart_ShowEmote)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        emote = _cp.emote;
        sourceObject = _cp.sourceObject;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        if (_title == "")
        {
            if (emote != null)
            {
                context = "Play " + emote.name + " Emote";
            }
            
        }
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(SerializedProperty serializedProperty, Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            emote = (EmoteAnim)EditorGUILayout.ObjectField("Emote: ", emote, typeof(EmoteAnim), false);
            sourceObject = (GameObject)EditorGUILayout.ObjectField("Object Source: ", sourceObject, typeof(GameObject), true);
            
        }
    #endif
}
