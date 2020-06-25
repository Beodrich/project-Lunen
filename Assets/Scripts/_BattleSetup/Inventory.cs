using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Inventory : MonoBehaviour
{
    
    [System.Serializable]
    public class InventoryEntry
    {
        public ScriptableItem item;
        public int amount;
        public InventoryEntry(ScriptableItem _item, int _amount)
        {
            item = _item;
            amount = _amount;
        }
    }

    public List<InventoryEntry> listOfItems;
    public List<InventoryEntry> requestedItems;
    public ScriptableItem.ItemType currentRequestedType;

    public int itemCap;

    void Awake()
    {
        NewInventoryType(currentRequestedType);
    }

    public void NewInventoryType(ScriptableItem.ItemType type)
    {
        requestedItems = new List<InventoryEntry>();
        foreach(InventoryEntry entry in listOfItems)
        {
            if (entry.item.itemType == type) requestedItems.Add(entry);
        }
        currentRequestedType = type;
    }

    public bool RemoveItem(ScriptableItem item, int amount)
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

    public void AddItem(ScriptableItem item, int amount)
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

    public int FindIndexInInventory(ScriptableItem item)
    {
        for (int i = 0; i < listOfItems.Count; i++)
        {
            if (listOfItems[i].item == item) return i;
        }
        return -1; //Returns -1 In Error
    }

    public void CheckToRescan(ScriptableItem item)
    {
        if (item.itemType == currentRequestedType)
        {
            NewInventoryType(currentRequestedType);
        }
    }
}
