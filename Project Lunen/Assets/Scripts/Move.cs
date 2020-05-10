using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private float horMov;
    private float verMov;
    public float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horMov = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        verMov = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.W)) {

            transform.Translate(new Vector3(0f, speed * Time.deltaTime, 0f));
        
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0f, -speed * Time.deltaTime, 0f));



        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0f, 0f));



        }
        if (Input.GetKey(KeyCode.D))
        {

            transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));


        }
    }
}
