using UnityEngine;

public class Utils
{
    public static Vector2 ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }
}
