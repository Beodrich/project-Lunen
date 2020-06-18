using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SortByYPosition : MonoBehaviour
{
    void Awake () {
        SetPosition();
    }

    void Update () {
        SetPosition();
    }

    void SetPosition () {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = -(int)transform.position.y;
        }
        else
        {

        }
    }

}
