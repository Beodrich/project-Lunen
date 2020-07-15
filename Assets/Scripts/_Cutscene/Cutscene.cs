using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Malee.List;

public class Cutscene : MonoBehaviour
{
    [Space(10)]
    [Header("Edit Cutscenes With Window->Cutscene Editor!")]
    
    public string cutsceneName;
    public bool stopsBattle;
    [SerializeReference] public List<CutPart> parts;
}

[System.Serializable]
public class PackedCutscene
{
    public string cutsceneName;
    public bool stopsBattle;
    [SerializeReference] public List<CutPart> parts;

    public PackedCutscene(string _cutsceneName, List<CutPart> _parts)
    {
        cutsceneName = _cutsceneName;
        parts = _parts;
    }

    public PackedCutscene(Cutscene cutscene)
    {
        cutsceneName = cutscene.cutsceneName;
        parts = cutscene.parts;
    }

    public PackedCutscene(CutsceneScript cutscene)
    {
        cutsceneName = cutscene.name;
        parts = cutscene.parts;
        stopsBattle = cutscene.stopsBattle;
    }
}