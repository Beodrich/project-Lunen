using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private UI_Shop shop;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            shop.Show();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        shop.Hide();

    }
 
}
