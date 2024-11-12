using System.Collections.Generic;
using UnityEngine;

public class BushGeneration : GenericTreeGeneration<BushGeneration>
{
    RandomAudio randomAudio;
    private void Start()
    {
        randomAudio = GetComponent<RandomAudio>();
    }

    public BushGeneration() : base(
        depth: new Vector2Int(8, 12),
        radius: new Vector2(0.02f, 0.03f),
        cylinderStep: 4,
        trunkSplitChance: 0.1f, splitChance: 0.4f,
        splitRotate: 30f, nonSplitRotate: 10f,
        trunkHeight: new Vector2(0.1f, 0.15f), branchLength: new Vector2(0.1f, 0.17f),
        leavesCount: 5, nonEndLeafChance: 0.1f,
        crossRenderLeaves: false,
        leavesDim: new Vector2(1f, 2f), leavesScale: new Vector2(0.05f, 0.075f),
        leavesRotationRange: new Vector3(30f, 180f, 30f), leavesRotationOffset: Vector3.zero
    )
    { }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        const float spacing = 8f;
        const float strength = 3f;
        Vector2Int jitterCount = new Vector2Int(4, 8);
        JitterPoissonSampling jps = new JitterPoissonSampling(chunkSize, chunkSize, spacing, strength, jitterCount, seed: seed);
        return jps.fill();
    }

    public override bool FilterPoint(float globalX, float globalZ, int maskSeed)
    {
        const float size = 1f;
        const float threshold = 0.4f;
        return Mathf.PerlinNoise(globalX * size, globalZ * size + maskSeed / 1000) > threshold;
    }

    public override float MaxDim() { return 0.6f; }

    public override bool IntersectionCheck() { return true; }
    public override void OnIntersect(float sqrSpeed)
    {
        if (!randomAudio.IsPlaying())
        {
            randomAudio.PlayRandomSound(sqrSpeed / 3f);
        }
    }

    protected float renderRadiusSquare = 250f;
    public override float RenderRadiusSquare() { return renderRadiusSquare; }
}
