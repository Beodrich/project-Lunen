using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_NewCutscene : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.NewCutscene;
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
    public NewCutsceneType newCutsceneType;
    public int cutsceneIndex;
    public string cutsceneRoute;
    public CutsceneScript cutsceneGlobal;
    public Cutscene cutsceneLocal;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        switch(newCutsceneType)
        {
            case NewCutsceneType.Global:
                sr.battleSetup.CutsceneStartLite(new PackedCutscene(cutsceneGlobal), cutsceneRoute);
            break;

            case NewCutsceneType.SceneBased:
                if (sr.sceneAttributes.sceneCutscenes.Count == 0)
                {
                    sr.battleSetup.AdvanceCutscene();
                }
                else
                {
                    int nextcutscene = cutsceneIndex;
                    if (sr.sceneAttributes.sceneCutscenes.Count <= nextcutscene) nextcutscene = 0;
                    sr.battleSetup.CutsceneStartLite(new PackedCutscene(sr.sceneAttributes.sceneCutscenes[nextcutscene]), cutsceneRoute);
                }
                
            break;

            case NewCutsceneType.Local:
                sr.battleSetup.CutsceneStartLite(new PackedCutscene(cutsceneLocal), cutsceneRoute);
            break;

            default:
                Debug.Log("[ERROR] NULL CUTSCENE");
                sr.battleSetup.AdvanceCutscene();
            break;
            
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_NewCutscene _cp = (CutPart_NewCutscene)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        newCutsceneType = _cp.newCutsceneType;
        cutsceneIndex = _cp.cutsceneIndex;
        cutsceneRoute = _cp.cutsceneRoute;
        cutsceneGlobal = _cp.cutsceneGlobal;
        cutsceneLocal = _cp.cutsceneLocal;
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
            newCutsceneType = (NewCutsceneType)EditorGUILayout.EnumPopup("Cutscene Type: ", newCutsceneType);
            switch (newCutsceneType)
            {
                case NewCutsceneType.Global:
                    //cutsceneGlobalFind = EditorGUILayout.TextField(" ", routeAfterBattle);
                    cutsceneGlobal = EditorGUILayout.ObjectField("Cutscene Script: ", cutsceneGlobal, typeof(CutsceneScript)) as CutsceneScript;
                    cutsceneRoute = EditorGUILayout.TextField("Cutscene Route: ", cutsceneRoute);
                break;
                
                case NewCutsceneType.SceneBased:
                    cutsceneIndex = EditorGUILayout.IntField("SceneAttributes Index: ", cutsceneIndex);
                    cutsceneRoute = EditorGUILayout.TextField("Cutscene Route: ", cutsceneRoute);
                break;

                case NewCutsceneType.Local:
                    //cutsceneGlobalFind = EditorGUILayout.TextField(" ", routeAfterBattle);
                    cutsceneLocal = EditorGUILayout.ObjectField("Cutscene: ", cutsceneLocal, typeof(Cutscene)) as Cutscene;
                    cutsceneRoute = EditorGUILayout.TextField("Cutscene Route: ", cutsceneRoute);
                break;
            }
        }
    #endif
}
