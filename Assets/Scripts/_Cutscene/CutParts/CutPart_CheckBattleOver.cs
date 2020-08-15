﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_CheckBattleOver : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.CheckBattleOver;
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
        if (sr.director.PlayerLunenAlive.Count == 0) sr.battleSetup.PlayerLose();
        else if (sr.director.EnemyLunenAlive.Count == 0) sr.battleSetup.PlayerWin();
        else
        {
            sr.canvasCollection.OpenState(CanvasCollection.UIState.Battle);
            sr.canvasCollection.EnsureValidTarget();
            sr.battleSetup.AdvanceCutscene();
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_CheckBattleOver _cp = (CutPart_CheckBattleOver)cp;

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
