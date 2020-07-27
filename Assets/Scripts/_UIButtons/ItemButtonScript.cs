﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonScript : MonoBehaviour
{
    [HideInInspector] public SetupRouter sr;
    
    [HideInInspector] public Button button;
    [HideInInspector] public SpriteColorShift scs;
    [HideInInspector] public Image image;
    [HideInInspector] public Color defaultColor;

    [HideInInspector] public Inventory.InventoryEntry entry;
    [HideInInspector] public Item itemEntry;
    [HideInInspector] public GameData.PlayerLunen lunenEntry;

    public Text nameText;
    public Text amountText;

    private void Awake()
    {
        if (sr == null) sr = GameObject.Find("BattleSetup").GetComponent<SetupRouter>();
        
        button = GetComponent<Button>();
        scs = GetComponent<SpriteColorShift>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void Update()
    {
        
    }

    public void RestoreOriginalColor()
    {
        image.color = defaultColor;
    }

    public void SetInventoryEntry(Inventory.InventoryEntry _entry)
    {
        entry = _entry;
        if (entry != null)
        {
            button.interactable = true;
            nameText.text = entry.item.name;
            amountText.text = entry.amount + "x";
        }
        else
        {
            button.interactable = false;
            nameText.text = "";
            amountText.text = "";
        }
    }

    public void SetShopEntry(Item _entry)
    {
        itemEntry = _entry;
        if (itemEntry != null)
        {
            button.interactable = true;
            nameText.text = itemEntry.name;
            amountText.text = "$" + itemEntry.buyValue;
        }
        else
        {
            button.interactable = false;
            nameText.text = "";
            amountText.text = "";
        }
    }

    public void SetLunenEntry(GameData.PlayerLunen _entry)
    {
        lunenEntry = _entry;
        if (lunenEntry != null)
        {
            button.interactable = true;
            nameText.text = lunenEntry.nickname;
            amountText.text = "Lv " + lunenEntry.level;
        }
        else
        {
            button.interactable = false;
            nameText.text = "";
            amountText.text = "";
        }
    }

    public void SetButtonInteractivity(bool interactible)
    {
        button.interactable = interactible;
    }

    public void SetSelected(bool selected)
    {
        scs.SetColorState(selected);
    }

    public void TryBuyItem() {
        if (HasEnoughGold())
        {
            Debug.Log("Has enough gold");
            BoughtItem();

        }
        else {
            Debug.Log("Not enough gold");
        }
    }

    void BoughtItem() {
         Debug.Log("Bought an item"+ itemEntry.name);
         sr.inventory.AddItem(itemEntry, 1);
         sr.inventory.gold -= itemEntry.buyValue;
         sr.canvasCollection.RefreshInventoryButtons();
        //playerInventory.Add(itemType);
    }
    private bool HasEnoughGold() {

        return (sr.inventory.gold >= itemEntry.buyValue);
    }
}
