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
        depth: new Vector2Int(12, 15),
        radius: new Vector2(0.02f, 0.02f),
        cylinderStep: 4,
        trunkSplitChance: 0.5f, splitChance: 0.2f,
        splitRotate: 90f, nonSplitRotate: 30f,
        trunkHeight: new Vector2(0.05f, 0.1f), branchLength: new Vector2(0.05f, 0.1f),
        leavesCount: 4, nonEndLeafChance: 0.6f,
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

    public override bool IntersectionCheck() { return true; }
    public override void OnIntersect(float sqrSpeed)
    {
        if (!randomAudio.IsPlaying())
        {
            randomAudio.PlayRandomSound(sqrSpeed / 3f);
        }
    }
}
