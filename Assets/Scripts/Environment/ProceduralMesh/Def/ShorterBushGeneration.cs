using System.Collections.Generic;
using UnityEngine;

public class ShorterBushGeneration : GenericTreeGeneration<ShorterBushGeneration>
{
    RandomAudio randomAudio;
    private void Start()
    {
        randomAudio = GetComponent<RandomAudio>();
    }

    public ShorterBushGeneration() : base(
        depth: 12,
        radius: new Vector2(0.02f, 0.02f),
        cylinderStep: 4,
        trunkSplitChance: 1f, minTrunkDepth: 0, trunkRotate: 30f,
        splitChance: 0.3f, splitChanceFactor: 1f,
        branchRotate: 90f, branchRotateFactor: 1f,
        branchRadiusFactor: 1f, minBranchRadius: 0.003f, branchLength: 0.1f, branchLengthFactor: 0.9f,
        leavesCount: 1, startLeaveDepth: 8,
        crossRenderLeaves: false,
        leavesDim: new Vector2(1f, 2f), leavesScale: new Vector2(0.05f, 0.1f),
        leavesRotationRange: new Vector3(30f, 180f, 60f), leavesRotationOffset: Vector3.zero
    )
    { }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        const float spacing = 5f;
        const float strength = 3f;
        Vector2Int jitterCount = new Vector2Int(10, 14);
        JitterPoissonSampling jps = new JitterPoissonSampling(chunkSize, chunkSize, spacing, strength, jitterCount, seed: seed);
        return jps.fill();
    }

    public override float MaxDim() { return 0.6f; }
    protected override int PreGenCount() { return 30; }
    public override bool IntersectionCheck() { return true; }
    public override void OnIntersect(float sqrSpeed)
    {
        if (!randomAudio.IsPlaying())
        {
            randomAudio.PlayRandomSound(sqrSpeed / 3f);
        }
    }
    private static List<float> RRS = new List<float> { 250f, 250f };
    public override List<float> RenderRadiusSquare() { return RRS; }
}
