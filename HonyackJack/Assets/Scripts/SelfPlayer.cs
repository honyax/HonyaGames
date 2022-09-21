using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfPlayer : BasePlayer
{
    public override void ReceiveCard(int card)
    {
        base.ReceiveCard(card);

        Debug.Log($"Receive:{card % 13 + 1}");
        var game = Game.Instance;
        var cardObj = Instantiate(game.CardPrefab, transform);
        cardObj.sprite = game.Sprites[card];
        cardObj.sortingOrder = _drawCardObjs.Count;
        _drawCardObjs.Add(cardObj);

        var interval = 5f / (_drawCardObjs.Count + 1);
        var xPos = -2.5f;
        foreach (var drawCradObj in _drawCardObjs)
        {
            xPos += interval;
            drawCradObj.transform.localPosition = new Vector3(xPos, -1.5f, 0);
        }
    }
}
