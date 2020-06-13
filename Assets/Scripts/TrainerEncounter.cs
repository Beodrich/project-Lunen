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
    public bool defeated;

    private void Awake()
    {
        TeamObjects.AddRange(gameObject.transform.Cast<Transform>().Where(c => c.gameObject.tag == "Monster").Select(c => c.gameObject).ToArray());
        for (int i = 0; i < TeamObjects.Count; i++)
        {
            Team.Add(TeamObjects[i].GetComponent<Monster>());
        }
    }

    public void ClearTeamOfNull()
    {
        for (int i = 0; i < TeamObjects.Count; i++)
        {
            if (TeamObjects[i] == null)
            {
                TeamObjects.RemoveAt(i);
                i--;
            }
        }
    }
}
