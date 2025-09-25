using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class HomeStorage : MonoBehaviour
{
    private Dictionary<string, int> items = new Dictionary<string, int>();

    public int money = 0;
    public TextMeshProUGUI moneyText;

    public event Action OnStorageChanged; // notify UI

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddItem(string itemName, int amount)
    {
        if (!items.ContainsKey(itemName))
            items[itemName] = 0;

        items[itemName] += amount;
        Debug.Log($"Added {amount} {itemName}(s). Total: {items[itemName]}");

        OnStorageChanged?.Invoke();
    }

    public bool RemoveItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName) && items[itemName] >= amount)
        {
            items[itemName] -= amount;
            if (items[itemName] <= 0) items.Remove(itemName);

            OnStorageChanged?.Invoke();
            return true;
        }
        return false;
    }

    public int GetItemCount(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }

    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(items);
    }

    // --- Money functions ---
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }
}
