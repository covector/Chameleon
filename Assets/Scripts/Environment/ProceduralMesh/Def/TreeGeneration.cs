using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : GenericTreeGeneration<TreeGeneration>
{
    public TreeGeneration() : base(
        depth: new Vector2Int(10, 10),
        radius: new Vector2(0.05f, 0.4f),
        cylinderStep: 9,
        trunkSplitChance: 0.1f, splitChance: 0.4f,
        splitRotate: 30f, nonSplitRotate: 10f,
        trunkHeight: new Vector2(1f, 6f), branchLength: new Vector2(1f, 1f),
        leavesCount: 4, nonEndLeafChance: 0.1f,
        crossRenderLeaves: false,
        leavesDim: new Vector2(1f, 3f), leavesScale: new Vector2(0.35f, 1f),
        leavesRotationRange: new Vector3(45f, 90f, 180f), leavesRotationOffset: Vector3.zero
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

    public override int PreGenCount() { return 50; }

    public override bool ItemSpawnCheck() { return true; }
    public override bool CollisionCheck() { return true; }
}
