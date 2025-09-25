using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections.Generic;

public class PlantingScript : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tilledTile;
    public float hoeRadius = 2f;
    public GameObject plantPrefab;
    public Vector2 plantOffset;
    public TextMeshProUGUI seedText;

    public static Dictionary<Vector3Int, GameObject> plantedTiles = new Dictionary<Vector3Int, GameObject>();

    private PlayerMovement movement;
    private Hotbar hotbar;

    void Start()
    {
        movement = FindObjectOfType<PlayerMovement>();
        hotbar = FindObjectOfType<Hotbar>();
    }

    void Update()
    {
        // Always get the current selected slot each frame
        InventorySlot currentSeedSlot = GetCurrentSeedSlot();
        int seedCount = currentSeedSlot != null ? currentSeedSlot.count : 0;

        // Continuously update the UI
        if (seedText != null)
            seedText.text = seedCount > 0 ? seedCount.ToString() : "";

        if (!enabled) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (currentSeedSlot == null || currentSeedSlot.count <= 0) return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector3Int cellPos = tilemap.WorldToCell(mouseWorld);

            if (tilemap.GetTile(cellPos) != tilledTile) return;
            if (Vector2.Distance(transform.position, tilemap.GetCellCenterWorld(cellPos)) > hoeRadius) return;
            if (plantedTiles.ContainsKey(cellPos)) return;

            movement.FaceDirection(tilemap.GetCellCenterWorld(cellPos).x - transform.position.x);

            Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos) + (Vector3)plantOffset;
            GameObject plant = Instantiate(plantPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = plant.GetComponent<Rigidbody2D>();
            if (rb == null) rb = plant.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            PlantGrowth pg = plant.GetComponent<PlantGrowth>();
            if (pg != null) pg.SetPlantedCell(cellPos);

            plantedTiles[cellPos] = plant;

            // Reduce seed count
            ConsumeSeed(currentSeedSlot);
        }
    }


    private InventorySlot GetCurrentSeedSlot()
    {
        if (hotbar == null) return null;

        InventorySlot selectedSlot = hotbar.GetSelectedSlot();
        if (selectedSlot != null && !selectedSlot.IsEmpty && selectedSlot.itemName.ToLower().Contains("seed"))
        {
            return selectedSlot;
        }

        return null;
    }

    private void ConsumeSeed(InventorySlot slot)
    {
        if (slot == null) return;

        slot.count = Mathf.Max(slot.count - 1, 0);

        if (slot.count == 0)
        {
            slot.Clear();
            hotbar.SelectSlot(0);
        }
        else
        {
            if (slot.countText != null)
                slot.countText.text = slot.count > 1 ? slot.count.ToString() : "";
        }
    }
}
