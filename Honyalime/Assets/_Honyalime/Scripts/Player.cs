using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private enum State
    {
        None,
        Grounded,
        Jumping,
    }

    [SerializeField]
    private float _jumpVelocity = 10.0f;

    [SerializeField]
    private float _gravity = -9.8f;

    [SerializeField]
    private float _moveAccelaration = 25.0f;

    [SerializeField]
    private float _moveMaxVelocity = 5.0f;

    private Transform _cachedTransform;
    private SphereCollider _cachedCollider;
    private float _velocityX;
    private float _velocityY;
    private State _state = State.None;

    private void Start()
    {
        _cachedTransform = transform;
        _cachedCollider = GetComponent<SphereCollider>();
    }

    public void Initialize()
    {
        transform.position = new Vector3(0, _cachedCollider.radius, 0);
        _velocityX = 0;
        _velocityY = 0;
        _state = State.Grounded;
    }

    public void UpdatePlaying()
    {
        var kb = Keyboard.current;
        if (kb.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
        {
            Move(-_moveMaxVelocity);
        }
        else if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
        {
            Move(_moveMaxVelocity);
        }
        else
        {
            Move(0);
        }

        UpdatePosition();
    }

    private void Move(float toVelocity)
    {
        if (_velocityX < toVelocity)
        {
            _velocityX += _moveAccelaration * Time.deltaTime;
            _velocityX = Mathf.Min(_velocityX, toVelocity);
        }
        else
        {
            _velocityX -= _moveAccelaration * Time.deltaTime;
            _velocityX = Mathf.Max(_velocityX, toVelocity);
        }
    }

    private void Jump()
    {
        if (Mathf.Abs(_velocityY) > 0)
            return;

        _velocityY = _jumpVelocity;
        _state = State.Jumping;
    }

    private void UpdatePosition()
    {
        var deltaX = _velocityX * Time.deltaTime;
        var deltaY = _velocityY * Time.deltaTime;
        var pos = _cachedTransform.position;
        pos.x += deltaX;
        pos.y += deltaY;
        if (_state != State.Grounded)
        {
            _velocityY += _gravity * Time.deltaTime;
            if (pos.y <= _cachedCollider.radius)
            {
                pos.y = _cachedCollider.radius;
                _velocityY = 0;
                _state = State.Grounded;
            }
        }
        var maxX = 1.0f * 10 / 2;
        if (Mathf.Abs(pos.x) > maxX)
        {
            pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
            _velocityX = 0;
        }
        _cachedTransform.position = pos;
    }

    public void HitWall(Wall wall)
    {
        Game.Instance.GameOver();
    }
}
