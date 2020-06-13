using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleInGame : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public SpriteRenderer thisRenderer;
    public bool KeepVisible;
    void Start()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
        if (!KeepVisible) thisRenderer.color = new Color(0,0,0,0);
    }
}
