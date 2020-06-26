using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "GameElements/Cutscene")]
public class CutsceneScript : ScriptableObject
{
    public bool stopsBattle;
    public List<CutsceneRoute> routes;
}