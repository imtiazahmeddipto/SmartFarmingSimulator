using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    public Transform cartContent;        // UI container for cart items
    public GameObject cartItemPrefab;    // prefab with CartItemUI
    public TextMeshProUGUI totalText;
    public Button buyButton;

    private List<CartItem> cart = new List<CartItem>();
    private int total = 0;
    private HomeStorage storage;

    void Start()
    {
        storage = FindObjectOfType<HomeStorage>();
        buyButton.onClick.AddListener(BuyAll);
        UpdateUI();
    }

    public void AddToCart(string itemName, int price, int quantity)
    {
        CartItem existing = cart.Find(i => i.itemName == itemName);
        if (existing != null)
        {
            existing.quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem(itemName, price, quantity));
        }

        total += price * quantity;
        UpdateUI();
    }

    public void RemoveFromCart(string itemName)
    {
        CartItem existing = cart.Find(i => i.itemName == itemName);
        if (existing != null)
        {
            total -= existing.price * existing.quantity;
            cart.Remove(existing);
        }

        UpdateUI();
    }

    void BuyAll()
    {
        // check if player has enough money
        if (storage.money < total)
        {
            Debug.Log("Not enough money!");
            return;
        }

        // deduct money
        storage.SpendMoney(total);

        // add items to storage
        foreach (CartItem item in cart)
        {
            storage.AddItem(item.itemName, item.quantity);
        }

        // clear cart
        cart.Clear();
        total = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform child in cartContent)
            Destroy(child.gameObject);

        foreach (CartItem item in cart)
        {
            GameObject go = Instantiate(cartItemPrefab, cartContent);
            CartItemUI ui = go.GetComponent<CartItemUI>();
            ui.Setup(item.itemName, item.quantity, item.price, this);
        }

        totalText.text = "Total: " + total;
    }
}
