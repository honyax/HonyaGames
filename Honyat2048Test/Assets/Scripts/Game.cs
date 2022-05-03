using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Block _blockPrefab;

    private List<Block> _blocks = new List<Block>();
    private Block _currentBlock = null;
    private DateTime _thrownTime;
    private int _id = 0;
    private List<Block> _removeBlocks = new List<Block>();

    private enum GameState
    {
        None,
        SpawnBlock,
        WaitForDrag,
        Dragging,
        ThrowBlock,
        Thrown,
    }

    private GameState _state = GameState.None;

    void Start()
    {
        _state = GameState.SpawnBlock;
    }

    private void Update()
    {
        switch (_state)
        {
            case GameState.SpawnBlock:
                var block = Instantiate(_blockPrefab, this.transform);
                _id++;
                block.Initialize(_id, UnityEngine.Random.Range(1, 7));
                _currentBlock = block;
                _blocks.Add(block);
                _state = GameState.WaitForDrag;
                break;
            case GameState.WaitForDrag:
                if (Input.GetMouseButton(0))
                {
                    _state = GameState.Dragging;
                }
                break;
            case GameState.Dragging:
                if (Input.GetMouseButtonUp(0))
                {
                    _state = GameState.ThrowBlock;
                }
                else
                {
                    DragBlock();
                }
                break;
            case GameState.ThrowBlock:
                _currentBlock.Throw();
                _thrownTime = DateTime.UtcNow;
                _state = GameState.Thrown;
                break;
            case GameState.Thrown:
                if ((DateTime.UtcNow - _thrownTime).TotalSeconds > 1)
                {
                    _state = GameState.SpawnBlock;
                }
                break;

            case GameState.None:
            default:
                break;
        }

        _removeBlocks.Clear();
        foreach (var block in _blocks)
        {
            if (block.Broken)
            {
                _removeBlocks.Add(block);
            }
        }
        foreach (var removeBlock in _removeBlocks)
        {
            _blocks.Remove(removeBlock);
            Destroy(removeBlock.gameObject);
        }
    }

    private void DragBlock()
    {
        var mousePos = Input.mousePosition;
        var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 6));
        var pos = _currentBlock.transform.localPosition;
        var targetPosX = Mathf.Clamp(targetPos.x, -2.5f, 2.5f);
        _currentBlock.transform.localPosition = new Vector3(targetPosX, pos.y, pos.z);
    }
}
