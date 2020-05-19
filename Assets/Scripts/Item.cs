using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public enum ItemType { 
    Nothing,
    Potion,
    JelBanana,
    HolyGrail,
    Pokeball,
    GreatBall,
    UltraBall,
    JojoBall,
    //can add more latter
    }
    public static int GetCost(ItemType itemType) {
        switch (itemType) {
            default:
            case ItemType.Nothing: return 0;
            case ItemType.Potion: return 30;
            case ItemType.JelBanana: return 100;
            case ItemType.HolyGrail: return 200;
            case ItemType.Pokeball: return 40;
            case ItemType.GreatBall: return 50;
            case ItemType.UltraBall: return 75;
            case ItemType.JojoBall: return 250;

        
        }

    }
    public static Sprite GetSprite(ItemType itemType) {
        switch (itemType) {
            default:
            case ItemType.Potion: return GameAssets.Instance.Potion;
            case ItemType.Pokeball: return GameAssets.Instance.Pokeball;
;
                //TO DO: ADD MORE LATTER 

        }
    
    }
}
