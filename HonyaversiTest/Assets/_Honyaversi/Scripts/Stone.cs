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

    private Color _color = Color.Black;
    private State _state = State.None;

    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this._color = color;
            this._state = State.Fix;

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
            this._state = State.None;
        }

        gameObject.SetActive(value);
    }
}
