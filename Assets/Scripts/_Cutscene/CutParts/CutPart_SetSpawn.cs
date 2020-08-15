using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_SetSpawn : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.SetSpawn;
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

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        sr.battleSetup.respawnScene = sr.battleSetup.lastOverworld;
        sr.battleSetup.respawnLocation = sr.playerLogic.gameObject.transform.position;
        sr.battleSetup.respawnDirection = sr.playerLogic.move.lookDirection;
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_SetSpawn _cp = (CutPart_SetSpawn)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;
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
        }
    #endif
}
