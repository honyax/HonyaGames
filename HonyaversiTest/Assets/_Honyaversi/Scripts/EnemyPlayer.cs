using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayer : BasePlayer
{
    public override bool TryGetSelected(out int x, out int z)
    {
        return base.TryGetSelected(out x, out z);
    }
}
