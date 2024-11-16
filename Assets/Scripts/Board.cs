using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;

    private void Awake(){
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominos.Length; i++) {
            tetrominos[i].Initialize();
        }
    }

    private void Start(){
        SpawnPiece();
    }

    private void SpawnPiece(){
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        activePiece.Initialize(this, spawnPosition, data);
        Set(activePiece);
    }

    public void Set(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
}
