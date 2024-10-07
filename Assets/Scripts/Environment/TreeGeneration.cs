using UnityEngine;
using static MeshBuilder;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TreeGeneration : MonoBehaviour
{
    public void Generate(int seed)
    {
        System.Random rand = new System.Random(seed);
        MeshBuilder meshBuilder = new MeshBuilder(1, seed);
        Grow(meshBuilder, rand, Matrix4x4.identity, 10, new State(0, 0.05f + 0.35f * (float)rand.NextDouble(), 10));
        GetComponent<MeshFilter>().mesh = meshBuilder.Build();
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
            TempMesh cylinder = TransformMesh(CreateCylinder(state.radius * depth / state.totalDepth, height, 8), newTrans);
            meshBuilder.AddMesh(cylinder, 0);
            //meshBuilder.AddCylinder(state.radius * depth / state.totalDepth, height, 8, 0, newTrans);
            if (depth > 1)
            {
                newTrans = newTrans * Matrix4x4.Translate(new Vector3(0f, 0.9f * height, 0f));
                State newState = split ? state : new State(state.split + 1, state.radius, state.totalDepth); 
                Grow(meshBuilder, random, newTrans, depth - 1, newState);
            }
        }
    }
}
