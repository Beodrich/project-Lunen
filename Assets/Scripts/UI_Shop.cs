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
    private List<Item.ItemType> playerInventory = new List<Item.ItemType>();
    private int gold = 40;




    private void Awake()
    {
        container = transform.Find("container");
        shopItemTemp = container.Find("ShopItemTemp");
        shopItemTemp.gameObject.SetActive(false);

    }
    private void Start()
    {
        CreateButton(Item.ItemType.Pokeball, Item.GetSprite(Item.ItemType.Pokeball), "PokeBall", Item.GetCost(Item.ItemType.Pokeball), 0);
        CreateButton(Item.ItemType.Potion, Item.GetSprite(Item.ItemType.Potion), "Potion", Item.GetCost(Item.ItemType.Potion), 1);
        Hide();
    }
    void CreateButton(Item.ItemType itemType, Sprite itemSprint, string itemName, int itemCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemp, container);//make a copy of the item temp
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        shopItemTransform.gameObject.SetActive(true);
        float shopItemHeight = 100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);
        shopItemTransform.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());

        shopItemTransform.Find("nametext").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("itemPicture").GetComponent<Image>().sprite = itemSprint;
        shopItemTransform.GetComponent<Button>().onClick.AddListener(() => TryBuyItem(itemType));
    }
    void TryBuyItem(Item.ItemType itemType) {
        if (HasEnoughGold(itemType))
        {
            Debug.Log("Has enough gold");
            BoughtItem(itemType);

        }
        else {
            Debug.Log("Not enough gold");
        }
    }
   public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() { gameObject.SetActive(false); }
    void BoughtItem(Item.ItemType itemType) {
         Debug.Log("Bought an item"+ itemType);
        playerInventory.Add(itemType);
    }
    private bool HasEnoughGold(Item.ItemType itemType) {

        if (gold >= Item.GetCost(itemType))
        {
            gold -= Item.GetCost(itemType);
            return true;
        }
        else {
            return false;
        }
    }
}









