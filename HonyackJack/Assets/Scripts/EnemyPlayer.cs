using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayer : BasePlayer
{
    public bool ThinkHit()
    {
        var point = CalcPoint();
        return 0 < point && point < 16;
    }

    public bool ThinkStand()
    {
        return !ThinkHit();
    }

    public void CardOpen()
    {
        _drawCardObjs[0].sprite = Game.Instance.Sprites[_hand[0]];
    }

    public override void ReceiveCard(int card)
    {
        base.ReceiveCard(card);

        var game = Game.Instance;
        var cardObj = Instantiate(game.CardPrefab, transform);
        if (_drawCardObjs.Count == 0)
        {
            cardObj.sprite = game.BackSprite;
        }
        else
        {
            cardObj.sprite = game.Sprites[card];
        }
        cardObj.sortingOrder = _drawCardObjs.Count;
        _drawCardObjs.Add(cardObj);

        var interval = 5f / (_drawCardObjs.Count + 1);
        var xPos = -2.5f;
        foreach (var drawnCardObj in _drawCardObjs)
        {
            xPos += interval;
            drawnCardObj.transform.localPosition = new Vector3(xPos, 2.5f, 0);
        }
    }
}
