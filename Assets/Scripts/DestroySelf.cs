using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [HideInInspector] public float TimeBeforeDie = 999999;
    public float TimeBeforeDieCurrent;

    void Start()
    {
        TimeBeforeDieCurrent = TimeBeforeDie;
    }

    void Update()
    {
        TimeBeforeDieCurrent -= Time.deltaTime;
        if (TimeBeforeDieCurrent < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetDieTime(float newTime)
    {
        TimeBeforeDie = newTime;
        TimeBeforeDieCurrent = newTime;
    }
}
