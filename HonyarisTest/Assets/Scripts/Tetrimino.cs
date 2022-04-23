using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino
{
    const int PatternXLength = 4;
    const int PatternYLength = 4;

    private Vector2Int _basePosition;
    private int _rollPattern;
    public Game.BlockType BlockType { get; private set; }
    private int RollPatternNum { get { return BlockType == Game.BlockType.TetriminoO ? 1 : 4; } }
    private int NextRollPattern { get { return _rollPattern + 1 < RollPatternNum ? _rollPattern + 1 : 0; } }

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
        _basePosition = new Vector2Int(3, 0);
        _rollPattern = 0;
    }

    public Vector2Int[] GetBlockPositions()
    {
        return GetBlockPositions(_rollPattern);
    }

    public Vector2Int[] GetBlockPositions(int rollPattern)
    {
        var positions = new Vector2Int[4];
        var pattern = _typePatterns[BlockType];
        var positionIndex = 0;
        for (var y = 0; y < PatternYLength; y++)
        {
            for (var x = 0; x < PatternXLength; x++)
            {
                if (pattern[rollPattern, y, x] == 1)
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

    public Vector2Int[] GetRolledBlockPositions()
    {
        return GetBlockPositions(NextRollPattern);
    }

    public void Roll()
    {
        _rollPattern = NextRollPattern;
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
                {
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                },
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 1 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
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
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 0, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 1, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 1, 0, 0, 0 },
                    { 1, 1, 0, 0 },
                    { 0, 1, 0, 0 },
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
                {
                    { 0, 0, 1, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 0, 0, 0 },
                    { 1, 1, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 1, 0, 0 },
                    { 1, 1, 0, 0 },
                    { 1, 0, 0, 0 },
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
                {
                    { 0, 1, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 1, 1, 0, 0 },
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
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 0 },
                    { 1, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 1, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
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
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                },
                {
                    { 0, 1, 0, 0 },
                    { 1, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 0, 0 },
                },
            }
        },
    };
}
