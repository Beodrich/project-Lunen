using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CutPart_SetNewSprite : CutPart
{
    //Standard Values
    [SerializeField] public string _name = "";
    public static CutPartType _type = CutPartType.SetNewSprite;
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

    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;

    //Functions

    public void PlayPart (SetupRouter sr)
    {
        spriteRenderer.sprite = newSprite;
        sr.battleSetup.AdvanceCutscene();
    }

    public void Duplicate (CutPart cp)
    {
        CutPart_SetNewSprite _cp = (CutPart_SetNewSprite)cp;

        _title = _cp._title;
        _startNextSimultaneous = _cp.startNextSimultaneous;

        spriteRenderer = _cp.spriteRenderer;
        newSprite = _cp.newSprite;
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
            spriteRenderer = EditorGUILayout.ObjectField("Object Sprite To Change: ", spriteRenderer, typeof(SpriteRenderer)) as SpriteRenderer;
            newSprite = EditorGUILayout.ObjectField("New Sprite: ", newSprite, typeof(Sprite)) as Sprite;
        }
    #endif
}
