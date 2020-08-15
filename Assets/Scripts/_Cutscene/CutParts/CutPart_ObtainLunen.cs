using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_ObtainLunen : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.ObtainLunen;
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

    public Lunen lunenObtained;
    public int lunenLevel;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        GameObject go = sr.generateMonster.GenerateLunen(lunenObtained, lunenLevel);
        //go.GetComponent<Monster>().MonsterTeam = Director.Team.PlayerTeam;
        sr.battleSetup.PlayerLunenTeam.Add(go);
        go.transform.SetParent(sr.battleSetup.transform);
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_ObtainLunen _cp = (CutPart_ObtainLunen)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        lunenObtained = _cp.lunenObtained;
        lunenLevel = _cp.lunenLevel;
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
            lunenObtained = (Lunen)EditorGUILayout.ObjectField("Lunen: ", lunenObtained, typeof(Lunen), true);
            lunenLevel = EditorGUILayout.IntField("Level: ", lunenLevel);
    }
    #endif
}
