using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : GenericTreeGeneration<TreeGeneration>
{
    public TreeGeneration() : base(
        depth: 15,
        radius: new Vector2(0.1f, 0.4f),
        cylinderStep: 9,
        trunkSplitChance: 0.05f, minTrunkDepth: 2, trunkRotate: 5f, 
        splitChance: 0.3f, splitChanceFactor: 1.05f,
        branchRotate: 20f, branchRotateFactor: 1.1f,
        branchRadiusFactor: 0.6f, minBranchRadius: 0.01f, branchLength: 3f, branchLengthFactor: 0.8f,
        leavesCount: 4, startLeaveDepth: 4,
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

    protected override int PreGenCount() { return 40; }
    public override bool ItemSpawnCheck() { return true; }
    public override bool CollisionCheck() { return true; }
    private static List<float> RRS = new List<float> { -1, 600f };
    public override List<float> RenderRadiusSquare() { return RRS; }
}
