using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //[SerializeField]
    float Acceleration = 40f;

    //[SerializeField]
    float MaxSpeed = 5f;

    //[SerializeField]
    float JumpSpeed = 25f;

    //[SerializeField]
    float Gravity = 80f;

    //[SerializeField]
    float Repulsion = 0.5f;

    Rigidbody2D _rb;
    RaycastHit2D[] _hitResults;

    Vector2 _velocity = Vector2.zero;
    Vector2 _movement = Vector2.zero;
    ContactFilter2D _contactFilter;
    BoxCollider2D _boxCollider;
    Animator _anim;
    SpriteRenderer _renderer;

    public bool IsGrounded { get; private set; } = false;

    private Vector2 FrontPos
    {
        get
        {
            var pos = transform.position;
            var frontPos = new Vector2(pos.x + _boxCollider.offset.x, pos.y + _boxCollider.offset.y);
            if (_state == EnemyState.LeftMove)
            {
                frontPos.x -= _boxCollider.size.x / 2;
            }
            else
            {
                frontPos.x += _boxCollider.size.x / 2;
            }
            return frontPos;
        }
    }

    enum EnemyState
    {
        None,
        LeftMove,
        RightMove,
    }
    EnemyState _state = EnemyState.None;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hitResults = new RaycastHit2D[16];
        _contactFilter.useTriggers = false;
        _contactFilter.useLayerMask = false;
        _boxCollider = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();

        _state = EnemyState.RightMove;
    }

    void Update()
    {
        UpdateState();
        UpdateVelocity();
        UpdateMovement();
        UpdatePosition();
        UpdateAnimator();
    }

    void UpdateState()
    {
        if (IsWallFront() || IsCliffFront())
        {
            switch (_state)
            {
                case EnemyState.LeftMove:
                    _state = EnemyState.RightMove;
                    break;
                case EnemyState.RightMove:
                default:
                    _state = EnemyState.LeftMove;
                    break;
            }
        }
    }

    /// <summary>目の前に壁があるか</summary>
    bool IsWallFront()
    {
        var pos = FrontPos;
        var count = Physics2D.Linecast(new Vector2(pos.x - 0.1f, pos.y), new Vector2(pos.x + 0.1f, pos.y), _contactFilter, _hitResults);
        for (var i = 0; i < count; i++)
        {
            if (_hitResults[i].rigidbody != _rb)
                return true;
        }
        return false;
    }

    /// <summary>目の前に崖があるか</summary>
    bool IsCliffFront()
    {
        var pos = FrontPos;
        var count = Physics2D.Linecast(pos, new Vector2(pos.x, pos.y - _boxCollider.size.y / 2 - 0.1f), _contactFilter, _hitResults);
        for (var i = 0; i < count; i++)
        {
            if (_hitResults[i].rigidbody != _rb)
                return false;
        }
        return true;
    }

    void UpdateVelocity()
    {
        var pad = Pad.Instance;

        // X方向の速度
        var xDeltaSpeed = Acceleration * Time.deltaTime;
        if (_state == EnemyState.LeftMove)
        {
            _velocity.x = Mathf.Max(-MaxSpeed, _velocity.x - xDeltaSpeed);
        }
        else if (_state == EnemyState.RightMove)
        {
            _velocity.x = Mathf.Min(MaxSpeed, _velocity.x + xDeltaSpeed);
        }
        else
        {
            if (_velocity.x > 0)
            {
                _velocity.x = Mathf.Max(0, _velocity.x - xDeltaSpeed);
            }
            else
            {
                _velocity.x = Mathf.Min(0, _velocity.x + xDeltaSpeed);
            }
        }

        // Y方向の速度
        _velocity.y -= Gravity * Time.deltaTime;

        if (IsGrounded)
        {
        }
    }

    void UpdateMovement()
    {
        _movement = _velocity * Time.deltaTime;

        IsGrounded = false;

        var distance = _movement.magnitude;
        if (distance < 0.001f)
            return;

        var pos = transform.position;
        if (_movement.y < 0)
        {
            if (IsHit(pos, Vector2.down, -_movement.y))
            {
                // 着地
                _velocity.y = Mathf.Max(_velocity.y, 0);
                IsGrounded = true;
            }
        }

        if (_movement.y > 0)
        {
            if (IsHit(pos, Vector2.up, _movement.y))
            {
                // 天井
                _velocity.y = Mathf.Min(_velocity.y, 0);
            }
        }

        if (_movement.x < 0)
        {
            if (IsHit(pos, Vector2.left, -_movement.x))
            {
                // 左の壁
                _velocity.x = Mathf.Max(_velocity.x, Repulsion);
            }
        }

        if (_movement.x > 0)
        {
            if (IsHit(pos, Vector2.right, _movement.x))
            {
                // 右の壁
                _velocity.x = Mathf.Min(_velocity.x, -Repulsion);
            }
        }

        _movement = _velocity * Time.deltaTime;
    }

    bool IsHit(Vector2 origin, Vector2 direction, float distance)
    {
        var pos = origin + _boxCollider.offset;
        var size = _boxCollider.size;
        var count = Physics2D.BoxCast(pos, size * 0.8f, 0, direction, _contactFilter, _hitResults, distance + 0.1f);
        for (var i = 0; i < count; i++)
        {
            if (_hitResults[i].rigidbody != _rb)
            {
                if (Vector2.Dot(direction, _hitResults[i].normal) < -0.9f)
                    return true;
            }
        }
        return false;
    }

    void UpdatePosition()
    {
        var pos = transform.localPosition;
        pos.x += _movement.x;
        pos.y += _movement.y;
        transform.localPosition = pos;
    }

    void UpdateAnimator()
    {
        if (!IsGrounded)
        {
        }
        else if (_movement.x > 0.01f)
        {
            _renderer.flipX = false;
        }
        else if (_movement.x < -0.01f)
        {
            _renderer.flipX = true;
        }
        else
        {
        }
    }
}
