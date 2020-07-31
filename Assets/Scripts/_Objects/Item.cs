using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New Item", menuName = "GameElements/Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Capture,
        Healing,
        Cure,
        Buff,
        Held,
        Key,
        GreatHeal,
        UltraHeal,
        MaxHeal
    }

    public string description;
    public bool tossable; //Whether or not the item can be thrown away

    public Sprite icon;

    public ItemType itemType;

    public int buyValue;
    public int sellValue;

    [ConditionalField(nameof(itemType), false, ItemType.Capture)] public float CatchRate;

    public static int GetHealValue(Item.ItemType healItem) {

      

        switch (healItem)
        {
            case Item.ItemType.Healing:  return 20;
            case Item.ItemType.GreatHeal: return 40;
            case Item.ItemType.UltraHeal: return 60;
           

        }
        return -1;//return -1 in error 
    }

}
