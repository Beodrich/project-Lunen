using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GrassEncounter : MonoBehaviour
{
    [System.Serializable]
    public struct Encounter
    {
        public Lunen lunen;
        [MinMaxRange(1,30)]
        public RangedInt LevelRange;
        [Range(0f, 100f)]
        public float chanceWeight;
    }

    public List<Encounter> possibleEncounters;
    public float chanceModifier;

}
