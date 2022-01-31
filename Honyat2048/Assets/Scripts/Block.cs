using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _hitEffectPrefab;

    [SerializeField]
    private TextMeshPro[] _scoreTexts;

    private const float BreakTime = 5.0f;

    public enum BlockState
    {
        None,
        Spawned,
        Dragging,
        Thrown,
        Breaking,
        Broken,
    }

    BlockState _state = BlockState.None;
    public BlockState State { get { return _state; } }

    private bool MouseButtonPushed { get { return Input.GetMouseButton(0); } }

    private int _sequenceNumber;
    public int SequenceNumber { get { return _sequenceNumber; } }

    private int _point;
    public int Point { get { return _point; } }

    private Game _parent;
    private float _min;
    private float _max;
    private float _breakLine;

    private Material _material;
    private Color _baseColor;

    private float _breakSeconds = 0;

    private AudioSource _audioSource;

    void Start()
    {
    }

    public void Initialize(Game parent, int sequenceNumber, int point, float min, float max, float breakLine)
    {
        _state = BlockState.Spawned;
        GetComponent<Rigidbody>().isKinematic = true;
        _sequenceNumber = sequenceNumber;
        _point = point;
        _parent = parent;
        _min = min;
        _max = max;
        _breakLine = breakLine;
        _material = gameObject.GetComponent<MeshRenderer>().material;
        _audioSource = gameObject.GetComponent<AudioSource>();
        UpdatePoint();
    }

    void Update()
    {
        switch (_state)
        {
            case BlockState.Spawned:
                if (MouseButtonPushed)
                {
                    _state = BlockState.Dragging;
                }
                break;
            case BlockState.Dragging:

                Drag();

                if (!MouseButtonPushed)
                {
                    Throw();
                    _state = BlockState.Thrown;
                }
                break;

            case BlockState.Thrown:
                if (transform.position.z > _breakLine)
                {
                    _state = BlockState.Breaking;
                }
                break;

            case BlockState.Breaking:
                if (transform.position.z < _breakLine)
                {
                    _state = BlockState.Thrown;
                    _material.color = _baseColor;
                }
                else
                {
                    _breakSeconds += Time.deltaTime;

                    if (_breakSeconds > BreakTime)
                    {
                        _baseColor = Color.black;
                        _material.color = _baseColor;
                        _parent.GameOver();
                    }
                    else
                    {
                        var t = (Mathf.Sin(_breakSeconds * 10) + 1) / 2;
                        _material.color = Color.Lerp(_baseColor, Color.white, t);
                    }
                }
                break;

            case BlockState.Broken:
            case BlockState.None:
            default:
                break;
        }
    }

    void Drag()
    {
        var mouse = Input.mousePosition;
        var target = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 5));
        //Debug.Log($"{mouse}, {target}, {transform.position}");
        target.x = Mathf.Clamp(target.x, _min, _max);
        target.y = transform.position.y;
        target.z = transform.position.z;
        this.transform.position = target;
    }

    void Throw()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(Vector3.back * 750);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision:{collision.gameObject.name}");
        var opponent = collision.gameObject.GetComponent<Block>();
        if (opponent == null)
            return;

        // 生成時間が古いほうが残る
        if (opponent.Point == _point && opponent.SequenceNumber > _sequenceNumber)
        {
            var myPos = transform.position;
            var opponentPos = opponent.transform.position;

            // blockの場所を2つの中間地点に強制的に移動
            transform.position = (myPos + opponentPos) / 2;

            // 衝突時に威力生成
            var opponentRb = collision.gameObject.GetComponent<Rigidbody>();
            var rb = GetComponent<Rigidbody>();
            var vec = (myPos - opponentPos).normalized;
            rb.AddForce(vec * opponentRb.velocity.magnitude * opponentRb.mass * 10);

            Destroy(collision.gameObject);
            _audioSource.PlayOneShot(_audioSource.clip);

            Instantiate(_hitEffectPrefab, opponentPos, Quaternion.identity);

            _point++;
            _breakSeconds = 0;
            UpdatePoint();
        }
    }

    private static Color[] _blockColors;
    private void InitializeBlockColors()
    {
        const int SNUM = 4;
        const int HNUM = 6;
        const float SSPACE = 1f / (SNUM - 1);
        const float HSPACE = 1f / HNUM;

        _blockColors = new Color[HNUM * SNUM];

        for (var sCnt = 0; sCnt < SNUM; sCnt++)
        {
            for (var hCnt = 0; hCnt < HNUM; hCnt++)
            {
                var h = HSPACE * hCnt + (HSPACE / 2) * (sCnt % 2);
                var s = 1f - SSPACE * sCnt;
                var v = 1f - 0.15f * sCnt;
                var index = sCnt * HNUM + hCnt;
                _blockColors[index] = Color.HSVToRGB(h, s, v);
            }
        }
    }

    private void UpdatePoint()
    {
        if (_blockColors == null)
        {
            InitializeBlockColors();
        }

        Color color = Color.black;
        if (_point < _blockColors.Length)
        {
            color = _blockColors[_point];
        }

        _baseColor = color;
        _material.color = _baseColor;

        var score = 1;
        for (var i = 0; i < _point; i++)
        {
            score *= 2;
        }

        foreach (var scoreText in _scoreTexts)
        {
            scoreText.text = score.ToString();
        }
    }
}
