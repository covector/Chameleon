using System.Collections.Generic;
using UnityEngine;

public class JitterGridSampling
{
    float spacing;
    float strength;
    Vector2 globalPos;
    float halfWidth;
    float halfHeight;
    System.Random random;

    public JitterGridSampling(float width, float height, float spacing, float strength, Vector2 globalPos, int seed = 0)
    {
        this.spacing = spacing;
        this.strength = strength;
        this.globalPos = globalPos;
        this.halfWidth = width / 2f;
        this.halfHeight = height / 2f;
        this.random = new System.Random(seed);
    }

    public List<Vector2> fill()
    {
        List<Vector2> points = new List<Vector2>();
        float xOffset = Mathf.Repeat(globalPos.x, spacing);
        float yOffset = Mathf.Repeat(globalPos.y, spacing);
        for (float x = -halfWidth; x < halfWidth; x += spacing)
        {
            for (float y = -halfHeight; y < halfHeight; y += spacing)
            {
                points.Add(new Vector2(
                    x + xOffset + strength * (Mathf.PerlinNoise1D(random.Next(10000)) - 0.5f),
                    y + yOffset + strength * (Mathf.PerlinNoise1D(random.Next(10000)) - 0.5f)
                ));
            }
        }
        return points;
    }
}
