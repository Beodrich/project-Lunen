using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LunenButton : MonoBehaviour
{
    public Text TitleText;
    public Text LevelText;
    public MonsterHealthText HealthText;
    public DrawHealthbar HealthSlider;
    public DrawHealthbar CooldownSlider;
    public DrawHealthbar ExperienceSlider;

    public Image LunenType1;
    public Image LunenType2;

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
