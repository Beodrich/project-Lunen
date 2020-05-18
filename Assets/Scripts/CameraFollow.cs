using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Sel;
    public Vector2 Min;
    public Vector2 Max;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3
        (
        Mathf.Clamp(Sel.transform.position.x, Min.x, Max.x),
        Mathf.Clamp(Sel.transform.position.y, Min.y, Max.y),
        transform.position.z
        );
    }
}
