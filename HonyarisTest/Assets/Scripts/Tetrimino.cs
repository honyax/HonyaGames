using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino
{
    private Vector2Int _basePosition;
    public Game.BlockType BlockType { get; private set; }

    public Tetrimino()
    {
    }

    public void Initialize()
    {
        BlockType = Game.BlockType.TetriminoI;
        _basePosition = new Vector2Int(0, 0);
    }

    public Vector2Int[] GetBlockPositions()
    {
        return new Vector2Int[] {
            _basePosition,
            new Vector2Int(_basePosition.x, _basePosition.y + 1),
            new Vector2Int(_basePosition.x, _basePosition.y + 2),
            new Vector2Int(_basePosition.x, _basePosition.y + 3),
        };
    }

    public void MoveDown()
    {
        _basePosition.Set(_basePosition.x, _basePosition.y + 1);
    }

    public void MoveLeft()
    {
        _basePosition.Set(_basePosition.x - 1, _basePosition.y);
    }

    public void MoveRight()
    {
        _basePosition.Set(_basePosition.x + 1, _basePosition.y);
    }
}
