using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunen : MonoBehaviour
{
    [System.Serializable]
    public struct LearnedAction
    {
        public GameObject Action;
        public int Level;
    }

    [Header("Species Info")]

    public string Name;
    public List<Types.Element> Elements;
    public List<LearnedAction> ActionsToLearn;

    public GameObject EvolutionLunen;
    public int EvolutionLevel;

    [Header("Stats")]

    [VectorLabels("Start"," PerLevel")]
    public Vector2Int Health;
    [VectorLabels("Start", " PerLevel")]
    public Vector2Int Attack;
    [VectorLabels("Start", " PerLevel")]
    public Vector2Int Defense;
    [VectorLabels("Start", " PerLevel")]
    public Vector2Int Speed;

    public float CooldownTime;
}
