using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private UI_Shop shop;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
       IShopCustomer shopCustomer = collision.GetComponent<IShopCustomer>();
       if (shopCustomer != null) {
            Debug.Log(shopCustomer + "has entered");
            shop.Show(shopCustomer);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IShopCustomer shopCustomer = collision.GetComponent<IShopCustomer>();
        if (shopCustomer != null) {
            Debug.Log(shopCustomer + " has left");
           shop.Hide();
        
        }
    }
}
