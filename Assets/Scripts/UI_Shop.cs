using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_Shop : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform container;
    private Transform shopItemTemp;

    private void Awake()
    {
        container = transform.Find("container");

        shopItemTemp = container.Find("ShopItemTemp");
        shopItemTemp.gameObject.SetActive(false);
    }
    private void Start()
    {
        CreateItemButton(Item.GetSprite(Item.ItemType.Potion), "Potion", Item.GetCost(Item.ItemType.Potion), 0);

        CreateItemButton(Item.GetSprite(Item.ItemType.Pokeball), "PokeBall", Item.GetCost(Item.ItemType.Pokeball), 0);
    }
    private void CreateItemButton(Sprite itemSprint, string itemName, int itemCost, int positionIndex) {
        Transform shopeItemTranform = Instantiate(shopItemTemp, container);
      RectTransform shopItemTranform=  shopItemTemp.GetComponent<RectTransform>();
        float shopItemHeight = 30f;
        shopItemTranform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);
        shopItemTranform.Find("nametext").GetComponent<TextMeshProUGUI>().SetText(itemName);

        shopItemTranform.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());
        shopItemTranform.Find("itemPicture").GetComponent<Image>().sprite = itemSprint;
        
    
    
    
    
    
    }
}
