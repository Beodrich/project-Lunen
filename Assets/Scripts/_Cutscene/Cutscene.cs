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

    public static CutPart GetNewPart(CutPartType type)
    {
        switch (type)
        {
            case CutPartType.Dialogue: return new CutPart_Dialogue();
            case CutPartType.Choice: return new CutPart_Choice();
            case CutPartType.RouteStart: return new CutPart_RouteStart();
            case CutPartType.End: return new CutPart_End();
            case CutPartType.Movement: return new CutPart_Movement();
            case CutPartType.Animation: return new CutPart_Animation();
            case CutPartType.Battle: return new CutPart_Battle();
            case CutPartType.Wait: return new CutPart_Wait();
            case CutPartType.HealParty: return new CutPart_HealParty();
            case CutPartType.Blank: return new CutPart_Blank();
            case CutPartType.SetSpawn: return new CutPart_SetSpawn();
            case CutPartType.ChangeRoute: return new CutPart_ChangeRoute();
            case CutPartType.ChangeScene: return new CutPart_ChangeScene();
            case CutPartType.ChangeCameraFollow: return new CutPart_ChangeCameraFollow();
            case CutPartType.NewCutscene: return new CutPart_NewCutscene();
            case CutPartType.ObtainItem: return new CutPart_ObtainItem();
            case CutPartType.ObtainLunen: return new CutPart_ObtainLunen();
            case CutPartType.SetAsCollected: return new CutPart_SetAsCollected();
            case CutPartType.SetPanel: return new CutPart_SetPanel();
            case CutPartType.CheckBattleOver: return new CutPart_CheckBattleOver();
            case CutPartType.CaptureWildLunen: return new CutPart_CaptureWildLunen();
            case CutPartType.Destroy: return new CutPart_Destroy();
            case CutPartType.SetNewSprite: return new CutPart_SetNewSprite();
            case CutPartType.StoryTriggerBranch: return new CutPart_StoryTriggerBranch();
            case CutPartType.StoryTriggerSet: return new CutPart_StoryTriggerSet();
        }
        return null;
    }

    public string[] GetListOfParts()
    {
        List<string> partList = new List<string>();
        foreach(CutPart part in parts) partList.Add(part.partTitle);
        return partList.ToArray();
    }

    public string[] GetListOfRoutes()
    {
        List<string> partList = new List<string>();
        foreach(CutPart part in parts)
        {
            if (part.cutPartType == CutPartType.RouteStart) partList.Add(part.partTitle);
        }
        return partList.ToArray();
    }

    public string GetRouteString(int index)
    {
        List<CutPart> partList = new List<CutPart>();
        foreach(CutPart part in parts)
        {
            if (part.cutPartType == CutPartType.RouteStart) partList.Add(part);
        }
        if (index < partList.Count)
        {
            return partList[index].partTitle;
        }
        else
        {
            return "Error!";
        }
    }

    public int GetRouteIndex(string index)
    {
        List<CutPart> partList = new List<CutPart>();
        foreach(CutPart part in parts)
        {
            if (part.cutPartType == CutPartType.RouteStart) partList.Add(part);
        }
        for (int i = 0; i < partList.Count; i++)
        {
            if (partList[i].partTitle == index) return i;
        }
        return -1;
    }
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