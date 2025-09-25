using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class HoeScript : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tilledTile;
    public float holdTime = 0.5f;
    public float hoeRadius = 2f;
    public List<TileBase> allowedTiles;

    private Vector3Int targetedCell;
    private float holdTimer = 0f;
    private bool isHolding = false;

    private PlayerMovement movement;
    private Animator animator;

    void Start()
    {
        movement = FindObjectOfType<PlayerMovement>();
        if (movement != null)
            animator = movement.GetComponent<Animator>();
    }

    void Update()
    {
        if (!enabled || movement == null) return;
        if (movement.IsRunning) { ResetHoe(); return; }

        if (Input.GetMouseButton(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            targetedCell = tilemap.WorldToCell(worldPos);

            if (!IsValidHoeCell(targetedCell)) { ResetHoe(); return; }

            // Face the hoeing direction
            float direction = targetedCell.x - transform.position.x;
            movement.FaceDirection(direction);

            if (animator != null)
                animator.SetBool("IsMining", true);

            if (!isHolding) holdTimer = 0f;
            isHolding = true;

            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime)
            {
                HoeTile(targetedCell);
                ResetHoe();
            }
        }
        else
        {
            ResetHoe();
        }
    }

    void ResetHoe()
    {
        if (animator != null)
            animator.SetBool("IsMining", false);

        isHolding = false;
        holdTimer = 0f;
    }

    // <-- Made this PUBLIC so CursorHighlight can use it
    public bool IsValidHoeCell(Vector3Int cellPos)
    {
        Vector3 cellWorldCenter = tilemap.GetCellCenterWorld(cellPos);
        if (Vector2.Distance(transform.position, cellWorldCenter) > hoeRadius) return false;

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null || !allowedTiles.Contains(tile)) return false;

        return true;
    }

    void HoeTile(Vector3Int cellPos)
    {
        tilemap.SetTile(cellPos, tilledTile);
    }
}
