using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Inventory : MonoBehaviour
{
    
    [System.Serializable]
    public class InventoryEntry
    {
        public Item item;
        public int amount;
        public InventoryEntry(Item _item, int _amount)
        {
            item = _item;
            amount = _amount;
        }
    }

    public List<InventoryEntry> listOfItems;
    public List<InventoryEntry> requestedItems;
    public Item.ItemType currentRequestedType;

    public int itemCap;
    public int gold;

    void Awake()
    {
        NewInventoryType(currentRequestedType);
    }

    public void NewInventoryType(Item.ItemType type)
    {
        requestedItems = new List<InventoryEntry>();
        foreach(InventoryEntry entry in listOfItems)
        {
            if (entry.item.itemType == type) requestedItems.Add(entry);
        }
        currentRequestedType = type;
    }

    public bool RemoveItem(Item item, int amount)
    {
        int index = FindIndexInInventory(item);
        if (index != -1)
        {
            if (listOfItems[index].amount >= amount)
            {
                listOfItems[index].amount -= amount;
                if (listOfItems[index].amount == 0)
                {
                    listOfItems.RemoveAt(index);
                    CheckToRescan(item);
                }
                return true;
            }
            else return false;
        }
        return false;
    }

    public void AddItem(Item item, int amount)
    {
        int index = FindIndexInInventory(item);
        if (index != -1)
        {
            listOfItems[index].amount += amount;
        }
        else
        {
            listOfItems.Add(new InventoryEntry(item, amount));
            CheckToRescan(item);
        }
    }

    public int FindIndexInInventory(Item item)
    {
        for (int i = 0; i < listOfItems.Count; i++)
        {
            if (listOfItems[i].item == item) return i;
        }
        return -1; //Returns -1 In Error
    }

    public void CheckToRescan(Item item)
    {
        if (item.itemType == currentRequestedType)
        {
            NewInventoryType(currentRequestedType);
        }
    }
}
