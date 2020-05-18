using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
