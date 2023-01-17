using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rounding
{
    // �����_���ۂ߂�
    public static Vector2 Round(Vector2 fix)
    {
        return new Vector2(Mathf.Round(fix.x), Mathf.Round(fix.y));
    }
    public static Vector3 Round(Vector3 fix)
    {
        return new Vector3(Mathf.Round(fix.x), Mathf.Round(fix.y));
    }
}
