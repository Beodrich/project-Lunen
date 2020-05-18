using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassEncounter : MonoBehaviour
{
    [System.Serializable]
    public struct Encounter
    {
        public GameObject lunen;
        [VectorLabels("Min", "Max")]
        public Vector2Int LevelRange;
        [Range(0f, 100f)]
        public float chanceWeight;
    }

    public List<Encounter> possibleEncounters;
    public float chanceModifier;

}
