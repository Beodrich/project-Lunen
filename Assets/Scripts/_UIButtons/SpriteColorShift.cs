using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteColorShift : MonoBehaviour
{
    public Color color1;
    public Color color2;

    public Color firstColor;

    public Image t;

    public bool changeColor;

    public float timeBetweenColors;
    public float timeBetweenColorsCurrent;
    bool turnAround;


    // Start is called before the first frame update
    void Awake()
    {
        firstColor = t.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (changeColor)
        {
            timeBetweenColorsCurrent = Time.time % (timeBetweenColors*2);

            t.color = Color.Lerp(color1, color2, Mathf.Abs((timeBetweenColorsCurrent-timeBetweenColors) / timeBetweenColors));
        }
        


    }

    public void SetColorState(bool toggle)
    {
        changeColor = toggle;
        if (!toggle)
        {
            t.color = firstColor;
        }
    }
}
