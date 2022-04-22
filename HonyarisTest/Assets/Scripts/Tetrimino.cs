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

    public void Initialize(Game.BlockType blockType)
    {
        BlockType = blockType;
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

    public void Move(int deltaX, int deltaY)
    {
        _basePosition.Set(_basePosition.x + deltaX, _basePosition.y + deltaY);
    }
}
