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

    public Color CurrentColor { get; private set; } = Color.Black;
    public State CurrentState { get; private set; } = State.None;

    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this.CurrentColor = color;
            this.CurrentState = State.Fix;

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
