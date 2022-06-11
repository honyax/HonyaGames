using System;
using UnityEngine;

public class Stone : MonoBehaviour
{
    static readonly float AppeareSeconds = 0.5f;
    static readonly float ReverseSeconds = 0.5f;

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

    private DateTime _stateChangedAt = DateTime.MinValue;
    private float ElapsedSecondsSinceStateChange { get { return (float)(DateTime.UtcNow - _stateChangedAt).TotalSeconds; } }

    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this.CurrentColor = color;
            this.CurrentState = State.Appearing;
            this._black.SetActive(true);
            this._white.SetActive(true);
            this._dot.SetActive(false);
            this._stateChangedAt = DateTime.UtcNow;

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
                //transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case Color.White:
                CurrentColor = Color.Black;
                //transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
        }
        this.CurrentState = State.Reversing;
        this._stateChangedAt = DateTime.UtcNow;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Appearing:
                // position
                {
                    var startPos = transform.localPosition;
                    var endPos = transform.localPosition;
                    startPos.y = 3;
                    endPos.y = 0;
                    var t = Mathf.Clamp01(1 - ElapsedSecondsSinceStateChange / AppeareSeconds);
                    t = 1 - t * t * t * t;
                    transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                    if (AppeareSeconds < ElapsedSecondsSinceStateChange)
                    {
                        transform.localPosition = endPos;
                        CurrentState = State.Fix;
                    }
                }
                break;

            case State.Reversing:
                {
                    // rotation
                    var startRot = Quaternion.identity;
                    var endRot = Quaternion.identity;
                    switch (CurrentColor)
                    {
                        case Color.Black:
                            startRot = Quaternion.Euler(0, 0, 180);
                            endRot = Quaternion.Euler(0, 0, 0);
                            break;

                        case Color.White:
                            startRot = Quaternion.Euler(0, 0, 0);
                            endRot = Quaternion.Euler(0, 0, 180);
                            break;
                    }
                    var t = Mathf.Clamp01(1 - ElapsedSecondsSinceStateChange / ReverseSeconds);
                    t = 1 - t * t * t * t;
                    transform.localRotation = Quaternion.Lerp(startRot, endRot, t);

                    // position
                    var maxY = 5f;
                    t = Mathf.Clamp01(ElapsedSecondsSinceStateChange / ReverseSeconds);
                    var pos = transform.localPosition;
                    pos.y = maxY * Mathf.Sin(t * Mathf.PI);
                    transform.localPosition = pos;

                    if (ReverseSeconds < ElapsedSecondsSinceStateChange)
                    {
                        var endPos = transform.localPosition;
                        endPos.y = 0;
                        transform.localPosition = endPos;
                        transform.localRotation = endRot;
                        CurrentState = State.Fix;
                    }
                }
                break;

            case State.None:
            case State.Fix:
                break;
        }
    }
}
