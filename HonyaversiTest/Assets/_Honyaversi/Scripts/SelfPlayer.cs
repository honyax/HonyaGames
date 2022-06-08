using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfPlayer : BasePlayer
{
    public override bool TryGetSelected(out int x, out int z)
    {
        return base.TryGetSelected(out x, out z);
    }

    private void Update()
    {
        switch (Game.Instance.CurrentState)
        {
            case Game.State.None:
            case Game.State.Initializing:
            case Game.State.WhiteTurn:
                break;

            case Game.State.BlackTurn:
                ExecTurn();
                break;

            case Game.State.Result:
                // TODO
                break;
        }
    }

    private int showingTurnDot = 0;

    private void ExecTurn()
    {
        var currentTurn = Game.Instance.CurrentTurn;
        if (showingTurnDot != currentTurn)
        {
            ShowDots();
            showingTurnDot = currentTurn;
        }
    }

    private void ShowDots()
    {
        // TODO
    }
}
