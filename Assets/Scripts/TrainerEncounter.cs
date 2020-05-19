using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerEncounter : MonoBehaviour
{
    [System.Serializable]
    public struct PartyLunen
    {
        public GameObject species;
        [Range(1, 50)]
        public int level;
        public List<GameObject> moves;
    }

    public List<PartyLunen> team;
}
