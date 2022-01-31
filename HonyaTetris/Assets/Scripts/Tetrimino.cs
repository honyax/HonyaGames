using System.Collections.Generic;
using UnityEngine;

public class Tetrimino
{
    private Vector2Int _basePosition;
    private int _rollPattern;

    public Game.TetriminoType Type { get; private set; }
    public int[,,] TypePattern { get { return TypePatterns[Type]; } }

    public Vector2Int[] GetBlockPositions()
    {
        return GetBlockPositions(_rollPattern);
    }

    public Vector2Int[] GetBlockPositions(int rollPattern)
    {
        var positions = new Vector2Int[4];

        var yNum = TypePattern.GetLength(1);
        var xNum = TypePattern.GetLength(2);
        var index = 0;
        for (var x = 0; x < xNum; x++)
        {
            for (var y = 0; y < yNum; y++)
            {
                if (TypePattern[rollPattern, y, x] == 1)
                {
                    positions[index].Set(_basePosition.x + x, _basePosition.y + y);
                    index++;
                }
            }
        }

#if false
        positions[0] = _basePosition;
        positions[1].Set(_basePosition.x, _basePosition.y + 1);
        positions[2].Set(_basePosition.x, _basePosition.y + 2);
        positions[3].Set(_basePosition.x, _basePosition.y + 3);
#endif

        return positions;
    }

    public void Initialize()
    {
        Type = (Game.TetriminoType)Random.Range(1, 8);
        _rollPattern = 0;
        _basePosition.Set(BasePositions[Type], 0);
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

    public Vector2Int[] GetRolledBlockPositions()
    {
        var rollPattern = _rollPattern + 1;
        var rollNum = TypePattern.GetLength(0);
        if (rollPattern == rollNum)
        {
            rollPattern = 0;
        }
        return GetBlockPositions(rollPattern);
    }

    public void Roll()
    {
        _rollPattern++;
        var rollNum = TypePattern.GetLength(0);
        if (_rollPattern == rollNum)
        {
            _rollPattern = 0;
        }
    }

    readonly Dictionary<Game.TetriminoType, int> BasePositions = new Dictionary<Game.TetriminoType, int>()
    {
        { Game.TetriminoType.TypeI, 3 },
        { Game.TetriminoType.TypeO, 4 },
        { Game.TetriminoType.TypeS, 3 },
        { Game.TetriminoType.TypeZ, 3 },
        { Game.TetriminoType.TypeJ, 3 },
        { Game.TetriminoType.TypeL, 3 },
        { Game.TetriminoType.TypeT, 3 },
    };

    readonly Dictionary<Game.TetriminoType, int[,,]> TypePatterns = new Dictionary<Game.TetriminoType, int[,,]>()
    {
        { Game.TetriminoType.TypeI, TypeIPatterns },
        { Game.TetriminoType.TypeO, TypeOPatterns },
        { Game.TetriminoType.TypeS, TypeSPatterns },
        { Game.TetriminoType.TypeZ, TypeZPatterns },
        { Game.TetriminoType.TypeJ, TypeJPatterns },
        { Game.TetriminoType.TypeL, TypeLPatterns },
        { Game.TetriminoType.TypeT, TypeTPatterns },
    };
    
    static readonly int[,,] TypeIPatterns = new int[,,]
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
    };

    static readonly int[,,] TypeOPatterns = new int[,,]
    {
        {
            { 1, 1 },
            { 1, 1 },
        },
    };

    static readonly int[,,] TypeSPatterns = new int[,,]
    {
        {
            { 0, 1, 1 },
            { 1, 1, 0 },
            { 0, 0, 0 },
        },
        {
            { 0, 1, 0 },
            { 0, 1, 1 },
            { 0, 0, 1 },
        },
        {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 1, 1, 0 },
        },
        {
            { 1, 0, 0 },
            { 1, 1, 0 },
            { 0, 1, 0 },
        },
    };

    static readonly int[,,] TypeZPatterns = new int[,,]
    {
        {
            { 1, 1, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 },
        },
        {
            { 0, 0, 1 },
            { 0, 1, 1 },
            { 0, 1, 0 },
        },
        {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 0, 1, 1 },
        },
        {
            { 0, 1, 0 },
            { 1, 1, 0 },
            { 1, 0, 0 },
        },
    };

    static readonly int[,,] TypeJPatterns = new int[,,]
    {
        {
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        },
        {
            { 0, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
        },
        {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 1 },
        },
        {
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 0 },
        },
    };

    static readonly int[,,] TypeLPatterns = new int[,,]
    {
        {
            { 0, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        },
        {
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 1 },
        },
        {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 0 },
        },
        {
            { 1, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
        },
    };

    static readonly int[,,] TypeTPatterns = new int[,,]
    {
        {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        },
        {
            { 0, 1, 0 },
            { 0, 1, 1 },
            { 0, 1, 0 },
        },
        {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
        },
        {
            { 0, 1, 0 },
            { 1, 1, 0 },
            { 0, 1, 0 },
        },
    };
}
