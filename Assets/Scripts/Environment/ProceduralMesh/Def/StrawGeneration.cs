using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;
using static Utils;

public class StrawGeneration : PreGenerate<StrawGeneration>
{
    protected Vector2 segmentLength = new Vector2(0.2f, 0.3f);
    protected Vector2 width = new Vector2(0.01f, 0.02f);
    protected int segments = 10;
    protected int duplicate = 10;
    protected float duplicateSpread = 2.0f;
    protected float yFactor = 0.4f;

    protected override void Edit(MeshBuilder meshBuilder)
    {
        maxDims.Add(0);
        for (int i = 0; i < duplicate; i++)
        {
            Grow(meshBuilder, rand,
                //new Vector3(RandomRange(rand, -duplicateSpread, duplicateSpread), 0f, RandomRange(rand, -duplicateSpread, duplicateSpread)),
                new Vector3(TriangleDistr(rand, duplicateSpread), 0f, TriangleDistr(rand, duplicateSpread)),
                segments, width, segmentLength);
        }
    }

    private Vector3 RandomVector(System.Random random, float length)
    {
        return new Vector3(RandomRange(random, -length, length), RandomRange(random, -length * yFactor, length * yFactor), RandomRange(random, -length, length));
    }

    private void AddDoubleSideQuad(MeshBuilder meshBuilder, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        meshBuilder.AddMesh(CreateQuad(v1, v4, v3, v2));
        meshBuilder.AddMesh(CreateQuad(v1, v2, v3, v4));
    }

    void Grow(MeshBuilder meshBuilder, System.Random random, Vector3 lastPos, int segmentsLeft, Vector2 width, Vector2 length)
    {
        if (segmentsLeft == 0) { return; }
        float randWidth = RandomRange(random, width);
        float randLength = RandomRange(random, segmentLength);
        Vector3 widthVec = RandomVector(random, randWidth);
        Vector3 lengthVec = RandomVector(random, randLength);
        Vector3 pos1 = lastPos + lengthVec;
        Vector3 pos2 = pos1 + widthVec;
        int vert2 = meshBuilder.GetLastVertInd();
        if (segments == segmentsLeft)
        {
            AddDoubleSideQuad(meshBuilder, lastPos, lastPos + RandomVector(random, randWidth), pos2, pos1);
        } else
        {
            AddDoubleSideQuad(meshBuilder, meshBuilder.Vertices[vert2], meshBuilder.Vertices[vert2 - 1], pos2, pos1);
        }
        Grow(meshBuilder, random, pos1, segmentsLeft - 1, width, length);
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        const float spacing = 4f;
        const float strength = 4f;
        JitterGridSampling jgs = new JitterGridSampling(chunkSize, chunkSize, spacing, strength, globalPosition - new Vector3(offset, 0, offset), seed);
        return jgs.fill();
    }

    public override int MaterialCount() { return 1; }
    public override bool RotateToGround() { return true; }
    public override float SpawnYOffset() { return 0; }
}
