using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_SetPanel : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.SetPanel;
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

    public CanvasCollection.UIState panelSelect;
    public UITransition.State panelState;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        if (panelState == UITransition.State.Enable)
        {
            switch(panelSelect)
            {
                default:
                    sr.canvasCollection.OpenState(panelSelect);
                    sr.battleSetup.AdvanceCutscene();
                break;
                case CanvasCollection.UIState.Party:
                    sr.canvasCollection.partyPanelOpenForBattle = true;
                    sr.canvasCollection.OpenPartyWindow();
                    //CutsceneStartLite(new PackedCutscene(sr.sceneAttributes.sceneCutscenes[cutsceneIndex]), cutsceneRoute, -1);
                break;
                case CanvasCollection.UIState.Battle:
                    sr.canvasCollection.OpenState(panelSelect);
                    sr.battleSetup.AdvanceCutscene();
                break;
            }
        }
        else
        {
            switch(panelSelect)
            {
                default:
                    sr.canvasCollection.CloseState(panelSelect);
                    sr.battleSetup.AdvanceCutscene();
                break;
                case CanvasCollection.UIState.Dialogue:
                    sr.canvasCollection.CloseState(panelSelect);
                    sr.battleSetup.SubmitPressed();
                    //sr.battleSetup.AdvanceCutscene();
                break;
                case CanvasCollection.UIState.Battle:
                    sr.canvasCollection.CloseState(panelSelect);
                    sr.battleSetup.AdvanceCutscene();
                break;
            }
        }
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_SetPanel _cp = (CutPart_SetPanel)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        panelSelect = _cp.panelSelect;
        panelState = _cp.panelState;
    }

    public void GetTitle()
    {
        string start = "[" + _type + "]";
        string context = _title;
        if (_title == "")
        {
            context = "Set " + panelSelect.ToString() + " Panel To " + panelState.ToString();
        }
        _name = (start + " " + context);
    }

    #if UNITY_EDITOR
        public void DrawInspectorPart(Cutscene cutscene = null, CutsceneScript cutsceneScript = null)
        {
            panelSelect = (CanvasCollection.UIState)EditorGUILayout.EnumPopup("Panel: ",panelSelect);
            panelState = (UITransition.State)EditorGUILayout.EnumPopup("Set To: ", panelState);
        }
    #endif
}
