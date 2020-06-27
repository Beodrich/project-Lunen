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
    private int gold = 40;

    public List<Item> sellItems;



    private void Awake()
    {
        container = transform.Find("container");
        shopItemTemp = container.Find("ShopItemTemp");
        shopItemTemp.gameObject.SetActive(false);

    }
    private void Start()
    {
        for (int i = 0; i < sellItems.Count; i++)
        {
            CreateButton(sellItems[i], i);
        }
        Hide();
    }
    void CreateButton(Item item, int positionIndex)
    {
        int itemCost = item.buyValue;
        string itemName = item.name;
        Sprite itemSprite = item.icon;
        Item.ItemType itemType = item.itemType;

        Transform shopItemTransform = Instantiate(shopItemTemp, container);//make a copy of the item temp
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        shopItemTransform.gameObject.SetActive(true);
        float shopItemHeight = 50f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);
        shopItemTransform.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());

        shopItemTransform.Find("nametext").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("itemPicture").GetComponent<Image>().sprite = itemSprite;
        shopItemTransform.GetComponent<Button>().onClick.AddListener(() => TryBuyItem(item));
    }
    void TryBuyItem(Item item) {
        if (HasEnoughGold(item))
        {
            Debug.Log("Has enough gold");
            BoughtItem(item);

        }
        else {
            Debug.Log("Not enough gold");
        }
    }
   public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() { gameObject.SetActive(false); }
    void BoughtItem(Item item) {
         Debug.Log("Bought an item"+ item.name);
        //playerInventory.Add(itemType);
    }
    private bool HasEnoughGold(Item item) {

        if (gold >= item.buyValue)
        {
            gold -= item.buyValue;
            return true;
        }
        else {
            return false;
        }
    }
}









