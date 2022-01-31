using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Self _self;

    [SerializeField]
    private Enemy _enemy;

    [SerializeField]
    private Text _winCountText;

    [SerializeField]
    private Text _loseCountText;

    [SerializeField]
    private Text _resultText;

    [SerializeField]
    public SpriteRenderer _cardPrefab;

    [SerializeField]
    public Sprite _backSprite;

    [SerializeField]
    public Sprite[] _sprites;

    public enum GameState
    {
        None,
        Initialize,
        PlayerTurn,
        EnemyTurn,
        Result,
    }

    public GameState State { get; private set; } = GameState.None;

    /// <summary>
    ///  0 - 12: spade
    /// 13 - 25: heart
    /// 26 - 38: diamond
    /// 39 - 51: club
    /// </summary>
    byte[] _cards;

    private byte _cardIndex;

    private ushort _winCount = 0;
    private ushort _loseCount = 0;

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Result:
                if (Input.GetMouseButton(0))
                {
                    InitializeGame();
                }
                break;

            case GameState.None:
            case GameState.Initialize:
            case GameState.PlayerTurn:
            case GameState.EnemyTurn:
            default:
                break;
        }
    }

    private void InitializeGame()
    {
        State = GameState.Initialize;
        InitializeCards();
        _self.InitializePlayer(this);
        _enemy.InitializePlayer(this);
        _resultText.gameObject.SetActive(false);

        DealCards();

        State = GameState.PlayerTurn;
    }

    private void InitializeCards()
    {
        _cardIndex = 0;

        _cards = new byte[52];
        for (byte i = 0; i < _cards.Length; i++)
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

    private void DealCards()
    {
        _self.ReceiveCards(new byte[] { _cards[0], _cards[1] });
        _enemy.ReceiveCards(new byte[] { _cards[2], _cards[3] });
        _cardIndex += 4;
    }

    public byte DrawCard()
    {
        var card = _cards[_cardIndex];
        _cardIndex++;
        return card;
    }

    public void TurnEnd()
    {
        switch (State)
        {
            case GameState.PlayerTurn:
                State = GameState.EnemyTurn;
                break;
            case GameState.EnemyTurn:
                State = GameState.Result;
                ExecResult();
                break;
        }
    }

    void ExecResult()
    {
        var selfPoint = _self.CalcPoint();
        var enemyPoint = _enemy.CalcPoint();
        Debug.Log($"Self:{selfPoint} Enemy:{enemyPoint}");
        if (selfPoint > enemyPoint)
        {
            _winCount++;
            _resultText.text = "Win";
        }
        else
        {
            _loseCount++;
            _resultText.text = "Lose";
        }

        _resultText.gameObject.SetActive(true);
        _winCountText.text = _winCount.ToString();
        _loseCountText.text = _loseCount.ToString();
    }
}
