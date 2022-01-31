using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    RuntimeAnimatorController _idle;

    [SerializeField]
    RuntimeAnimatorController _run;

    [SerializeField]
    RuntimeAnimatorController _jump;

    [SerializeField]
    GameObject _enemyDeathPrefab;

    //[SerializeField]
    float Acceleration = 40f;

    //[SerializeField]
    float MaxSpeed = 10f;

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
    GameObject _enemyDeath;

    int _enemyLayer = 0;

    public bool IsGrounded { get; private set; } = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _hitResults = new RaycastHit2D[16];
        _contactFilter.useTriggers = false;
        _contactFilter.useLayerMask = false;
        _boxCollider = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        _enemyDeath = Instantiate(_enemyDeathPrefab, Vector3.zero, Quaternion.identity);
        _enemyDeath.SetActive(false);
    }

    void Update()
    {
        UpdateVelocity();
        UpdateMovement();
        UpdatePosition();
        UpdateAnimator();
    }

    void UpdateVelocity()
    {
        var pad = Pad.Instance;

        // X方向の速度
        var xDeltaSpeed = Acceleration * Time.deltaTime;
        if (pad.L)
        {
            _velocity.x = Mathf.Max(-MaxSpeed, _velocity.x - xDeltaSpeed);
        }
        else if (pad.R)
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
            if (pad.A)
            {
                _velocity.y = JumpSpeed;
            }
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
            _anim.runtimeAnimatorController = _jump;
        }
        else if (_movement.x > 0.01f)
        {
            _anim.runtimeAnimatorController = _run;
            _renderer.flipX = false;
        }
        else if (_movement.x < -0.01f)
        {
            _anim.runtimeAnimatorController = _run;
            _renderer.flipX = true;
        }
        else
        {
            _anim.runtimeAnimatorController = _idle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitGo = collision.gameObject;
        if (hitGo.layer == _enemyLayer)
        {
            var myCollisionBottom = transform.position.ToVector2() + _boxCollider.offset - Vector2.up * (_boxCollider.size.y / 4);
            var hitCollisionPos = hitGo.transform.position.ToVector2() + collision.offset;
            if (myCollisionBottom.y > hitCollisionPos.y)
            {
                Destroy(hitGo);
                StartCoroutine(EnemyDeathEffect(hitGo.transform.position));
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
    private IEnumerator EnemyDeathEffect(Vector3 pos)
    {
        _enemyDeath.transform.position = pos;
        _enemyDeath.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        _enemyDeath.SetActive(false);
    }
}
