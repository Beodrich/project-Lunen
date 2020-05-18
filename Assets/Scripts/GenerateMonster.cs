using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMonster : MonoBehaviour
{
    public GameObject Template;
    public GameObject LunenBase;
    public List<GameObject> LunenMoves;
    [Range(1, 50)]
    public int LunenLevel = 1;

    private GameObject New1;
    private Monster NewMonster1;

    public bool Generate()
    {
        New1 = Instantiate(Template);
        //New1.transform.SetParent(this.transform);
        NewMonster1 = New1.GetComponent<Monster>();
        NewMonster1.Level = LunenLevel;
        for (int i = 0; i < LunenMoves.Count; i++)
        {
            NewMonster1.ActionSet.Add(Instantiate(LunenMoves[i]));
            NewMonster1.ActionSet[i].transform.SetParent(NewMonster1.transform);
        }
        
        NewMonster1.TemplateToMonster(LunenBase.GetComponent<Lunen>());
        return true;
    }
}
