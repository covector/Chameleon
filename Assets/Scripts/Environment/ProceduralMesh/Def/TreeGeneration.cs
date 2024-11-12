using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : GenericTreeGeneration<TreeGeneration>
{
    public TreeGeneration() : base(
        depth: 15,
        radius: new Vector2(0.1f, 0.4f),
        cylinderStep: 9,
        trunkSplitChance: 0.05f, splitChance: 0.4f,
        splitRotate: 25f, splitRadiusFactor: 0.6f, nonSplitRotate: 10f,
        branchLength: 2.5f, branchLengthFactor: 0.8f,
        leavesCount: 6, startLeaveDepth: 4,
        crossRenderLeaves: false,
        leavesDim: new Vector2(1f, 1f), leavesScale: new Vector2(0.2f, 0.5f),
        leavesRotationRange: new Vector3(45f, 90f, 180f), leavesRotationOffset: new Vector3(-45f, 0f, 0f)
    )
    { }
    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        const float spacing = 4f;
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, spacing, seed: seed);
        return fpds.fill();
    }

    public override bool FilterPoint(float globalX, float globalZ, int maskSeed)
    {
        const float size = 10f;
        const float threshold = 0.5f;
        return Mathf.PerlinNoise(globalX * size, globalZ * size + maskSeed / 1000) > threshold;
    }

    public override int PreGenCount() { return 40; }

    public override bool ItemSpawnCheck() { return true; }
    public override bool CollisionCheck() { return true; }
}
