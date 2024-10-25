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
        radius: new Vector2(0.03f, 0.03f),
        cylinderStep: 4,
        trunkSplitChance: 0.1f, splitChance: 0.4f,
        splitRotate: 30f, nonSplitRotate: 10f,
        trunkHeight: new Vector2(0.15f, 0.25f), branchLength: new Vector2(0.15f, 0.25f),
        leavesCount: 5, nonEndLeafChance: 0.1f,
        crossRenderLeaves: false,
        leavesDim: new Vector2(1f, 2f), leavesScale: new Vector2(0.08f, 0.12f),
        leavesRotationRange: new Vector3(30f, 180f, 30f), leavesRotationOffset: Vector3.zero
    )
    { }

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

    public override float MaxDim() { return 1.0f; }

    public override bool IntersectionCheck() { return true; }
    public override void OnIntersect(float sqrSpeed)
    {
        if (!randomAudio.IsPlaying())
        {
            randomAudio.PlayRandomSound(sqrSpeed / 3f);
        }
    }
}
