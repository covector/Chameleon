using System.Collections.Generic;
using UnityEngine;

public class BushGeneration : GenericTreeGeneration<BushGeneration>
{
    public BushGeneration() : base(
        new Vector2Int(8, 12),
        new Vector2(0.03f, 0.03f),
        0.1f, 0.4f,
        30f, 10f,
        new Vector2(0.15f, 0.25f), new Vector2(0.15f, 0.25f),
        5, new Vector2(0.1f, 0.2f), new Vector3(30f, 180f, 30f)
    )
    { }

    public override int PreGenCount() { return 20; }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        JitterGridSampling jgs = new JitterGridSampling(chunkSize, chunkSize, chunkSize / 6f, chunkSize / 1.2f, globalPosition - new Vector3(offset, 0, offset), seed);
        return jgs.fill();
    }

    public override bool FilterPoint(float globalX, float globalZ, int maskSeed)
    {
        const float size = 0.5f;
        const float threshold = 0.4f;
        return Mathf.PerlinNoise(globalX * size, globalZ * size + maskSeed / 1000) > threshold;
    }
}
