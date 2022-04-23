using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    const int FieldXLength = 10;
    const int FieldYLength = 20;
    const int NextFieldXLength = 4;
    const int NextFieldYLength = 4;

    [SerializeField]
    private GameObject _field;

    [SerializeField]
    private GameObject _nextField;

    [SerializeField]
    private SpriteRenderer _blockPrefab;

    private SpriteRenderer[,] _blockObjects;
    private SpriteRenderer[,] _nextBlockObjects;

    private Tetrimino _tetrimino = new Tetrimino();
    private Tetrimino _nextTetrimino = new Tetrimino();
    private float _fallInterval;
    private DateTime _lastFallTime;
    private DateTime _lastControlTime;

    private BlockType[,] _fieldBlocks;

    public enum BlockType
    {
        None = 0,
        TetriminoI = 1,
        TetriminoO = 2,
        TetriminoS = 3,
        TetriminoZ = 4,
        TetriminoJ = 5,
        TetriminoL = 6,
        TetriminoT = 7,
    }

    private void Start()
    {
        InitializeBlockObjects();
        Initialize();
        Draw();
    }

    private void InitializeBlockObjects()
    {
        _blockObjects = new SpriteRenderer[FieldYLength, FieldXLength];
        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                var block = Instantiate(_blockPrefab, _field.transform);
                block.transform.localPosition = new Vector3(x, y, 0);
                block.transform.localRotation = Quaternion.identity;
                block.transform.localScale = Vector3.one;
                block.color = Color.black;
                _blockObjects[y, x] = block;
            }
        }

        _nextBlockObjects = new SpriteRenderer[NextFieldYLength, NextFieldXLength];
        for (var y = 0; y < NextFieldYLength; y++)
        {
            for (var x = 0; x < NextFieldXLength; x++)
            {
                var block = Instantiate(_blockPrefab, _nextField.transform);
                block.transform.localPosition = new Vector3(x, y, 0);
                block.transform.localRotation = Quaternion.identity;
                block.transform.localScale = Vector3.one;
                block.color = Color.black;
                _nextBlockObjects[y, x] = block;
            }
        }

        _fieldBlocks = new BlockType[FieldYLength, FieldXLength];
    }

    private void Initialize()
    {
        _tetrimino.Initialize();
        _nextTetrimino.Initialize();
        _fallInterval = 0.3f;
        _lastFallTime = DateTime.UtcNow;
        _lastControlTime = DateTime.UtcNow;

        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                _fieldBlocks[y, x] = BlockType.None;
            }
        }
    }

    private void Update()
    {
        var controlled = ControlTetrimino();

        var now = DateTime.UtcNow;
        if ((now - _lastFallTime).TotalSeconds < _fallInterval)
        {
            if (!controlled)
                return;
        }
        else
        {
            _lastFallTime = now;

            if (!TryMoveTetrimino(0, 1))
            {
                var positions = _tetrimino.GetBlockPositions();
                foreach (var position in positions)
                {
                    _fieldBlocks[position.y, position.x] = _tetrimino.BlockType;
                }
                DeleteLines();

                _tetrimino.Initialize(_nextTetrimino.BlockType);
                _nextTetrimino.Initialize();
            }
        }

        Draw();
    }

    private void DeleteLines()
    {
        for (var y = FieldYLength - 1; y >= 0; )
        {
            var hasBlank = false;
            for (var x = 0; x < FieldXLength; x++)
            {
                if (_fieldBlocks[y, x] == BlockType.None)
                {
                    hasBlank = true;
                    break;
                }
            }
            if (hasBlank)
            {
                y--;
                continue;
            }

            for (var downY = y; downY >= 0; downY--)
            {
                for (var x = 0; x < FieldXLength; x++)
                {
                    _fieldBlocks[downY, x] = downY == 0 ? BlockType.None : _fieldBlocks[downY - 1, x];
                }
            }
        }
    }

    private bool ControlTetrimino()
    {
        var now = DateTime.UtcNow;
        if ((now - _lastControlTime).TotalSeconds < 0.1f)
            return false;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (TryMoveTetrimino(-1, 0))
            {
                _lastControlTime = now;
                return true;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (TryMoveTetrimino(1, 0))
            {
                _lastControlTime = now;
                return true;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (TryMoveTetrimino(0, 1))
            {
                _lastControlTime = now;
                return true;
            }
        }

        return false;
    }

    private bool TryMoveTetrimino(int deltaX, int deltaY)
    {
        if (CanMoveTetrimino(deltaX, deltaY))
        {
            _tetrimino.Move(deltaX, deltaY);
            return true;
        }

        return false;
    }

    private bool CanMoveTetrimino(int deltaX, int deltaY)
    {
        var blockPositions = _tetrimino.GetBlockPositions();
        foreach (var blockPosition in blockPositions)
        {
            var x = blockPosition.x + deltaX;
            var y = blockPosition.y + deltaY;
            if (x < 0 || x >= FieldXLength)
                return false;
            if (y < 0 || y >= FieldYLength)
                return false;
            if (_fieldBlocks[y, x] != BlockType.None)
                return false;
        }

        return true;
    }

    private void Draw()
    {
        // Draw Field
        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                var blockObj = _blockObjects[y, x];
                var blockType = _fieldBlocks[y, x];
                blockObj.color = GetBlockColor(blockType);
            }
        }

        // Draw Tetrimino
        {
            var positions = _tetrimino.GetBlockPositions();
            var color = GetBlockColor(_tetrimino.BlockType);
            foreach (var position in positions)
            {
                var tetriminoBlock = _blockObjects[position.y, position.x];
                tetriminoBlock.color = color;
            }
        }

        // Draw Next Field
        for (var y = 0; y < NextFieldYLength; y++)
        {
            for (var x = 0; x < NextFieldXLength; x++)
            {
                _nextBlockObjects[y, x].color = GetBlockColor(BlockType.None);
            }
        }

        // Draw Next Tetrimino
        {
            var positions = _nextTetrimino.GetBlockPositions();
            var color = GetBlockColor(_nextTetrimino.BlockType);
            foreach (var position in positions)
            {
                var tetriminoBlock = _nextBlockObjects[position.y, position.x];
                tetriminoBlock.color = color;
            }
        }
    }

    private Color GetBlockColor(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.None:
                return Color.black;
            case BlockType.TetriminoI:
                return Color.cyan;
            case BlockType.TetriminoO:
                return Color.yellow;
            case BlockType.TetriminoS:
                return Color.green;
            case BlockType.TetriminoZ:
                return Color.red;
            case BlockType.TetriminoJ:
                return Color.blue;
            case BlockType.TetriminoL:
                return new Color(1, 0.5f, 0);
            case BlockType.TetriminoT:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
}
