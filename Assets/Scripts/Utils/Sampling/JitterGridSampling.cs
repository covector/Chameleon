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

    public JitterGridSampling(float width, float height, float spacing, float strength, Vector3 globalPos, int seed = 0)
    {
        this.spacing = spacing;
        this.strength = strength;
        this.globalPos = Utils.ToVector2(globalPos);
        this.halfWidth = width / 2f;
        this.halfHeight = height / 2f;
        this.random = new System.Random(seed);
    }

    public List<Vector2> fill()
    {
        List<Vector2> points = new List<Vector2>();
        float xOffset = Mathf.Repeat(Mathf.Abs(globalPos.x), spacing);
        float yOffset = Mathf.Repeat(Mathf.Abs(globalPos.y), spacing);
        bool gridOffset = Mathf.Repeat(Mathf.Abs(globalPos.x), spacing * 2) < spacing;
        for (float x = -halfWidth; x < halfWidth; x += spacing)
        {
            gridOffset = !gridOffset;
            for (float y = -halfHeight; y < halfHeight; y += spacing)
            {
                points.Add(new Vector2(
                    x + xOffset + strength * (float)random.NextDouble(), 
                    y + yOffset + strength * (float)random.NextDouble() + (gridOffset ? spacing / 2f : 0f)
                ));
            }
        }
        return points;
    }
}
