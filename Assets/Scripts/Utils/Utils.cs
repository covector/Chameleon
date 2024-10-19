using System;
using UnityEngine;

public class Utils
{
    public static Vector2 ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 ProjectToGround(Vector2 loc, int levels = 1)
    {
        return ProjectToGround(loc.x, loc.y, levels);
    }

    public static Vector3 ProjectToGround(float x, float z, int levels = 1)
    {
        return new Vector3(
            x,
            ChunkGeneration.GetGroudLevel(x, z, levels),
            z
        );
    }

    public static Vector2Int GetChunkIndFromCoord(float x, float z, float chunkSize)
    {
        return new Vector2Int(Mathf.FloorToInt(x / chunkSize), Mathf.FloorToInt(z / chunkSize));
    }

    public static Vector2Int GetChunkIndFromCoord(Vector3 loc, float chunkSize)
    {
        return GetChunkIndFromCoord(loc.x, loc.z, chunkSize);
    }

    public static Vector2Int GetChunkIndFromCoord(Vector2 loc, float chunkSize)
    {
        return GetChunkIndFromCoord(loc.x, loc.y, chunkSize);
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

    public static float RandomRange(System.Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public static float RandomRange(System.Random random, float max)
    {
        return (float)random.NextDouble() * max;
    }

    public static int RandomRange(System.Random random, Vector2Int range)
    {
        return range.x == range.y ? range.x : random.Next(range.x, range.y);
    }

    public static float RandomRange(System.Random random, Vector2 range)
    {
        return range.x == range.y ? range.x : RandomRange(random, range.x, range.y);
    }

    public static Matrix4x4 RandomRotation(System.Random random, Vector3 range)
    {
        return Matrix4x4.Rotate(Quaternion.Euler(
            RandomRange(random , -range.x, range.x),
            RandomRange(random, -range.y, range.y),
            RandomRange(random, -range.z, range.z)
        ));
    }

    public static Matrix4x4 RandomRotation(System.Random random, float range)
    {
        return RandomRotation(random, Vector3.one * range);
    }
}