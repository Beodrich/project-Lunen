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
    public GameObject ExperienceSlider;

    public GameObject innerImage;

    public Color DefaultButtonColor;

    [HideInInspector]
    public bool isSelected;
    private SpriteColorShift scs;
    private SpriteColorShift scsInner;
    private Image thisImage;
    
    [HideInInspector] public Button button;

    private void Awake()
    {
        scs = GetComponent<SpriteColorShift>();
        scsInner = innerImage.GetComponent<SpriteColorShift>();
        thisImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (scsInner != null)
        {
            if (isSelected)
            {
                scsInner.SetColorState(true);
            }
            else
            {
                scsInner.SetColorState(false);
            }
        }
    }
}
