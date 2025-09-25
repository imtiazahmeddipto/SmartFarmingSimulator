using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image icon;
    public TextMeshProUGUI countText;

    [Header("Settings")]
    public bool isStatic = false; // manually mark a slot as static

    [HideInInspector] public string itemName;
    [HideInInspector] public int count;
    public StoragePanel storagePanel;
    // --- Properties ---
    public bool IsEmpty => string.IsNullOrEmpty(itemName);

    private Canvas canvas;
    private GameObject dragIcon;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(string name, int c, Sprite itemSprite = null)
    {
        itemName = name;
        count = c;

        if (itemSprite != null)
        {
            icon.sprite = itemSprite;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }

        countText.text = count > 1 ? count.ToString() : "";
    }

    public void Clear()
    {
        itemName = "";
        count = 0;
        icon.enabled = false;
        countText.text = "";
    }

    // --- Drag & Drop ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty || isStatic) return;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.rootCanvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = icon.sprite;
        img.raycastTarget = false;
        img.SetNativeSize();
        dragIcon.transform.localScale = Vector3.one * 1.5f;
        dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null) Destroy(dragIcon);

        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        // Dropped on another InventorySlot
        if (target != null && target.TryGetComponent<InventorySlot>(out InventorySlot targetSlot))
        {
            if (!targetSlot.isStatic)
            {
                SwapWith(targetSlot);
            }
        }
        // Dropped on StoragePanel content
        else if (target != null && target.transform.IsChildOf(storagePanel.contentParent))
        {
            MoveToStorage();
        }
    }

    private void MoveToStorage()
    {
        if (IsEmpty) return;

        storagePanel.storage.AddItem(itemName, count); // just call it

        Clear();
        storagePanel.RefreshUI();
    }




    private void SwapWith(InventorySlot targetSlot)
    {
        string tempName = targetSlot.itemName;
        int tempCount = targetSlot.count;
        Sprite tempSprite = targetSlot.icon.sprite;

        targetSlot.SetItem(itemName, count, icon.sprite);

        if (!string.IsNullOrEmpty(tempName))
            SetItem(tempName, tempCount, tempSprite);
        else
            Clear();
    }
}
