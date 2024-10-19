using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public abstract class GenericTreeGeneration<T> : PreGenerate<T> where T : class
{
    protected int preGenCount;
    protected Vector2Int depth;
    protected Vector2 radius;
    protected float trunkSplitChance;
    protected float splitChance;
    protected float splitRotate;
    protected float nonSplitRotate;
    protected Vector2 trunkHeight;
    protected Vector2 branchLength;
    protected int leavesCount;
    protected Vector2 leavesDim;
    protected Vector3 leavesRotation;

    public GenericTreeGeneration(
        Vector2Int depth,
        Vector2 radius,
        float trunkSplitChance, float splitChance,
        float splitRotate, float nonSplitRotate,
        Vector2 trunkHeight, Vector2 branchLength,
        int leavesCount, Vector2 leavesDim, Vector2 leavesRotation
        )
    {
        this.depth = depth;
        this.radius = radius;
        this.trunkSplitChance = trunkSplitChance;
        this.splitChance = splitChance;
        this.splitRotate = splitRotate;
        this.nonSplitRotate = nonSplitRotate;
        this.trunkHeight = trunkHeight;
        this.branchLength = branchLength;
        this.leavesCount = leavesCount;
        this.leavesDim = leavesDim;
        this.leavesRotation = leavesRotation;
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        int randDepth = Utils.RandomRange(rand, depth);
        float randRadius = Utils.RandomRange(rand, radius);
        Grow(meshBuilder, rand, Matrix4x4.identity, randDepth, new State(0, randRadius, randDepth, -1));
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, chunkSize / 2f, seed: seed);
        return fpds.fill();
    }

    public override int MaterialCount() { return 2; }

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
        float splitSample = state.split == 0 ? trunkSplitChance : splitChance;
        bool split = depth < state.totalDepth && random.NextDouble() < splitSample;
        for (int i = 0; i < (split ? 2 : 1); i++)
        {
            Matrix4x4 newTrans = cumTrans * Utils.RandomRotation(random, split ? splitRotate : nonSplitRotate);
            float height = state.split == 0 ? Utils.RandomRange(rand, trunkHeight) : Utils.RandomRange(rand, branchLength);
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

                for (int j = 0; j < leavesCount; j++)
                {
                    Matrix4x4 trans = newTrans *
                        Matrix4x4.Translate(new Vector3(0f, j == 0 ? height : Utils.RandomRange(random, height), 0f)) *
                        Utils.RandomRotation(random, leavesRotation)
                    ;
                    TempMesh leafFront = TransformMesh(
                        CreateQuad(new Vector3(0.5f * leavesDim.x, 0f, leavesDim.y), Vector3.left * leavesDim.x, Vector3.back * leavesDim.y),
                        trans
                    );
                    meshBuilder.AddMesh(leafFront, 1);
                    TempMesh leafBack = TransformMesh(
                        CreateQuad(new Vector3(-0.5f * leavesDim.x, 0f, leavesDim.y), Vector3.right * leavesDim.x, Vector3.back * leavesDim.y),
                        trans
                    );
                    meshBuilder.AddMesh(leafBack, 1);
                }
            }
        }
    }
}
