using UnityEngine;

public class Enemy : Player
{
    bool showFirstCard = false;

    public override void InitializePlayer(Game game)
    {
        base.InitializePlayer(game);

        showFirstCard = false;
    }

    private void Update()
    {
        if (_game.State != Game.GameState.EnemyTurn)
            return;

        if (!showFirstCard)
        {
            showFirstCard = true;
            _drawnCardObjs[0].sprite = _game._sprites[_hand[0]];
        }

        if (CalcPoint() == 0 || CalcPoint() > 15)
        {
            _game.TurnEnd();
            return;
        }

        var card = _game.DrawCard();
        this.ReceiveCard(card);
    }

    public override void ReceiveCard(byte card)
    {
        base.ReceiveCard(card);

        var cardObj = Instantiate(_game._cardPrefab, transform);
        if (_drawnCardObjs.Count == 0)
        {
            cardObj.sprite = _game._backSprite;
        }
        else
        {
            cardObj.sprite = _game._sprites[card];
        }
        cardObj.sortingOrder = _drawnCardObjs.Count;
        _drawnCardObjs.Add(cardObj);

        var interval = 5f / (_drawnCardObjs.Count + 1);
        var xPos = -2.5f;
        foreach (var drawnCardObj in _drawnCardObjs)
        {
            xPos += interval;
            drawnCardObj.transform.localPosition = new Vector3(xPos, 2, 0);
        }
    }
}
