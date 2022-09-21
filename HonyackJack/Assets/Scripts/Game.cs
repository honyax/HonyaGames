using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : SingletonMonoBehaviour<Game>
{
    [SerializeField]
    private SelfPlayer _selfPlayer;

    [SerializeField]
    private EnemyPlayer _enemyPlayer;

    [SerializeField]
    private SpriteRenderer _cardPrefab;
    public SpriteRenderer CardPrefab { get { return _cardPrefab; } }

    [SerializeField]
    private Text _resultText;

    [SerializeField]
    private Sprite _backSprite;
    public Sprite BackSprite { get { return _backSprite; } }

    [SerializeField]
    private Sprite[] _sprites;
    public Sprite[] Sprites { get { return _sprites; } }

    public enum GameState
    {
        None,
        Initialize,
        PlayerTurn,
        EnemyTurn,
        Result,
    }
    public GameState State { get; private set; } = GameState.None;

    int[] _cards;
    int _cardIndex = 0;

    bool _hitClicked = false;
    bool _standClicked = false;
    bool _retryClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        State = GameState.Initialize;
    }

    void InitializeCards()
    {
        _cardIndex = 0;
        _cards = new int[52];
        for (var i = 0; i < _cards.Length; i++)
        {
            _cards[i] = i;
        }
        for (var i = 0; i < _cards.Length; i++)
        {
            var randomIndex = Random.Range(0, _cards.Length);
            var tmp = _cards[i];
            _cards[i] = _cards[randomIndex];
            _cards[randomIndex] = tmp;
        }
    }

    void DealCards()
    {
        _selfPlayer.ReceiveCard(_cards[0]);
        _selfPlayer.ReceiveCard(_cards[1]);
        _enemyPlayer.ReceiveCard(_cards[2]);
        _enemyPlayer.ReceiveCard(_cards[3]);
        _cardIndex += 4;
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.Initialize:
                _selfPlayer.Initialize();
                _enemyPlayer.Initialize();
                InitializeCards();
                DealCards();
                _resultText.gameObject.SetActive(false);
                State = GameState.PlayerTurn;
                break;

            case GameState.PlayerTurn:
                if (_hitClicked)
                {
                    _hitClicked = false;
                    _selfPlayer.ReceiveCard(_cards[_cardIndex]);
                    _cardIndex++;
                }
                else if (_standClicked || _selfPlayer.CalcPoint() == 0)
                {
                    _standClicked = false;
                    _enemyPlayer.CardOpen();
                    State = GameState.EnemyTurn;
                }
                break;

            case GameState.EnemyTurn:
                if (_enemyPlayer.ThinkHit())
                {
                    _enemyPlayer.ReceiveCard(_cards[_cardIndex]);
                    _cardIndex++;
                }
                else if(_enemyPlayer.ThinkStand())
                {
                    State = GameState.Result;
                    ExecResult();
                }
                break;

            case GameState.Result:
                if (_retryClicked)
                {
                    _retryClicked = false;
                    State = GameState.Initialize;
                }
                break;

            case GameState.None:
                break;
        }
    }

    void ExecResult()
    {
        var selfPoint = _selfPlayer.CalcPoint();
        var enemyPoint = _enemyPlayer.CalcPoint();
        var isWin = selfPoint > enemyPoint;
        _resultText.text = isWin ? "Win" : "Lose";
        _resultText.gameObject.SetActive(true);
        Debug.Log($"{_cards[0]%13+1}, {_cards[1]%13+1}, {_cards[2]%13+1}, {_cards[3]%13+1}");
        Debug.Log($"SelfPoint:{selfPoint} EnemyPoint:{enemyPoint} WIN:{isWin}");
    }

    public void OnHitClicked()
    {
        //Debug.Log("Hit!!");
        if (State == GameState.PlayerTurn)
        {
            _hitClicked = true;
        }
    }

    public void OnStandClicked()
    {
        //Debug.Log("Stand!!");
        if (State == GameState.PlayerTurn)
        {
            _standClicked = true;
        }
    }

    public void OnRetryClicked()
    {
        if (State == GameState.Result)
        {
            _retryClicked = true;
        }
    }
}
