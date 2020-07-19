using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "GameElements/Cutscene")]
public class CutsceneScript : ScriptableObject
{
    [Space(10)]
    [Header("Edit Cutscenes With Window->Cutscene Editor!")]
    
    public bool stopsBattle;
    [SerializeReference] public List<CutPart> parts;

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