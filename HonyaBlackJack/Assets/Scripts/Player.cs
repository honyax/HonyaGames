using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected Game _game;

    protected List<byte> _hand = new List<byte>();
    protected List<SpriteRenderer> _drawnCardObjs = new List<SpriteRenderer>();

    public virtual void InitializePlayer(Game game)
    {
        _game = game;
        _hand.Clear();

        foreach (var drawnCardObj in _drawnCardObjs)
        {
            Destroy(drawnCardObj.gameObject);
        }
        _drawnCardObjs.Clear();
    }

    public void ReceiveCards(byte[] cards)
    {
        foreach (var card in cards) {
            this.ReceiveCard(card);
        }
    }
    public virtual void ReceiveCard(byte card)
    {
        _hand.Add(card);
    }

    public byte CalcPoint()
    {
        var hasAce = false;
        var totalPoint = 0;
        foreach (var card in _hand)
        {
            var point = card % 13 + 1;
            point = Mathf.Min(point, 10);
            totalPoint += point;
            if (point == 1)
                hasAce = true;
        }

        if (hasAce && totalPoint <= 11)
            totalPoint += 10;

        if (totalPoint > 21)
            totalPoint = 0;

        return (byte)totalPoint;
    }
}
