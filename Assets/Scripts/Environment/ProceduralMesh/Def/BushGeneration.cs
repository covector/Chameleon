using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public class BushGeneration : PreGenerate<BushGeneration>
{
    public override int MaterialCount() { return 2; }
    public override int PreGenCount()
    {
        return 20;
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        int depth = 8 + rand.Next(4);
        Grow(meshBuilder, rand, Matrix4x4.identity, depth, new State(0, 0.03f, depth, -1));
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        JitterGridSampling jgs = new JitterGridSampling(chunkSize, chunkSize, chunkSize / 4f, chunkSize / 1.2f, globalPosition - new Vector3(offset, 0, offset), seed);
        return jgs.fill();
    }

    struct State
    {
        public int split;
        public float radius;
        public int totalDepth;
        public int lastVertInd;
        public State(int split, float radius, int totalDepth, int lastVertInd)
        {
            this.radius = radius;
            this.split = split;
            this.totalDepth = totalDepth;
            this.lastVertInd = lastVertInd;
        }
    }

    void Grow(MeshBuilder meshBuilder, System.Random random, Matrix4x4 cumTrans, int depth, State state)
    {
        if (depth == 0) { return; }
        float splitChance = state.split == 0 ? 0.1f : 0.4f;

        bool split = depth < state.totalDepth && random.NextDouble() < splitChance;
        for (int i = 0; i < (split ? 2 : 1); i++)
        {
            Matrix4x4 newTrans = cumTrans * Utils.RandomRotation(random, split ? 30f : 10f);
            float height = 0.15f + 0.1f * (float)random.NextDouble();
            float radius = state.radius * depth / state.totalDepth;
            if (depth == state.totalDepth && i == 0) { maxDims.Add(radius); }
            TempMesh cylinder = TransformMesh(UNIT_CYLINDER, newTrans * Matrix4x4.Scale(new Vector3(radius, height, radius)));
            meshBuilder.AddMesh(cylinder, 0);
            int newLastVertInd = meshBuilder.GetLastVertInd();
            if (state.lastVertInd > 0 && i == 0)
            {
                meshBuilder.MergeCylinders(state.lastVertInd, newLastVertInd, cylinder.vertices.Count);
            }
            if (depth > 1)
            {
                newTrans = newTrans * Matrix4x4.Translate(new Vector3(0f, 0.9f * height, 0f));
                State newState = new State(split ? state.split : state.split + 1, state.radius, state.totalDepth, newLastVertInd);
                Grow(meshBuilder, random, newTrans, depth - 1, newState);
            }
            else
            {
                for (int j = 0; j < 5; j++)
                {
                    TempMesh leaf = TransformMesh(
                        CreateQuad(new Vector3(0.5f, 0f, 2f), Vector3.left, Vector3.back * 2f),
                        newTrans * Matrix4x4.Translate(new Vector3(0f, Utils.RandomRange(random, height), 0f)) *
                        Utils.RandomRotation(random, Vector3.up * 360f) *
                        Matrix4x4.Scale(Vector3.one * 0.1f)
                    );
                    meshBuilder.AddMesh(leaf, 1);
                }
            }
        }
    }
}
