using UnityEngine;
using UnityEngine.Tilemaps;

public class HoldBoard : MonoBehaviour {
    public Tilemap tilemap { get; private set; }
    private TetrominoData heldPieceData;

    private void Awake() {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    public void SetHeldPiece(TetrominoData data) {
        ClearBoard();
        heldPieceData = data;

        if (data != null) {
            // Center the piece visually in the Hold board area
            Vector3Int centerPosition = Vector3Int.zero;
            foreach (Vector2Int cell in data.cells) {
                Vector3Int tilePosition = new Vector3Int(cell.x, cell.y, 0) + centerPosition;
                tilemap.SetTile(tilePosition, data.tile);
            }
        }
    }

    public void ClearBoard() {
        tilemap.ClearAllTiles();
    }
}
