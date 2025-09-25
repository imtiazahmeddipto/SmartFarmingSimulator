using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorHighlight : MonoBehaviour
{
    public Tilemap tilemap;                  // Assign your Tilemap
    public HoeScript interaction;            // Reference to HoeScript
    public SpriteRenderer highlightRenderer; // Square outline sprite

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (cam == null || tilemap == null || highlightRenderer == null || interaction == null)
            return;

        // Convert mouse position to world + cell
        Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f; // Ensure correct plane
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        // Snap highlight to tile center
        transform.position = tilemap.GetCellCenterWorld(cellPos);

        // Check if hoeing is valid
        bool valid = interaction.IsValidHoeCell(cellPos);

        if (valid)
        {
            highlightRenderer.enabled = true;
            highlightRenderer.color = Color.green;
        }
        else
        {
            highlightRenderer.enabled = false;
        }
    }
}
