using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemText;  // shows "Seed x3 - 30"
    public Button cancelButton;       // remove button

    private string itemName;
    private ShopPanel shopPanel;

    // Initialize from ShopPanel
    public void Setup(string name, int quantity, int price, ShopPanel panel)
    {
        itemName = name;
        shopPanel = panel;

        itemText.text = $"{name} x{quantity} - {price * quantity}";
        cancelButton.onClick.AddListener(RemoveFromCart);
    }

    void RemoveFromCart()
    {
        shopPanel.RemoveFromCart(itemName);
        Destroy(gameObject);
    }
}
