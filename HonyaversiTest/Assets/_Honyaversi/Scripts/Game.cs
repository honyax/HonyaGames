using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    const int XNUM = 8;
    const int ZNUM = 8;

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

    public State CurrentState { get; private set; } = State.None;

    private Stone[][] _stones;

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

                CurrentState = State.BlackTurn;
                break;

            case State.BlackTurn:
                {
                    if (TryGetSelected(out var x, out var z))
                    {
                        _stones[z][x].SetActive(true, Stone.Color.Black);
                        Reverse(Stone.Color.Black, x, z);
                        if (IsGameFinished())
                        {
                            CurrentState = State.Result;
                        }
                        else
                        {
                            CurrentState = State.WhiteTurn;
                        }
                    }
                }
                break;
            case State.WhiteTurn:
                {
                    if (TryGetSelected(out var x, out var z))
                    {
                        _stones[z][x].SetActive(true, Stone.Color.White);
                        Reverse(Stone.Color.White, x, z);
                        if (IsGameFinished())
                        {
                            CurrentState = State.Result;
                        }
                        else
                        {
                            CurrentState = State.BlackTurn;
                        }
                    }
                }
                break;

            case State.Result:
                break;

            case State.None:
            default:
                break;
        }
    }

    private bool TryGetSelected(out int x, out int z)
    {
        // TODO
        x = 0;
        z = 0;
        return false;
    }

    private void Reverse(Stone.Color color, int putX, int putZ)
    {
        for (var dirZ = -1; dirZ <= 1; dirZ++)
        {
            for (var dirX = -1; dirX <= 1; dirX++)
            {
                int x = putX;
                int z = putZ;
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

                for (var i = 0; i < reverseCount; i++)
                {
                    _stones[putZ + dirZ * i][putX + dirX * i].Reverse();
                }
            }
        }
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
