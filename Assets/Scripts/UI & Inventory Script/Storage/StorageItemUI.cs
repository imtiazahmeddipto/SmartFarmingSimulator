using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public TMP_InputField sellInput;
    public Button sellButton;

    public Image iconImage;   // ← drag your UI Image here in Inspector

    private string itemName;
    private HomeStorage storage;
    private ItemDatabase itemDatabase;
    public Button takeToInventoryButton;
    public Inventory playerInventory;

    public void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
    }

    public void Setup(string name, int count, HomeStorage s, ItemDatabase db, Inventory inv)
    {
        itemName = name;
        storage = s;
        itemDatabase = db;
        playerInventory = inv;

        nameText.text = name;
        UpdateQuantity();

        // Set icon
        Sprite itemSprite = itemDatabase.GetItemSprite(itemName);
        if (iconImage != null && itemSprite != null)
        {
            iconImage.sprite = itemSprite;
        }

        sellButton.onClick.AddListener(SellItem);
        takeToInventoryButton.onClick.AddListener(TakeToInventory);

        if (itemSprite != null)
        {
            iconImage.sprite = itemSprite;
            Debug.Log($"Icon assigned for {itemName}: {itemSprite.name}");
        }
        else
        {
            Debug.LogWarning($"Icon missing for {itemName} in ItemDatabase!");
        }

    }

    void TakeToInventory()
    {
        int takeAmount = 0;
        if (!int.TryParse(sellInput.text, out takeAmount) || takeAmount <= 0)
            return;

        int available = storage.GetItemCount(itemName);
        if (takeAmount > available)
            takeAmount = available;

        Sprite itemSprite = itemDatabase.GetItemSprite(itemName);

        bool added = playerInventory.AddItem(itemName, takeAmount, itemSprite);
        if (added)
        {
            storage.RemoveItem(itemName, takeAmount);
            UpdateQuantity();
        }
    }

    void UpdateQuantity()
    {
        int count = storage.GetItemCount(itemName);
        quantityText.text = $"x{count}";
    }

    void SellItem()
    {
        if (int.TryParse(sellInput.text, out int sellAmount))
        {
            if (sellAmount <= 0) return;

            int available = storage.GetItemCount(itemName);
            if (sellAmount > available) sellAmount = available;

            bool removed = storage.RemoveItem(itemName, sellAmount);
            if (removed)
            {
                int sellPrice = itemDatabase.GetPrice(itemName);
                storage.AddMoney(sellAmount * sellPrice);
                UpdateQuantity();
            }
        }
    }
}
