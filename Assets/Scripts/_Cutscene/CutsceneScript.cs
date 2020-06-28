using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "GameElements/Cutscene")]
public class CutsceneScript : ScriptableObject
{
    [Space(10)]
    [Header("Edit Cutscenes With Window->Cutscene Editor!")]
    
    public bool stopsBattle;
    public bool showAllData;
    public List<CutscenePart> parts;
}