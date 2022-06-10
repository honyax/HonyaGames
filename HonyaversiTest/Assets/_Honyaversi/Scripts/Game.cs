using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Game : SingletonMonoBehaviour<Game>
{
    public const int XNUM = 8;
    public const int ZNUM = 8;

    public enum State
    {
        None,
        Initializing,
        BlackTurn,
        WhiteTurn,
        Result,
    }

    [SerializeField]
    private Stone _stonePrefab;

    [SerializeField]
    private Transform _stoneBase;

    [SerializeField]
    private SelfPlayer _selfPlayer;

    [SerializeField]
    private EnemyPlayer _enemyPlayer;

    [SerializeField]
    private GameObject _cursor;

    [SerializeField]
    private TextMeshPro _blackScoreText;

    [SerializeField]
    private TextMeshPro _whiteScoreText;

    public State CurrentState { get; private set; } = State.None;

    public GameObject Cursor { get { return _cursor; } }

    public int CurrentTurn {
        get
        {
            var turnCount = 0;
            for (var z = 0; z < ZNUM; z++)
            {
                for (var x = 0; x < XNUM; x++)
                {
                    if (_stones[z][x].CurrentState != Stone.State.None)
                        turnCount++;
                }
            }
            return turnCount;
        }
    }

    private Stone[][] _stones;
    public Stone[][] Stones { get { return _stones; } }

    private void Start()
    {
        _stones = new Stone[ZNUM][];
        for (var z = 0; z < ZNUM; z++)
        {
            _stones[z] = new Stone[XNUM];
            for (var x = 0; x < XNUM; x++)
            {
                var stone = Instantiate(_stonePrefab, _stoneBase);
                var t = stone.transform;
                t.localPosition = new Vector3(x * 10, 0, z * 10);
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                _stones[z][x] = stone;
            }
        }

        _cursor.SetActive(false);
        CurrentState = State.Initializing;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Initializing:
                for (var z = 0; z < ZNUM; z++)
                {
                    for (var x = 0; x < XNUM; x++)
                    {
                        _stones[z][x].SetActive(false, Stone.Color.Black);
                    }
                }

                _stones[3][3].SetActive(true, Stone.Color.Black);
                _stones[3][4].SetActive(true, Stone.Color.White);
                _stones[4][3].SetActive(true, Stone.Color.White);
                _stones[4][4].SetActive(true, Stone.Color.Black);
                UpdateScore();

                CurrentState = State.BlackTurn;
                break;

            case State.BlackTurn:
                {
                    if (_selfPlayer.TryGetSelected(out var x, out var z))
                    {
                        _stones[z][x].SetActive(true, Stone.Color.Black);
                        Reverse(Stone.Color.Black, x, z);
                        UpdateScore();
                        if (IsGameFinished())
                        {
                            CurrentState = State.Result;
                        }
                        else
                        {
                            if (_enemyPlayer.CanPut())
                            {
                                CurrentState = State.WhiteTurn;
                            }
                            else if (!_selfPlayer.CanPut())
                            {
                                CurrentState = State.Result;
                            }
                        }
                    }
                }
                break;
            case State.WhiteTurn:
                {
                    if (_enemyPlayer.TryGetSelected(out var x, out var z))
                    {
                        _stones[z][x].SetActive(true, Stone.Color.White);
                        Reverse(Stone.Color.White, x, z);
                        UpdateScore();
                        if (IsGameFinished())
                        {
                            CurrentState = State.Result;
                        }
                        else
                        {
                            if (_selfPlayer.CanPut())
                            {
                                CurrentState = State.BlackTurn;
                            }
                            else if (!_enemyPlayer.CanPut())
                            {
                                CurrentState = State.Result;
                            }
                        }
                    }
                }
                break;

            case State.Result:
                {
                    var kb = Keyboard.current;
                    if (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
                    {
                        CurrentState = State.Initializing;
                    }
                }
                break;

            case State.None:
            default:
                break;
        }
    }

    private void UpdateScore()
    {
        var blackScore = 0;
        var whiteScore = 0;
        for (var z = 0; z < ZNUM; z++)
        {
            for (var x = 0; x < XNUM; x++)
            {
                if (_stones[z][x].CurrentState != Stone.State.None)
                {
                    switch (_stones[z][x].CurrentColor)
                    {
                        case Stone.Color.Black:
                            blackScore++;
                            break;
                        case Stone.Color.White:
                            whiteScore++;
                            break;
                    }
                }
            }
        }

        _blackScoreText.text = blackScore.ToString();
        _whiteScoreText.text = whiteScore.ToString();
    }

    private void Reverse(Stone.Color color, int putX, int putZ)
    {
        for (var dirZ = -1; dirZ <= 1; dirZ++)
        {
            for (var dirX = -1; dirX <= 1; dirX++)
            {
                var reverseCount = CalcReverseCount(color, putX, putZ, dirX, dirZ);

                for (var i = 1; i <= reverseCount; i++)
                {
                    _stones[putZ + dirZ * i][putX + dirX * i].Reverse();
                }
            }
        }
    }

    private int CalcReverseCount(Stone.Color color, int putX, int putZ, int dirX, int dirZ)
    {
        var x = putX;
        var z = putZ;
        var reverseCount = 0;
        for (var i = 0; i < 8; i++)
        {
            x += dirX;
            z += dirZ;
            if (x < 0 || XNUM <= x || z < 0 || ZNUM <= z)
            {
                reverseCount = 0;
                break;
            }

            var stone = _stones[z][x];
            if (stone.CurrentState == Stone.State.None)
            {
                reverseCount = 0;
                break;
            }
            else
            {
                if (stone.CurrentColor != color)
                {
                    reverseCount++;
                }
                else
                {
                    break;
                }
            }
        }

        return reverseCount;
    }

    public int CalcTotalReverseCount(Stone.Color color, int putX, int putZ)
    {
        var totalReverseCount = 0;
        for (var dirZ = -1; dirZ <= 1; dirZ++)
        {
            for (var dirX = -1; dirX <= 1; dirX++)
            {
                totalReverseCount += CalcReverseCount(color, putX, putZ, dirX, dirZ);
            }
        }

        return totalReverseCount;
    }

    private bool IsGameFinished()
    {
        for (var z = 0; z < ZNUM; z++)
        {
            for (var x = 0; x < XNUM; x++)
            {
                if (_stones[z][x].CurrentState == Stone.State.None)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
