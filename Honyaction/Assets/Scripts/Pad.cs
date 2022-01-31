using UnityEngine;

public class Pad : SingletonMonoBehaviour<Pad>
{
    [SerializeField]
    SpriteRenderer _u;

    [SerializeField]
    SpriteRenderer _l;

    [SerializeField]
    SpriteRenderer _r;

    [SerializeField]
    SpriteRenderer _d;

    [SerializeField]
    SpriteRenderer _a;

    readonly Color _upColor = Color.white;
    readonly Color _downColor = Color.red;

    public bool U { get; private set; } = false;
    public bool L { get; private set; } = false;
    public bool R { get; private set; } = false;
    public bool D { get; private set; } = false;
    public bool A { get; private set; } = false;

    void Update()
    {
        UpdateInput();
        UpdateColor();
    }

    void UpdateInput()
    {
        // U
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            U = true;
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            U = false;
        // L
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            L = true;
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            L = false;
        // R
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            R = true;
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            R = false;
        // D
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            D = true;
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            D = false;
        // A
        if (Input.GetKeyDown(KeyCode.Space))
            A = true;
        else if (Input.GetKeyUp(KeyCode.Space))
            A = false;
    }

    void UpdateColor()
    {
        _u.color = GetColor(U);
        _l.color = GetColor(L);
        _r.color = GetColor(R);
        _d.color = GetColor(D);
        _a.color = GetColor(A);
    }

    Color GetColor(bool isDown)
    {
        return isDown ? _downColor : _upColor;
    }
}
