using System;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelayInSecond = 1f;
    public float lockDelayInSecond = 0.5f;

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data){
        this.board = board;
        this.position = position;
        this.data = data;
        rotationIndex = 0;
        stepTime = Time.time + stepDelayInSecond;
        lockTime = 0f;

        if(cells == null){
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++){
            cells[i] = (Vector3Int) data.cells[i];
        }
    }

    public void Update(){
        board.Clear(this);

        lockTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)){
            Move(Vector2Int.right);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow)){
            Rotate(1);
        }

        if(Input.GetKeyDown(KeyCode.DownArrow)){
            Move(Vector2Int.down);
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            HardDrop();
        }

        if(Time.time >= stepTime){
            Step();
        }

        board.Set(this);
    }

    private void Step(){
        stepTime = Time.time + stepDelayInSecond;

        Move(Vector2Int.down);

        if(lockTime >= lockDelayInSecond){
            Lock();
        }
    }
    
    private void HardDrop(){
        while(Move(Vector2Int.down)){
            continue;
        }

        Lock();
    }

    private void Lock(){
        board.Set(this);
        board.ClearLine();
        board.SpawnPiece();
    }
    
    private bool Move(Vector2Int translation){
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if(valid){
            position = newPosition;
            lockTime = 0;
        }

        return valid;
    }

    private void Rotate(int direction){
        int originalRotation = rotationIndex;
        rotationIndex = (rotationIndex + direction) % 4;

        ApplyRotationMatrix(direction);

        if(!TestWallKicks(rotationIndex, direction)){
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction){
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0]  + cell.y * Data.RotationMatrix[1]) * direction);
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2]  + cell.y * Data.RotationMatrix[3]) * direction);
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0]  + cell.y * Data.RotationMatrix[1]) * direction);
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2]  + cell.y * Data.RotationMatrix[3]) * direction);
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection){
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++){
            Vector2Int translation = data.wallKicks[wallKickIndex, i];
            
            if(Move(translation)){
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection){
        int wallKickIndex = rotationIndex * 2;

        if(rotationDirection < 0){
            wallKickIndex--;
        }

        return wallKickIndex % data.wallKicks.GetLength(0);
    }
}
