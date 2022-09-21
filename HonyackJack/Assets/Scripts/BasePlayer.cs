using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    protected List<int> _hand = new List<int>();

    protected List<SpriteRenderer> _drawCardObjs = new List<SpriteRenderer>();

    public void Initialize()
    {
        _hand.Clear();
        foreach (var drawCardObj in _drawCardObjs)
        {
            Destroy(drawCardObj.gameObject);
        }
        _drawCardObjs.Clear();
    }

    public virtual void ReceiveCard(int card)
    {
        _hand.Add(card);
    }

    public int CalcPoint()
    {
        var totalPoint = 0;
        var hasAce = false;
        foreach (var card in _hand)
        {
            var point = card % 13 + 1;
            point = Mathf.Min(point, 10);
            totalPoint += point;
            if (point == 1)
            {
                hasAce = true;
            }
        }

        if (hasAce && totalPoint <= 11)
        {
            totalPoint += 10;
        }

        if (totalPoint > 21)
        {
            totalPoint = 0;
        }

        return totalPoint;
    }
}
