using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LunenButton : MonoBehaviour
{
    public GameObject Text;
    public GameObject LevelText;
    public GameObject HealthSlider;
    public GameObject CooldownSlider;

    public Color DefaultButtonColor;

    [HideInInspector]
    public bool isSelected;
    private SpriteColorShift scs;
    private Image thisImage;

    private void Awake()
    {
        scs = GetComponent<SpriteColorShift>();
        thisImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (scs != null)
        {
            if (isSelected)
            {
                scs.enabled = true;
            }
            else
            {
                scs.enabled = false;
                thisImage.color = DefaultButtonColor;
            }
        }
    }
}
