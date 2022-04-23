using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino
{
    const int PatternXLength = 4;
    const int PatternYLength = 4;

    private Vector2Int _basePosition;
    public Game.BlockType BlockType { get; private set; }

    public Tetrimino()
    {
    }

    public void Initialize(Game.BlockType blockType = Game.BlockType.None)
    {
        if (blockType == Game.BlockType.None)
        {
            blockType = (Game.BlockType)Random.Range(1, 8);
        }

        BlockType = blockType;
        _basePosition = new Vector2Int(0, 0);
    }

    public Vector2Int[] GetBlockPositions()
    {
        var positions = new Vector2Int[4];
        var pattern = _typePatterns[BlockType];
        var positionIndex = 0;
        for (var y = 0; y < PatternYLength; y++)
        {
            for (var x = 0; x < PatternXLength; x++)
            {
                if (pattern[0, y, x] == 1)
                {
                    positions[positionIndex] = new Vector2Int(_basePosition.x + x, _basePosition.y + y);
                    positionIndex++;
                }
            }
        }

        return positions;
    }

    public void Move(int deltaX, int deltaY)
    {
        _basePosition.Set(_basePosition.x + deltaX, _basePosition.y + deltaY);
    }

    static readonly Dictionary<Game.BlockType, int[,,]> _typePatterns = new Dictionary<Game.BlockType, int[,,]>()
    {
        {
            Game.BlockType.TetriminoI,
            new int[,,]
            {
                {
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 1 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
        {
            Game.BlockType.TetriminoO,
            new int[,,]
            {
                {
                    { 0, 1, 1, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
        {
            Game.BlockType.TetriminoS,
            new int[,,]
            {
                {
                    { 0, 1, 1, 0 },
                    { 1, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
        {
            Game.BlockType.TetriminoZ,
            new int[,,]
            {
                {
                    { 1, 1, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
        {
            Game.BlockType.TetriminoJ,
            new int[,,]
            {
                {
                    { 1, 0, 0, 0 },
                    { 1, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
        {
            Game.BlockType.TetriminoL,
            new int[,,]
            {
                {
                    { 0, 0, 1, 0 },
                    { 1, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
        {
            Game.BlockType.TetriminoT,
            new int[,,]
            {
                {
                    { 0, 1, 0, 0 },
                    { 1, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
    };
}
