using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoText : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public Text text;

    void Start()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (sr.canvasCollection.MenuPanelOpen)
        {
            text.text = (string)sr.database.GetTriggerValue("PlayerInfo/Name") + "\n$" + sr.inventory.gold;
        }
        else if (sr.playerLogic != null)
        {
            if (sr.playerLogic.move.moveDetection.inShop)
            {
                text.text = (string)sr.database.GetTriggerValue("PlayerInfo/Name") + "\n$" + sr.inventory.gold;
            }
        }
    }
}
