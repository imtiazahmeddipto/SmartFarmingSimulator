using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [Header("UI Slots")]
    public GameObject[] slots;       // Assign slot UI GameObjects in Inspector
    public Image highlight;          // Assign highlight image

    [Header("References")]
    public Inventory inventory;      // Drag your Inventory here
    public Transform ItenHolder;         // Drag the Player GameObject here (all usable items must be its children)

    private int selectedSlot = 0;

    void Start()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
        }

        // Make sure highlight starts correctly
        Canvas.ForceUpdateCanvases();
        UpdateHighlight();
        ActivateSelectedItem();
    }

    void Update()
    {
        // Switch slots with number keys 1–5
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
    }
    public InventorySlot GetSelectedSlot()
    {
        if (selectedSlot >= 0 && selectedSlot < slots.Length)
            return slots[selectedSlot].GetComponent<InventorySlot>();
        return null;
    }

    // Called when switching hotbar slots
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        selectedSlot = slotIndex;
        UpdateHighlight();
        ActivateSelectedItem();
    }

    void UpdateHighlight()
    {
        if (highlight == null || selectedSlot >= slots.Length) return;

        highlight.gameObject.SetActive(true);
        highlight.transform.position = slots[selectedSlot].transform.position;
    }

    void ActivateSelectedItem()
    {
        // Disable all children first
        for (int i = 0; i < ItenHolder.childCount; i++)
        {
            ItenHolder.GetChild(i).gameObject.SetActive(false);
        }

        string itemName = GetSelectedItemName();
        if (string.IsNullOrEmpty(itemName)) return;

        // Enable only the one that matches
        Transform child = ItenHolder.Find(itemName);
        if (child != null)
        {
            child.gameObject.SetActive(true);
        }
    }

    string GetSelectedItemName()
    {
        if (inventory == null || selectedSlot >= inventory.slots.Length) return null;

        InventorySlot invSlot = inventory.slots[selectedSlot];
        if (invSlot == null || invSlot.IsEmpty) return null;

        return invSlot.itemName;
    }
}
