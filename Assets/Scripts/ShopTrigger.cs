using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered the shop");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("left the shop");
    }
}
