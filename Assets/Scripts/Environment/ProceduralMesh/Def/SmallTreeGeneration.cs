using System.Collections.Generic;
using UnityEngine;

public class SmallTreeGeneration : GenericTreeGeneration<SmallTreeGeneration>
{
    public SmallTreeGeneration() : base(
        depth: 15,
        radius: new Vector2(0.04f, 0.05f),
        cylinderStep: 7,
        trunkSplitChance: 0.3f, minTrunkDepth: 1, trunkRotate: 10f,
        splitChance: 0.3f, splitChanceFactor: 1f,
        branchRotate: 45f, branchRotateFactor: 1f,
        branchRadiusFactor: 0.8f, minBranchRadius: 0.005f, branchLength: 0.8f, branchLengthFactor: 0.8f,
        leavesCount: 1, startLeaveDepth: 1,
        crossRenderLeaves: true,
        leavesDim: new Vector2(1f, 3f), leavesScale: new Vector2(0.25f, 0.35f),
        leavesRotationRange: new Vector3(45f, 180f, 180f), leavesRotationOffset: new Vector3(-33.67f, 0f, 0f)
    ) { }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        const float spacing = 15f;
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, spacing, seed: seed);
        return fpds.fill();
    }

    private static List<float> RRS = new List<float> { 500f, 500f };
    public override List<float> RenderRadiusSquare() { return RRS; }
}
