using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public enum Color
    {
        Black,
        White,
    }

    public enum State
    {
        None,
        Appearing,
        Reversing,
        Fix,
    }

    [SerializeField]
    private GameObject _black;

    [SerializeField]
    private GameObject _white;

    [SerializeField]
    private GameObject _dot;

    public Color CurrentColor { get; private set; } = Color.Black;
    public State CurrentState { get; private set; } = State.None;

    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this.CurrentColor = color;
            this.CurrentState = State.Fix;
            this._black.SetActive(true);
            this._white.SetActive(true);
            this._dot.SetActive(false);

            switch (color)
            {
                case Color.Black:
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Color.White:
                    transform.localRotation = Quaternion.Euler(0, 0, 180);
                    break;
            }
        }
        else
        {
            this.CurrentState = State.None;
        }

        gameObject.SetActive(value);
    }

    public void EnableDot()
    {
        this._black.SetActive(false);
        this._white.SetActive(false);
        this._dot.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Reverse()
    {
        if (CurrentState == State.None)
        {
            Debug.LogError("Invalid Stone State");
            return;
        }

        switch (CurrentColor)
        {
            case Color.Black:
                CurrentColor = Color.White;
                break;
            case Color.White:
                CurrentColor = Color.Black;
                break;
        }
        CurrentState = State.Fix;
    }
}
