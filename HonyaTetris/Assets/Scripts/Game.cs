using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    const int FieldXLength = 10;
    const int FieldYLength = 20;

    [SerializeField]
    private GameObject _field;

    [SerializeField]
    private SpriteRenderer _blockPrefab;

    [SerializeField]
    private GameObject _gameOver;

    enum GameScene
    {
        None,
        Initialize,
        Running,
        Result,
    }

    public enum TetriminoType
    {
        None,
        TypeI,
        TypeO,
        TypeS,
        TypeZ,
        TypeJ,
        TypeL,
        TypeT,
    }

    private GameScene _gameScene = GameScene.None;

    private Tetrimino _tetrimino = new Tetrimino();

    private SpriteRenderer[,] _blockObjects;
    private TetriminoType[, ] _fieldSpaces;

    private DateTime _lastFallTime;
    private TimeSpan FallElapsed { get { return DateTime.UtcNow - _lastFallTime; } }
    private float _fallInterval;

    private DateTime _lastControlTime;
    private TimeSpan ControlElapsed { get { return DateTime.UtcNow - _lastControlTime; } }
    const float ControlInterval = 0.1f;

    private void Initialize()
    {
        _gameScene = GameScene.Initialize;
        _tetrimino.Initialize();
        _lastFallTime = DateTime.UtcNow;
        _lastControlTime = DateTime.UtcNow;
        _fallInterval = 0.3f;
        _gameOver.SetActive(false);

        _fieldSpaces = new TetriminoType[FieldXLength, FieldYLength];
        for (var x = 0; x < FieldXLength; x++)
        {
            for (var y = 0; y < FieldYLength; y++)
            {
                _fieldSpaces[x, y] = TetriminoType.None;
            }
        }
        UpdateBlockColors();

        _gameScene = GameScene.Running;
    }

    private void InstantiateBlocks()
    {
        _blockObjects = new SpriteRenderer[FieldXLength, FieldYLength];
        for (var x = 0; x < FieldXLength; x++)
        {
            for (var y = 0; y < FieldYLength; y++)
            {
                var block = Instantiate(_blockPrefab, Vector3.zero, Quaternion.identity, _field.transform);
                block.transform.localPosition = new Vector3(x, y, 0);
                block.color = Color.black;
                _blockObjects[x, y] = block;
            }
        }
    }

    void Start()
    {
        InstantiateBlocks();
        Initialize();
    }

    void Update()
    {
        switch (_gameScene)
        {
            case GameScene.Running:
                UpdatePlay();
                break;

            case GameScene.Result:
                UpdateResult();
                break;

            case GameScene.None:
            case GameScene.Initialize:
                break;
        }
    }

    void UpdatePlay()
    {
        ControlTetrimino();

        if (FallElapsed.TotalSeconds < _fallInterval)
            return;

        _lastFallTime = DateTime.UtcNow;
        if (CanMoveDownTetrimino())
        {
            _tetrimino.MoveDown();
            UpdateBlockColors();
        }
        else
        {
            FixTetrimino();

            DeleteLines();

            _tetrimino.Initialize();

            UpdateBlockColors();

            if (!CanCreateTetrimino())
            {
                _gameScene = GameScene.Result;
                _gameOver.SetActive(true);
            }
        }
    }

    void UpdateResult()
    {
        if (Input.GetMouseButton(0))
        {
            Initialize();
        }
    }

    void ControlTetrimino()
    {
        if (ControlElapsed.TotalSeconds < ControlInterval)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (CanMoveLeftTetrimino())
            {
                _tetrimino.MoveLeft();
                _lastControlTime = DateTime.UtcNow;
                UpdateBlockColors();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (CanMoveRightTetrimino())
            {
                _tetrimino.MoveRight();
                _lastControlTime = DateTime.UtcNow;
                UpdateBlockColors();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (CanMoveDownTetrimino())
            {
                _tetrimino.MoveDown();
                _lastControlTime = DateTime.UtcNow;
                UpdateBlockColors();
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            if (CanRollTetrimino())
            {
                _tetrimino.Roll();
                _lastControlTime = DateTime.UtcNow;
                UpdateBlockColors();
            }
        }
    }

    bool CanMoveTetrimino(int xDelta, int yDelta)
    {
        var blockPositions = _tetrimino.GetBlockPositions();
        foreach (var blockPosition in blockPositions)
        {
            var movedX = blockPosition.x + xDelta;
            var movedY = blockPosition.y + yDelta;
            if (movedX < 0 || movedX >= FieldXLength)
                return false;
            if (movedY < 0 || movedY >= FieldYLength)
                return false;
            if (_fieldSpaces[movedX, movedY] != TetriminoType.None)
                return false;
        }

        return true;
    }

    bool CanMoveDownTetrimino()
    {
        return CanMoveTetrimino(0, 1);
    }

    bool CanMoveLeftTetrimino()
    {
        return CanMoveTetrimino(-1, 0);
    }

    bool CanMoveRightTetrimino()
    {
        return CanMoveTetrimino(1, 0);
    }

    bool CanCreateTetrimino()
    {
        return CanMoveTetrimino(0, 0);
    }

    bool CanRollTetrimino()
    {
        var blockPositions = _tetrimino.GetRolledBlockPositions();
        foreach (var blockPosition in blockPositions)
        {
            var movedX = blockPosition.x;
            var movedY = blockPosition.y;
            if (movedX < 0 || movedX >= FieldXLength)
                return false;
            if (movedY < 0 || movedY >= FieldYLength)
                return false;
            if (_fieldSpaces[movedX, movedY] != TetriminoType.None)
                return false;
        }

        return true;
    }

    void FixTetrimino()
    {
        var blockPositions = _tetrimino.GetBlockPositions();
        foreach (var blockPosition in blockPositions)
        {
            _fieldSpaces[blockPosition.x, blockPosition.y] = _tetrimino.Type;
        }
    }

    void DeleteLines()
    {
        var deleteLineNumbers = new List<int>();
        for (var y = 0; y < FieldYLength; y++)
        {
            var hasBlank = false;
            for (var x = 0; x < FieldXLength; x++)
            {
                if (_fieldSpaces[x, y] == TetriminoType.None)
                {
                    hasBlank = true;
                    break;
                }
            }

            if (!hasBlank)
            {
                deleteLineNumbers.Add(y);
            }
        }

        for (var y = FieldYLength - 1; y >= 0; y--)
        {
            if (deleteLineNumbers.Contains(y))
                continue;

            var downLineCount = deleteLineNumbers.Where(line => line > y).Count();
            if (downLineCount == 0)
                continue;

            for (var x = 0; x < FieldXLength; x++)
            {
                _fieldSpaces[x, y + downLineCount] = _fieldSpaces[x, y];
                _fieldSpaces[x, y] = TetriminoType.None;
            }
        }
    }

    void UpdateBlockColors()
    {
        UpdateFieldBlockColors();
        UpdateTetriminoBlockColors();
    }

    void UpdateFieldBlockColors()
    {
        for (var x = 0; x < FieldXLength; x++)
        {
            for (var y = 0; y < FieldYLength; y++)
            {
                _blockObjects[x, y].color = ColorPatterns[_fieldSpaces[x, y]];
            }
        }
    }

    void UpdateTetriminoBlockColors()
    {
        var positions = _tetrimino.GetBlockPositions();
        var color = ColorPatterns[_tetrimino.Type];
        foreach (var pos in positions)
        {
            _blockObjects[pos.x, pos.y].color = color;
        }
    }

    readonly Dictionary<Game.TetriminoType, Color> ColorPatterns = new Dictionary<TetriminoType, Color>()
    {
        { TetriminoType.None, Color.black },
        { TetriminoType.TypeI, Color.cyan },
        { TetriminoType.TypeO, Color.yellow },
        { TetriminoType.TypeS, Color.green },
        { TetriminoType.TypeZ, Color.red },
        { TetriminoType.TypeJ, Color.blue },
        { TetriminoType.TypeL, new Color(1, 0.5f, 0, 1) },
        { TetriminoType.TypeT, Color.magenta },
    };
}
