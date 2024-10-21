using System.Collections.Generic;
using UnityEngine;

public class SmallTreeGeneration : GenericTreeGeneration<SmallTreeGeneration>
{
    public SmallTreeGeneration() : base(
        depth: new Vector2Int(10, 14),
        radius: new Vector2(0.03f, 0.08f),
        cylinderStep: 7,
        trunkSplitChance: 0.3f, splitChance: 0.3f,
        splitRotate: 50f, nonSplitRotate: 10f,
        trunkHeight: new Vector2(0.6f, 1f), branchLength: new Vector2(0.6f, 0.6f),
        leavesCount: 4, nonEndLeafChance: 0.25f,
        crossRenderLeaves: true,
        leavesDim: new Vector2(1f, 3f), leavesScale: new Vector2(0.25f, 0.35f),
        leavesRotationRange: new Vector3(45f, 180f, 180f), leavesRotationOffset: new Vector3(-90f, 0f, 0f)
    ) { }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, chunkSize / 1.5f, seed: seed);
        return fpds.fill();
    }

    public override int PreGenCount() { return 30; }
}
