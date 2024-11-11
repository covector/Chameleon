using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class JitterPoissonSampling : FastPoissonDiskSampling
{
    float strength;
    Vector2Int jitterCount;

    public JitterPoissonSampling(float width, float height, float spacing, float strength, Vector2Int jitterCount, int seed = 0) :
         base(width, height, spacing, seed: seed)
    {
        this.strength = strength;
        this.jitterCount = jitterCount;
    }

    public new List<Vector2> fill()
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> poisson = base.fill();
        
        foreach (Vector2 p in poisson)
        {
            int count = RandomRange(random, jitterCount);
            for (int i = 0; i < count; i++)
            {
                float subStrength = (float) random.NextDouble() * strength;
                points.Add(new Vector2(
                    p.x + RandomRange(random, -subStrength, subStrength),
                    p.y + RandomRange(random, -subStrength, subStrength)
                ));
            }
        }
        return points;
    }
}
