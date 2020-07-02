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
        Key
    }

    public string description;
    public bool tossable; //Whether or not the item can be thrown away

    public Sprite icon;

    public ItemType itemType;

    public int buyValue;
    public int sellValue;

    [ConditionalField(nameof(itemType), false, ItemType.Capture)] public float CatchRate;

}
