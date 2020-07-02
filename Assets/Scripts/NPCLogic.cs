using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLogic : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public Move move;

    public AnimationSet animationSet;
    public MoveScripts.Direction lookDirection;

    void Start()
    {
        GetImportantVariables();
        move.lookDirection = lookDirection;
    }
    
    void GetImportantVariables()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        if (move == null) move = GetComponent<Move>();

        move.animationSet = animationSet;
    }
}
