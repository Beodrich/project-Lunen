using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Shadow : MonoBehaviour
{

    [HideInInspector] public GameObject parent;
    [HideInInspector] public SpriteRenderer parentRenderer;
    [HideInInspector] public SpriteRenderer thisRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        parent = transform.parent.gameObject;
        thisRenderer = GetComponent<SpriteRenderer>();
        parentRenderer = parent.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        thisRenderer.sprite = parentRenderer.sprite;
    }
}

