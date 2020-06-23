using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LunenActionButton : MonoBehaviour
{
    public GameObject Name;
    public GameObject Power;
    public GameObject Type;
    public GameObject Cooldown;
    [HideInInspector] public Button button;
    [HideInInspector] public SpriteColorShift scs;
    [HideInInspector] public Image image;
    [HideInInspector] public Color defaultColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        scs = GetComponent<SpriteColorShift>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void ToggleSCS()
    {
        scs.enabled = !scs.enabled;
        if (!scs.enabled) RestoreOriginalColor();
    }

    public void RestoreOriginalColor()
    {
        image.color = defaultColor;
    }
}
