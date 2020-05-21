using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public float startX;
    public float startY;
    public float startZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public float lastX;
    public float lastY;
    public float lastZ;

    public bool unscaledDeltaTime;
    // Start is called before the first frame update
    void Start()
    {
        lastX = startX;
        lastY = startY;
        lastZ = startZ;
    }

    // Update is called once per frame
    void Update()
    {
        if (unscaledDeltaTime)
        {
            lastX += rotX * Time.unscaledDeltaTime;
            lastY += rotY * Time.unscaledDeltaTime;
            lastZ += rotZ * Time.unscaledDeltaTime;
        }
        else
        {
            lastX += rotX * Time.deltaTime;
            lastY += rotY * Time.deltaTime;
            lastZ += rotZ * Time.deltaTime;
        }
        
        transform.rotation = Quaternion.Euler(new Vector3(lastX, lastY, lastZ));
    }
}
