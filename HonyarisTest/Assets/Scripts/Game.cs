using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    const int FieldXLength = 10;
    const int FieldYLength = 20;

    [SerializeField]
    private GameObject _field;

    [SerializeField]
    private SpriteRenderer _blockPrefab;

    private SpriteRenderer[,] _blockObjects;

    private Tetrimino _tetrimino = new Tetrimino();
    private float _fallInterval;
    private DateTime _lastFallTime;
    private DateTime _lastControlTime;

    private BlockType[,] _fieldBlocks;

    public enum BlockType
    {
        None = 0,
        TetriminoI = 1,
    }

    private void Start()
    {
        InitializeBlockObjects();
        Initialize();
        Draw();
    }

    private void InitializeBlockObjects()
    {
        _blockObjects = new SpriteRenderer[FieldXLength, FieldYLength];
        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                var block = Instantiate(_blockPrefab, _field.transform);
                block.transform.localPosition = new Vector3(x, y, 0);
                block.transform.localRotation = Quaternion.identity;
                block.transform.localScale = Vector3.one;
                block.color = Color.black;
                _blockObjects[x, y] = block;
            }
        }
    }

    private void Initialize()
    {
        _tetrimino.Initialize(BlockType.TetriminoI);
        _fallInterval = 0.3f;
        _lastFallTime = DateTime.UtcNow;
        _lastControlTime = DateTime.UtcNow;

        _fieldBlocks = new BlockType[FieldXLength, FieldYLength];
        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                _fieldBlocks[x, y] = BlockType.None;
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
                // TODO: FIX TETRIMINO AND NEXT
            }
        }

        Draw();
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
            if (_fieldBlocks[x, y] != BlockType.None)
                return false;
        }

        return true;
    }

    private void Draw()
    {
        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                var blockObj = _blockObjects[x, y];
                var blockType = _fieldBlocks[x, y];
                blockObj.color = GetBlockColor(blockType);
            }
        }

        var positions = _tetrimino.GetBlockPositions();
        foreach (var position in positions)
        {
            var x = position.x;
            var y = position.y;
            if (0 <= x && x < FieldXLength && 0 <= y && y < FieldYLength)
            {
                var tetriminoBlock = _blockObjects[x, y];
                tetriminoBlock.color = GetBlockColor(_tetrimino.BlockType);
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
            default:
                return Color.white;
        }
    }
}
