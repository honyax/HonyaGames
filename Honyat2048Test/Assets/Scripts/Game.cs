using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : SingletonMonoBehaviour<Game>
{
    [SerializeField]
    private Block _blockPrefab;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private GameObject _gameOverText;

    private List<Block> _blocks = new List<Block>();
    private Block _currentBlock = null;
    private DateTime _thrownTime;
    private int _id = 0;
    private List<Tuple<Block, Block>> _hitPairBlocks = new List<Tuple<Block, Block>>();
    private int _totalScore = 0;

    private enum GameState
    {
        None,
        Initialize,
        SpawnBlock,
        WaitForDrag,
        Dragging,
        ThrowBlock,
        Thrown,
        GameOver,
    }

    private GameState _state = GameState.None;

    void Start()
    {
        Physics.gravity = Vector3.down * 4.0f;
        Block.InitializeBlockColors();
        _state = GameState.Initialize;
    }

    private void Update()
    {
        switch (_state)
        {
            case GameState.Initialize:
                foreach (var b in _blocks)
                {
                    Destroy(b.gameObject);
                }
                _blocks.Clear();
                _currentBlock = null;
                _thrownTime = DateTime.MinValue;
                _id = 0;
                _hitPairBlocks.Clear();
                _totalScore = 0;
                _gameOverText.SetActive(false);
                _scoreText.text = _totalScore.ToString();
                _state = GameState.SpawnBlock;
                return;

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

            case GameState.GameOver:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _state = GameState.Initialize;
                }
                return;

            case GameState.None:
            default:
                break;
        }

        foreach (var pair in _hitPairBlocks)
        {
            var growBlock = pair.Item1;
            var destroyBlock = pair.Item2;
            growBlock.GrowUp(destroyBlock.transform.position, destroyBlock.Rb);
            _blocks.Remove(destroyBlock);
            Destroy(destroyBlock.gameObject);
            _totalScore += growBlock.Score;

            _scoreText.text = _totalScore.ToString();
        }
        _hitPairBlocks.Clear();

        foreach (var block in _blocks)
        {
            if (block.BadZoneSeconds > 3)
            {
                _gameOverText.SetActive(true);
                _state = GameState.GameOver;
            }
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

    public void HitBlocks(Block a, Block b)
    {
        // IDÇÃè¨Ç≥Ç¢ï˚Ç Item1, ëÂÇ´Ç¢ï˚Ç Item2 Ç…Ç∑ÇÈ
        var pair = a.Id < b.Id ? Tuple.Create(a, b) : Tuple.Create(b, a);
        _hitPairBlocks.Add(pair);
    }
}
