using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public class TreeGeneration : PreGenerate<TreeGeneration>
{
    public override bool ItemSpawnCheck() { return true; }

    public override int PreGenCount()
    {
        return 50;
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        Grow(meshBuilder, rand, Matrix4x4.identity, 10, new State(0, 0.05f + 0.35f * (float)rand.NextDouble(), 10, -1));
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, chunkSize / 2f, seed);
        return fpds.fill();
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
            Matrix4x4 newTrans = cumTrans * Utils.RandomRotation(random, split ? 30f: 10f);
            float height = state.split == 0 ? 1f + 5f * (float)random.NextDouble() : 1f;
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
                State newState =  new State(split ? state.split : state.split + 1, state.radius, state.totalDepth, newLastVertInd); 
                Grow(meshBuilder, random, newTrans, depth - 1, newState);
            }
        }
    }
}
