using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassEncounter : MonoBehaviour
{
    [System.Serializable]
    public struct Encounter
    {
        public GameObject lunen;
        [Range(1,50)]
        public int minLevel;
        [Range(1, 50)]
        public int maxLevel;
        [Range(0f, 100f)]
        public float chanceWeight;
    }

    public List<Encounter> possibleEncounters;
    public float chanceModifier;

}
