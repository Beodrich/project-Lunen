using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private UI_Shop shop;
    void Start()
    {
        shop.Show();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {

            shop.Show();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        shop.Hide();
    }
}
