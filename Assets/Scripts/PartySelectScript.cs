using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectScript : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    
    [HideInInspector] public Button button;
    [HideInInspector] public SpriteColorShift scs;
    [HideInInspector] public Image image;
    [HideInInspector] public Color defaultColor;

    public bool restrictInBattle;

    private void Awake()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        
        button = GetComponent<Button>();
        scs = GetComponent<SpriteColorShift>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void Update()
    {
        if (sr.battleSetup.InBattle && restrictInBattle) button.interactable = false;
        else button.interactable = true;
    }

    public void SetButtonInteractivity(bool interactible)
    {
        button.interactable = interactible;
    }

    public void SetSelected(bool selected)
    {
        scs.enabled = selected;
        if (!selected)
        {
            image.color = defaultColor;
        }
    }
}
