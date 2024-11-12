using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public class RockGeneration : PreGenerate<RockGeneration>
{
    public override bool ItemSpawnCheck() { return true; }
    public override int PreGenCount() { return 60; }
    public override bool RecalculateNormals() { return true; }

    public static TempMesh UNIT_CUBESPHERE;
    private static bool primitivesInit = false;
    private static void TryInit()
    {
        if (!primitivesInit)
        {
            UNIT_CUBESPHERE = CreateCubeSphere(1f, 10);
            primitivesInit = true;
        }
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        TryInit();
        TempMesh mesh = TransformMesh(VoronoiDisplace(UNIT_CUBESPHERE, rand, 0.1f, 1.5f), RandomTransform(rand));
        meshBuilder.AddMesh(mesh, 0);
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        const float spacing = 20f;
        const float strength = 3f;
        Vector2Int jitterCount = new Vector2Int(8, 12);
        JitterPoissonSampling jps = new JitterPoissonSampling(chunkSize, chunkSize, spacing, strength, jitterCount, seed: seed);
        return jps.fill();
    }

    public override bool FilterPoint(float globalX, float globalZ, int maskSeed)
    {
        const float size = 1f;
        const float threshold = 0.4f;
        return Mathf.PerlinNoise(globalX * size, globalZ * size + maskSeed / 1000) > threshold;
    }

    Matrix4x4 RandomTransform(System.Random random)
    {
        float scale = random.NextDouble() > 0.45f ? Utils.RandomRange(random, 0.4f, 1.2f) : Utils.RandomRange(random, 0.4f, 0.7f);
        maxDims.Add(scale);
        return Matrix4x4.Scale(new Vector3(
            scale,
            scale * Utils.RandomRange(random, 0.6f, 1.5f),
            scale * Utils.RandomRange(random, 0.4f, 1f)
        )) *
        Utils.RandomRotation(random, Vector3.up * 360f);
    }

    protected float renderRadiusSquare = 400f;
    public override float RenderRadiusSquare() { return renderRadiusSquare; }
}
