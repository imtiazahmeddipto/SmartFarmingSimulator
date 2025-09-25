using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventorySlot[] slots;   // Assign in inspector
    public int staticSlots = 2;     // First 2 slots are immutable
    public ItemDatabase itemDatabase;
    // Add an item to inventory
    public bool AddItem(string itemName, int count, Sprite itemSprite = null)
    {
        int remaining = count;
        int maxStack = itemDatabase.GetMaxStack(itemName);

        // Try to stack first
        for (int i = staticSlots; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].itemName == itemName)
            {
                int canAdd = maxStack - slots[i].count;
                if (canAdd <= 0) continue;

                int toAdd = Mathf.Min(canAdd, remaining);
                slots[i].SetItem(itemName, slots[i].count + toAdd, itemSprite);
                remaining -= toAdd;

                if (remaining <= 0) return true;
            }
        }

        // Add to empty slot
        for (int i = staticSlots; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                int toAdd = Mathf.Min(maxStack, remaining);
                slots[i].SetItem(itemName, toAdd, itemSprite);
                remaining -= toAdd;

                if (remaining <= 0) return true;
            }
        }

        return remaining <= 0; // return false if inventory full
    }


    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < staticSlots || indexB < staticSlots) return; // Skip static slots

        var tempItem = slots[indexA].itemName;
        var tempCount = slots[indexA].count;
        var tempSprite = slots[indexA].icon.sprite;

        slots[indexA].SetItem(slots[indexB].itemName, slots[indexB].count, slots[indexB].icon.sprite);
        slots[indexB].SetItem(tempItem, tempCount, tempSprite);
    }
}
