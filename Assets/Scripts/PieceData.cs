using UnityEngine;

[CreateAssetMenu(fileName = "NewPiece", menuName = "BlockBlast/Piece")]
public class PieceData : ScriptableObject
{
    public string pieceName;
    public Color pieceColor = Color.cyan;

    // Each Vector2Int is a cell offset from the center (0,0)
    public Vector2Int[] cells;
}