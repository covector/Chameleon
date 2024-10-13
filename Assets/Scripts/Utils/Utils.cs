using System;
using UnityEngine;

public class Utils
{
    public static Vector2 ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector2Int GetChunkIndFromCoord(float x, float z, float chunkSize)
    {
        return new Vector2Int(Mathf.FloorToInt(x / chunkSize), Mathf.FloorToInt(z / chunkSize));
    }

    public static Vector2Int GetChunkIndFromCoord(Vector3 loc, float chunkSize)
    {
        return GetChunkIndFromCoord(loc.x, loc.z, chunkSize);
    }

    // r > 1
    public static void MidPointCircle(int r, Action<int, int> callback)
    {
        int x = r, y = 0;
        int P = 1 - r;
        for (int i = x; i >= y; i--)
        {
            callback(i, 0);
            if (i != y)
            {
                callback(0, i);
                callback(-i, 0);
                callback(0, -i);
            }
        }
        while (x > y)
        {
            y++;
            if (P <= 0) { P += 2 * y + 1; }
            else { x--; P += 2 * y - 2 * x + 1; }
            if (x < y) { break; }

            for (int i = x; i >= y; i--)
            {
                callback(i, y);
                callback(i, -y);
                callback(-i, y);
                callback(-i, -y);
                if (i != y)
                {
                    callback(y, i);
                    callback(-y, i);
                    callback(y, -i);
                    callback(-y, -i);
                }
            }
        }
    }
}