﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_HealParty : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.HealParty;
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
        for (int i = 0; i < sr.battleSetup.PlayerLunenTeam.Count; i++)
        {
            sr.battleSetup.PlayerLunenTeam[i].GetComponent<Monster>().Health.z = sr.battleSetup.PlayerLunenTeam[i].GetComponent<Monster>().GetMaxHealth();
        }
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_HealParty _cp = (CutPart_HealParty)cp;

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
        public void DrawInspectorPart(Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            
        }
    #endif
}
