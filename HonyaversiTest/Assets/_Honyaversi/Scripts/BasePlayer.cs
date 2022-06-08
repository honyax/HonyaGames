using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    public virtual bool TryGetSelected(out int x, out int z)
    {
        x = 0;
        z = 0;
        return false;
    }
}
