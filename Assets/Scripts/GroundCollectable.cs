using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GroundCollectable : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public GuidComponent gc;

    // Start is called before the first frame update
    void Start()
    {
        GameObject main = GameObject.Find("BattleSetup");
        gc = GetComponent<GuidComponent>();
        if (main != null)
        {
            sr = main.GetComponent<SetupRouter>();
            if (sr.battleSetup.GuidInList(gc.GetGuid()))
            {
                Destroy(gameObject);
            }
        }
    }
}
