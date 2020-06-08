using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;

    public GameObject Sel;
    public Vector2 Min;
    public Vector2 Max;
    // Start is called before the first frame update
    void Awake()
    {
        sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        sr.cameraFollow = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Sel != null)
        {
            transform.position = new Vector3
            (
            Mathf.Clamp(Sel.transform.position.x, Min.x, Max.x),
            Mathf.Clamp(Sel.transform.position.y, Min.y, Max.y),
            transform.position.z
            );
        }
        
    }
}
