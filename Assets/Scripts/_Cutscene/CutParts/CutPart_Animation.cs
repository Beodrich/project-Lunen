using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_Animation : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.Animation;
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

    public Move animationActor;
    public int animationPlay;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        animationActor.animHijack = true;
        animationActor.animTime = 0;
        animationActor.animationType = animationActor.animationSet.GetAnimationName(animationPlay);
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_Animation _cp = (CutPart_Animation)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        animationActor = _cp.animationActor;
        animationPlay = _cp.animationPlay;
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
            animationActor = (Move)EditorGUILayout.ObjectField("Animation Actor Move Script: ", animationActor, typeof(Move), true);
            if (animationActor != null)
            {
                animationPlay = EditorGUILayout.Popup("Animation To Play: ", animationPlay, animationActor.animationSet.GetAnimList());
            }
        }
    #endif
}
