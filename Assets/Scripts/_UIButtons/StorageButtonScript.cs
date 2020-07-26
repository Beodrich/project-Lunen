using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageButtonScript : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    [HideInInspector] public Button button;
    public Text targetText;
    public string buttonStorage;
    public string buttonParty;

    void Start()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        button = GetComponent<Button>();
    }


    // Update is called once per frame
    void Update()
    {
        if (sr.canvasCollection.StoragePanelOpen)
        {
            if (sr.canvasCollection.PartySwapSelect != -1)
            {
                button.interactable = true;
                targetText.text = buttonStorage;
            }
            else if (sr.canvasCollection.StorageLunenIndexSelect != -1)
            {
                button.interactable = true;
                targetText.text = buttonParty;
            }
            else
            {
                button.interactable = false;
            }
            
        }
        
    }
}
