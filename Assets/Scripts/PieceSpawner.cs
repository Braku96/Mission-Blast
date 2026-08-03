using UnityEngine;
using System.Collections.Generic;

public class PieceSpawner : MonoBehaviour
{
    [Header("All possible pieces")]
    public List<PieceData> allPieces;

    [Header("Tray slots (3 positions in world space)")]
    public Transform[] traySlots = new Transform[3];

    public PieceData[] currentPieces = new PieceData[3];

    public GameObject cellPrefab;
    public float previewCellSize = 0.4f;

    private List<GameObject>[] previewObjects = new List<GameObject>[3];

    void Start()
    {
        for (int i = 0; i < 3; i++)
            previewObjects[i] = new List<GameObject>();

        RefillTray();
    }

    public void RefillTray()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentPieces[i] == null)
                SpawnPieceInSlot(i);
        }
    }

    public void SpawnPieceInSlot(int slot)
    {
        foreach (var go in previewObjects[slot])
            Destroy(go);
        previewObjects[slot].Clear();

        PieceData piece = allPieces[Random.Range(0, allPieces.Count)];
        currentPieces[slot] = piece;

        // Fixed values — same for every piece no math tricks
        float cellUnit = 0.45f;  // spacing between cell centers
        float cellScale = 0.38f; // visual size of each cell

        // Find center of piece
        float sumX = 0, sumY = 0;
        foreach (var cell in piece.cells)
        {
            sumX += cell.x;
            sumY += cell.y;
        }
        float centerX = sumX / piece.cells.Length;
        float centerY = sumY / piece.cells.Length;

        foreach (var cell in piece.cells)
        {
            float x = (cell.x - centerX) * cellUnit;
            float y = (cell.y - centerY) * cellUnit;

            Vector3 pos = traySlots[slot].position + new Vector3(x, y, 0);
            GameObject go = Instantiate(cellPrefab, pos,
                                        Quaternion.identity, traySlots[slot]);
            go.transform.localScale = Vector3.one * cellScale;
            go.GetComponent<SpriteRenderer>().color = piece.pieceColor;
            previewObjects[slot].Add(go);
        }
    }
}