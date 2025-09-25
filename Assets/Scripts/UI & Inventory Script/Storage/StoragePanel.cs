using UnityEngine;

public class StoragePanel : MonoBehaviour
{
    public HomeStorage storage;
    public StorageItemUI itemUIPrefab;
    public Transform contentParent; // Vertical or Grid layout parent
    public ItemDatabase itemDatabase;
    public Inventory playerInventory;

    void Start()
    {
        storage.OnStorageChanged += RefreshUI;
        RefreshUI();
    }

    public void RefreshUI()
    {
        // Clear old UI
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Rebuild from storage
        foreach (var pair in storage.GetAllItems())
        {
            var ui = Instantiate(itemUIPrefab, contentParent);
            ui.Setup(pair.Key, pair.Value, storage, itemDatabase, playerInventory);
        }
    }
}
