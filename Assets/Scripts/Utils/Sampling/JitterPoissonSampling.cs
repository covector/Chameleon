using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class JitterPoissonSampling : FastPoissonDiskSampling
{
    protected float strength;
    protected float singleProb;
    protected Vector2Int jitterCount;

    public JitterPoissonSampling(float width, float height, float spacing, float strength, Vector2Int jitterCount, int seed = 0, float singleProb = 0f) :
         base(width, height, spacing, seed: seed)
    {
        this.strength = strength;
        this.jitterCount = jitterCount;
        this.singleProb = singleProb;
    }

    public new List<Vector2> fill()
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> poisson = base.fill();
        
        foreach (Vector2 p in poisson)
        {
            if (random.NextDouble() < singleProb)
            {
                points.Add(p);
            }
            else
            {
                int count = RandomRange(random, jitterCount);
                for (int i = 0; i < count; i++)
                {
                    points.Add(new Vector2(
                        p.x + TriangleDistr(random, strength),
                        p.y + TriangleDistr(random, strength)
                    ));
                }
            }
        }
        return points;
    }
}
