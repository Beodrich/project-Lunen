using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMonster : MonoBehaviour
{
    public bool Generate;
    public GameObject Template;
    public GameObject LunenBase;
    public GameObject[] LunenMoves;
    public int LunenLevel;

    private GameObject New1;
    private Monster NewMonster1;

    void Start()
    {
        if (Generate)
        {
            New1 = Instantiate(Template);
            New1.transform.SetParent(this.transform);
            NewMonster1 = New1.GetComponent<Monster>();
            NewMonster1.Level = LunenLevel;
            NewMonster1.ActionSet.AddRange(LunenMoves);
            NewMonster1.TemplateToMonster(LunenBase.GetComponent<Lunen>());
        }
    }
}
