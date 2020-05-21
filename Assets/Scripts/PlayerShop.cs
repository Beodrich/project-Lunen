using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviour, IShopCustomer
{
    private List<Item.ItemType> playerInventory;
    public void BoughtItem(Item.ItemType itemType)
    {
        Debug.Log("Bought item: " + itemType);
        playerInventory.Add(itemType);
        PrintInventory();
    }

    public bool TrySpendGoldAmount(int goldAmount)
    {
        throw new System.NotImplementedException();
    }
    private void PrintInventory() {
        foreach (var i in playerInventory) {

            Debug.Log(i.ToString());
        }
    
    }
}
