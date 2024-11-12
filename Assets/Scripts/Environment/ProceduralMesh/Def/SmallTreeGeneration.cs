using System.Collections.Generic;
using UnityEngine;

public class SmallTreeGeneration : GenericTreeGeneration<SmallTreeGeneration>
{
    public SmallTreeGeneration() : base(
        depth: 15,
        radius: new Vector2(0.04f, 0.05f),
        cylinderStep: 7,
        trunkSplitChance: 0.3f, splitChance: 0.3f,
        splitRotate: 45f, splitRadiusFactor: 0.8f, nonSplitRotate: 10f,
        branchLength: 0.8f, branchLengthFactor: 0.8f,
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

    public override int PreGenCount() { return 30; }
}
