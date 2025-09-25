using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemEntry  
    {
        public string itemName;
        public int price;      // global price
        public Sprite icon;    // assign sprite for this item
        public int maxStack = 50;
    }

    public List<ItemEntry> items = new List<ItemEntry>();

    public int GetPrice(string itemName)
    {
        ItemEntry entry = items.Find(i => i.itemName == itemName);
        return entry != null ? entry.price : 0;
    }
    public int GetMaxStack(string itemName)
    {
        ItemEntry entry = items.Find(i => i.itemName == itemName);
        return entry != null ? entry.maxStack : 99; // default to 99 if not set
    }

    public Sprite GetItemSprite(string itemName)
    {
        ItemEntry entry = items.Find(i => i.itemName == itemName);
        return entry != null ? entry.icon : null;
    }
}
