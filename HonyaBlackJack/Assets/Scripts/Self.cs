using UnityEngine;

public class Self : Player
{
    bool drawClicked = false;
    bool keepClicked = false;

    public override void InitializePlayer(Game game)
    {
        base.InitializePlayer(game);

        drawClicked = false;
        keepClicked = false;
    }

    private void Update()
    {
        if (_game.State != Game.GameState.PlayerTurn)
            return;

        if (drawClicked)
        {
            var card = _game.DrawCard();
            this.ReceiveCard(card);

            if (CalcPoint() == 0)
            {
                _game.TurnEnd();
            }
        }
        else if (keepClicked)
        {
            _game.TurnEnd();
        }

        drawClicked = false;
        keepClicked = false;
    }

    public void OnDrawClicked()
    {
        drawClicked = true;
    }

    public void OnKeepClicked()
    {
        keepClicked = true;
    }

    public override void ReceiveCard(byte card)
    {
        base.ReceiveCard(card);

        var cardObj = Instantiate(_game._cardPrefab, transform);
        cardObj.sprite = _game._sprites[card];
        cardObj.sortingOrder = _drawnCardObjs.Count;
        _drawnCardObjs.Add(cardObj);

        var interval = 5f / (_drawnCardObjs.Count + 1);
        var xPos = -2.5f;
        foreach (var drawnCardObj in _drawnCardObjs)
        {
            xPos += interval;
            drawnCardObj.transform.localPosition = new Vector3(xPos, -2, 0);
        }
    }
}
