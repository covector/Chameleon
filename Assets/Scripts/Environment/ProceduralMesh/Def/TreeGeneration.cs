using UnityEngine;
using static MeshBuilder;

public class TreeGeneration : PreGenerate<TreeGeneration>
{
    private float baseRadius;

    public override int PreGenCount()
    {
        return 50;
    }

    public override float MaxDim()
    {
        return baseRadius;
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        Grow(meshBuilder, rand, Matrix4x4.identity, 10, new State(0, 0.05f + 0.35f * (float)rand.NextDouble(), 10));
    }

    Matrix4x4 RandomRotation(System.Random random, float range)
    {
        return Matrix4x4.Rotate(Quaternion.Euler(
            2f * range * ((float)random.NextDouble() - 0.5f),
            2f * range * ((float)random.NextDouble() - 0.5f),
            2f * range * ((float)random.NextDouble() - 0.5f)
        ));
    }

    struct State
    {
        public int split;
        public float radius;
        public int totalDepth;
        public State(int split, float radius, int totalDepth)
        {
            this.radius = radius;
            this.split = split;
            this.totalDepth = totalDepth;
        }
    }

    void Grow(MeshBuilder meshBuilder, System.Random random, Matrix4x4 cumTrans, int depth, State state)
    {
        if (depth == 0) { return; }
        float splitChance = state.split == 0 ? 0.1f : 0.4f;

        bool split = random.NextDouble() < splitChance;
        for (int i = 0; i < (split ? 2 : 1); i++)
        {
            Matrix4x4 newTrans = cumTrans * RandomRotation(random, split ? 30f: 10f);
            float height = state.split == 0 ? 1f + 5f * (float)random.NextDouble() : 1f;
            float radius = state.radius * depth / state.totalDepth;
            if (depth == state.totalDepth) { this.baseRadius = radius; }
            //TempMesh cylinder = TransformMesh(CreateCylinder(state.radius * depth / state.totalDepth, height, 8), newTrans);
            TempMesh cylinder = TransformMesh(UNIT_CYLINDER, newTrans * Matrix4x4.Scale(new Vector3(radius, height, radius)));
            meshBuilder.AddMesh(cylinder, 0);
            if (depth > 1)
            {
                newTrans = newTrans * Matrix4x4.Translate(new Vector3(0f, 0.9f * height, 0f));
                State newState = split ? state : new State(state.split + 1, state.radius, state.totalDepth); 
                Grow(meshBuilder, random, newTrans, depth - 1, newState);
            }
        }
    }
}
