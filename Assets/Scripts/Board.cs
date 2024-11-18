using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public HoldBoard holdBoard; // Assign this in the Unity Editor
    
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    private List<TetrominoData> tetrominoBag = new List<TetrominoData>();
    public TileBase[] availableTiles;
    private List<Tile> tileBag = new List<Tile>();

    public RectInt Bounds{
        get{
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake() {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominos.Length; i++) {
            tetrominos[i].Initialize();
        }

        RefillBag(); // Initialize Tetromino bag
        RefillTileBag(); // Initialize Tile bag
    }

    private void Start(){
        SpawnPiece();
    }

    public void SpawnPiece(){
        if (tetrominoBag.Count == 0) {
            RefillBag();
        }
        
        if (tileBag.Count == 0) {
            RefillTileBag();
        }

        // Draw a Tetromino from the Tetromino bag
        TetrominoData data = tetrominoBag[0];
        tetrominoBag.RemoveAt(0);

        // Assign a tile from the Tile bag
        data.tile = tileBag[0];
        tileBag.RemoveAt(0);

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition)){
            Set(activePiece);
        } else {
            GameOver();
        }
    }

    private void RefillBag(){
        tetrominoBag.Clear();

        // Add all Tetrominoes to the bag
        foreach (TetrominoData tetromino in tetrominos) {
            tetrominoBag.Add(tetromino);
        }

        // Shuffle the bag
        for (int i = 0; i < tetrominoBag.Count; i++) {
            int randomIndex = Random.Range(0, tetrominoBag.Count);
            TetrominoData temp = tetrominoBag[i];
            tetrominoBag[i] = tetrominoBag[randomIndex];
            tetrominoBag[randomIndex] = temp;
        }
    }

    private void RefillTileBag() {
        tileBag.Clear();

        // Add all tiles to the bag
        foreach (Tile tile in availableTiles) {
            tileBag.Add(tile);
        }

        // Shuffle the tiles
        for (int i = 0; i < tileBag.Count; i++) {
            int randomIndex = Random.Range(0, tileBag.Count);
            Tile temp = tileBag[i];
            tileBag[i] = tileBag[randomIndex];
            tileBag[randomIndex] = temp;
        }
    }

    private void GameOver(){
        tilemap.ClearAllTiles();
    }

    public void Set(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position){
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + position;

            if(!bounds.Contains((Vector2Int)tilePosition)){
                return false;
            }

            if(tilemap.HasTile(tilePosition)){
                return false;
            }
        }

        return true;
    }

    public void ClearLine(){
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax){
            if(IsLineFull(row)){
                LineClear(row);
            }
            else{
                row++;
            }
        }

    }

    private bool IsLineFull(int row){
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if(!tilemap.HasTile(position)){
                return false;
            };
        }

        return true;
    }

    private void LineClear(int row){
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax){
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
