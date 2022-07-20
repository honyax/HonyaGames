using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3 ToVec3(this Vector2 vec2, float z = 0)
    {
        return new Vector3(vec2.x, vec2.y, z);
    }
}
