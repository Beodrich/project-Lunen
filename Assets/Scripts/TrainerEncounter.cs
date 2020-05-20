using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainerEncounter : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> TeamObjects;
    [HideInInspector]
    public List<Monster> Team;

    private void Awake()
    {
        TeamObjects.AddRange(gameObject.transform.Cast<Transform>().Where(c => c.gameObject.tag == "Monster").Select(c => c.gameObject).ToArray());
        for (int i = 0; i < TeamObjects.Count; i++)
        {
            Team.Add(TeamObjects[i].GetComponent<Monster>());
        }
    }
}
