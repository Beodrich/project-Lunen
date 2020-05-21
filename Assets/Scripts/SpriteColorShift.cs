using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteColorShift : MonoBehaviour
{
    public Color color1;
    public Color color2;

    public Image t;

    public float timeBetweenColors;
    public float timeBetweenColorsCurrent;
    bool turnAround;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeBetweenColorsCurrent -= Time.unscaledDeltaTime;
        if (timeBetweenColorsCurrent < 0)
        {
            timeBetweenColorsCurrent += timeBetweenColors;
            turnAround = !turnAround;
        }

        if (turnAround)
        {
            t.color = Color.Lerp(color1, color2, timeBetweenColorsCurrent / timeBetweenColors);
        }
        else
        {
            t.color = Color.Lerp(color2, color1, timeBetweenColorsCurrent / timeBetweenColors);
        }


    }
}
