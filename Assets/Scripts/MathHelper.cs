using UnityEngine;

public static class MathHelper
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float s = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float c = Mathf.Cos(degrees * Mathf.Deg2Rad);

        return new Vector2(
            v.x * c - v.y * s,
            v.x * s + v.y * c);
    }
}
