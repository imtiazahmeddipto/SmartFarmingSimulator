using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public string itemName;
    public int quantity = 1;
    public Button buyButton;

    private ShopPanel shopPanel;
    public ItemDatabase itemDatabase; // assign in inspector

    void Start()
    {
        shopPanel = FindObjectOfType<ShopPanel>();
        buyButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        int price = itemDatabase.GetPrice(itemName);
        shopPanel.AddToCart(itemName, price, quantity);
    }
}
