using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public PieceSpawner pieceSpawner;
    public Camera mainCamera;

    [Header("Colors")]
    public Color ghostValidColor = new Color(0.5f, 1f, 0.5f, 0.6f);
    public Color ghostInvalidColor = new Color(1f, 0.3f, 0.3f, 0.4f);

    private int draggedSlot = -1;
    private GameObject dragVisual;
    private PieceData draggedPiece;
    private Vector2Int lastGridPos = new Vector2Int(-1, -1);
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryPickUp();
        if (isDragging && Input.GetMouseButton(0)) DragMove();
        if (isDragging && Input.GetMouseButtonUp(0)) TryPlace();
    }

    Vector3 GetWorldPos()
    {
        Vector3 w = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        w.z = 0;
        return w;
    }

    void TryPickUp()
    {
        Vector3 world = GetWorldPos();
        for (int i = 0; i < 3; i++)
        {
            if (pieceSpawner.currentPieces[i] == null) continue;
            Vector3 slotPos = pieceSpawner.traySlots[i].position;
            if (Vector3.Distance(world, slotPos) < 2f)
            {
                PickUp(i);
                return;
            }
        }
    }

    void PickUp(int slot)
    {
        draggedSlot = slot;
        draggedPiece = pieceSpawner.currentPieces[slot];
        isDragging = true;

        dragVisual = new GameObject("DragVisual");
        foreach (var cell in draggedPiece.cells)
        {
            GameObject go = Instantiate(gridManager.cellPrefab, dragVisual.transform);
            go.transform.localPosition = new Vector3(
                cell.x * gridManager.cellSize,
                cell.y * gridManager.cellSize, -2f);
            go.transform.localScale = Vector3.one * gridManager.cellSize * 0.92f;
            go.GetComponent<SpriteRenderer>().color = draggedPiece.pieceColor;
        }

        DragMove();
    }

    void DragMove()
    {
        Vector3 world = GetWorldPos();
        world.y += 1.5f;
        dragVisual.transform.position = world;

        Vector2Int gridPos = gridManager.WorldToGrid(world);
        if (gridPos == lastGridPos) return;
        lastGridPos = gridPos;

        gridManager.ClearHighlights();
        bool valid = gridManager.CanPlace(draggedPiece, gridPos);
        Color highlight = valid ? ghostValidColor : ghostInvalidColor;
        gridManager.HighlightCells(draggedPiece, gridPos, highlight);
    }

    void TryPlace()
    {
        Vector3 world = GetWorldPos();
        world.y += 1.5f;
        Vector2Int gridPos = gridManager.WorldToGrid(world);

        gridManager.ClearHighlights();

        if (gridManager.CanPlace(draggedPiece, gridPos))
        {
            // Spawn new piece FIRST so all 3 slots are filled
            // before CheckGameOver runs
            pieceSpawner.currentPieces[draggedSlot] = null;
            pieceSpawner.SpawnPieceInSlot(draggedSlot);

            // Then place — this triggers CheckAndClearLines → CheckGameOver
            gridManager.PlacePiece(draggedPiece, gridPos);
        }

        Destroy(dragVisual);
        dragVisual = null;
        draggedSlot = -1;
        draggedPiece = null;
        isDragging = false;
        lastGridPos = new Vector2Int(-1, -1);
    }
}