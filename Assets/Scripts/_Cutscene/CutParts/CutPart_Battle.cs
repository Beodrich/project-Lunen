using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Battle : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Battle;
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

    public TrainerLogic trainerLogic;
    public bool postBattleCutscene;
    public Cutscene cutsceneAfterBattle;
    public string routeAfterBattle;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        if (postBattleCutscene)
        {
            sr.battleSetup.cutsceneAfterBattle = new PackedCutscene(cutsceneAfterBattle);
            sr.battleSetup.cutsceneAfterBattleRoute = sr.battleSetup.CutsceneFindRoute(sr.battleSetup.cutsceneAfterBattle, routeAfterBattle);
        }
        if (!trainerLogic.defeated && !trainerLogic.engaged)
        {
            trainerLogic.StartTrainerBattle();
        }
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Battle _cp = (CutPart_Battle)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        trainerLogic = _cp.trainerLogic;
        postBattleCutscene = _cp.postBattleCutscene;
        cutsceneAfterBattle = _cp.cutsceneAfterBattle;
        routeAfterBattle = _cp.routeAfterBattle;
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
            trainerLogic = EditorGUILayout.ObjectField("Trainer Logic Script: ", trainerLogic, typeof(TrainerLogic)) as TrainerLogic;
                
            postBattleCutscene = EditorGUILayout.Toggle("Post Battle Cutscene: ", postBattleCutscene);
            EditorGUIUtility.fieldWidth = 120;
            EditorGUIUtility.labelWidth = 80;
            GUILayout.BeginHorizontal();
            if (postBattleCutscene)
            {
                cutsceneAfterBattle = EditorGUILayout.ObjectField("Cutscene: ", cutsceneAfterBattle, typeof(Cutscene)) as Cutscene;
                EditorGUIUtility.labelWidth = 60;
                routeAfterBattle = EditorGUILayout.TextField("Route: ", routeAfterBattle);
            }
            GUILayout.EndHorizontal();
            EditorGUIUtility.fieldWidth = 0;
            EditorGUIUtility.labelWidth = 0;
        }
    #endif
}
