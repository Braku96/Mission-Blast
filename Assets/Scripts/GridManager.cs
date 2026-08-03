using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 9;
    public float cellSize = 0.72f;
    public GameObject cellPrefab;

    [Header("Colors")]
    public Color normalCellColor = new Color(0.55f, 0.38f, 0.18f);
    public Color zoneCellColor = new Color(0.80f, 0.65f, 0.40f);
    public Color blockedCellColor = new Color(0.30f, 0.18f, 0.08f);

    private GameObject[,] cellObjects;
    private AnimationManager animManager;
    public bool[,] grid;

    void Awake()
    {
        animManager = FindObjectOfType<AnimationManager>();
    }

    void Start()
    {
        grid = new bool[gridSize, gridSize];
        cellObjects = new GameObject[gridSize, gridSize];
        CalculateCellSize();
        SpawnGrid();
        ApplyBlockedCells(); // Add this line
    }

    void CalculateCellSize()
    {
        Camera cam = Camera.main;
        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        // 88% of screen width divided by 9 cells
        float availableWidth = screenWidth * 0.88f;
        cellSize = availableWidth / gridSize;
    }

    void SpawnGrid()
    {
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector3 pos = new Vector3(
                    transform.position.x + (col - (gridSize - 1) / 2f) * cellSize,
                    transform.position.y + (row - (gridSize - 1) / 2f) * cellSize,
                    0f);

                GameObject cell = Instantiate(cellPrefab, pos,
                                  Quaternion.identity, transform);
                cell.transform.localScale = Vector3.one * cellSize * 0.92f;
                cell.name = $"Cell_{row}_{col}";

                bool inDarkZone = (row / 3 + col / 3) % 2 == 0;
                cell.GetComponent<SpriteRenderer>().color =
                    inDarkZone ? normalCellColor : zoneCellColor;

                cellObjects[row, col] = cell;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float closestDist = float.MaxValue;
        Vector2Int closestCell = new Vector2Int(0, 0);

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                float dist = Vector2.Distance(
                    new Vector2(worldPos.x, worldPos.y),
                    new Vector2(cellObjects[row, col].transform.position.x,
                                cellObjects[row, col].transform.position.y));

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestCell = new Vector2Int(col, row);
                }
            }
        }
        return closestCell;
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return cellObjects[gridPos.y, gridPos.x].transform.position;
    }

    public void SetCellVisual(int row, int col, Color color, bool filled)
    {
        if (cellObjects[row, col] == null) return;

        // Never recolor a blocked cell
        if (IsBlocked(row, col)) return;

        var sr = cellObjects[row, col].GetComponent<SpriteRenderer>();
        sr.color = filled ? color :
                   ((row / 3 + col / 3) % 2 == 0 ? normalCellColor : zoneCellColor);
    }

    public bool CanPlace(PieceData piece, Vector2Int origin)
    {
        foreach (var cell in piece.cells)
        {
            int col = origin.x + cell.x;
            int row = origin.y + cell.y;
            if (col < 0 || col >= gridSize) return false;
            if (row < 0 || row >= gridSize) return false;
            if (grid[row, col]) return false;
            if (IsBlocked(row, col)) return false; // Extra safety
        }
        return true;
    }

    public void PlacePiece(PieceData piece, Vector2Int origin)
    {
        List<GameObject> placedCells = new List<GameObject>();

        foreach (var cell in piece.cells)
        {
            int col = origin.x + cell.x;
            int row = origin.y + cell.y;
            grid[row, col] = true;
            SetCellVisual(row, col, piece.pieceColor, true);
            placedCells.Add(cellObjects[row, col]);
        }

        if (animManager != null)
            animManager.PlayPlacePop(placedCells);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlace();

        CheckAndClearLines();
    }

    public void HighlightCells(PieceData piece, Vector2Int origin, Color color)
    {
        foreach (var cell in piece.cells)
        {
            int col = origin.x + cell.x;
            int row = origin.y + cell.y;
            if (col < 0 || col >= gridSize || row < 0 || row >= gridSize) continue;
            if (grid[row, col]) continue;
            cellObjects[row, col].GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void ClearHighlights()
    {
        for (int row = 0; row < gridSize; row++)
            for (int col = 0; col < gridSize; col++)
                if (!grid[row, col])
                {
                    bool inDarkZone = (row / 3 + col / 3) % 2 == 0;
                    cellObjects[row, col].GetComponent<SpriteRenderer>().color =
                        inDarkZone ? normalCellColor : zoneCellColor;
                }
    }

    public void CheckAndClearLines()
    {
        int totalCleared = 0;
        bool[,] toClear = new bool[gridSize, gridSize];

        // Check rows
        for (int row = 0; row < gridSize; row++)
        {
            bool full = true;
            for (int col = 0; col < gridSize; col++)
                if (!grid[row, col]) { full = false; break; }
            if (full)
            {
                totalCleared++;
                for (int col = 0; col < gridSize; col++)
                    if (!IsBlocked(row, col)) // Skip blocked cells
                        toClear[row, col] = true;
            }
        }

        // Check columns
        for (int col = 0; col < gridSize; col++)
        {
            bool full = true;
            for (int row = 0; row < gridSize; row++)
                if (!grid[row, col]) { full = false; break; }
            if (full)
            {
                totalCleared++;
                for (int row = 0; row < gridSize; row++)
                    if (!IsBlocked(row, col)) // Skip blocked cells
                        toClear[row, col] = true;
            }
        }

        // Check 3x3 zones
        for (int zoneRow = 0; zoneRow < 3; zoneRow++)
        {
            for (int zoneCol = 0; zoneCol < 3; zoneCol++)
            {
                bool full = true;
                for (int r = 0; r < 3; r++)
                    for (int c = 0; c < 3; c++)
                        if (!grid[zoneRow * 3 + r, zoneCol * 3 + c])
                        { full = false; break; }
                if (full)
                {
                    totalCleared++;
                    for (int r = 0; r < 3; r++)
                        for (int c = 0; c < 3; c++)
                            if (!IsBlocked(zoneRow * 3 + r, zoneCol * 3 + c)) // Skip blocked cells
                                toClear[zoneRow * 3 + r, zoneCol * 3 + c] = true;
                }
            }
        }

        if (totalCleared > 0)
        {
            List<GameObject> cellsToClear = new List<GameObject>();
            for (int row = 0; row < gridSize; row++)
                for (int col = 0; col < gridSize; col++)
                    if (toClear[row, col])
                        cellsToClear.Add(cellObjects[row, col]);

            int points = 0;
            for (int i = 1; i <= totalCleared; i++)
                points += i * 100;

            Vector3 center = Vector3.zero;
            foreach (var cell in cellsToClear)
                center += cell.transform.position;
            center /= cellsToClear.Count;

            if (animManager != null)
            {
                animManager.PlayClearFlash(cellsToClear, () =>
                {
                    for (int row = 0; row < gridSize; row++)
                        for (int col = 0; col < gridSize; col++)
                            if (toClear[row, col])
                            {
                                grid[row, col] = false;
                                SetCellVisual(row, col, Color.white, false);
                            }

                    ScoreManager.Instance.AddScore(points);
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayClear(totalCleared);
                    animManager.PlayScorePopup(center, points);
                    GameManager.Instance.CheckGameOver();
                });
            }
            else
            {
                for (int row = 0; row < gridSize; row++)
                    for (int col = 0; col < gridSize; col++)
                        if (toClear[row, col])
                        {
                            grid[row, col] = false;
                            SetCellVisual(row, col, Color.white, false);
                        }
                ScoreManager.Instance.AddScore(points);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayClear(totalCleared);
                GameManager.Instance.CheckGameOver();
            }
        }
        else
        {
            GameManager.Instance.CheckGameOver();
        }
    }
    void ApplyBlockedCells()
    {
        LevelData level = LevelManager.Instance.GetCurrentLevel();
        if (level == null) return;

        for (int i = 0; i < level.blockedCells.Length; i++)
        {
            if (!level.blockedCells[i]) continue;

            int row = i / gridSize;
            int col = i % gridSize;

            // Mark as permanently filled
            grid[row, col] = true;

            // Visual — dark blocked color
            if (cellObjects[row, col] != null)
                cellObjects[row, col].GetComponent<SpriteRenderer>().color
                    = blockedCellColor;
        }
    }

    public bool IsBlocked(int row, int col)
    {
        LevelData level = LevelManager.Instance.GetCurrentLevel();
        if (level == null) return false;

        int index = row * gridSize + col;
        if (index >= level.blockedCells.Length) return false;

        return level.blockedCells[index];
    }
}