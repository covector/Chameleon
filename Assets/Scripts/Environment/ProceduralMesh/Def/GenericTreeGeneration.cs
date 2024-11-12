using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public abstract class GenericTreeGeneration<T> : PreGenerate<T> where T : class
{
    protected int depth;
    protected Vector2 radius;
    protected int cylinderStep;
    protected float trunkSplitChance;
    protected float splitChance;
    protected float splitRotate;
    protected float splitRadiusFactor;
    protected float nonSplitRotate;
    protected float trunkHeight;
    protected float branchLength;
    protected float branchLengthFactor;
    protected int leavesCount;
    protected int startLeaveDepth;
    protected bool crossRenderLeaves;
    protected Vector2 leavesDim;
    protected Vector2 leavesScale;
    protected Vector3 leavesRotationRange;
    protected Vector3 leavesRotationOffset;

    public static TempMesh UNIT_CYLINDER;
    protected static bool primitivesInit = false;
    private static void TryInit(int cylinderStep)
    {
        if (!primitivesInit)
        {
            UNIT_CYLINDER = CreateCylinder(1f, 1f, cylinderStep);
            primitivesInit = true;
        }
    }

    public GenericTreeGeneration(
        int depth,
        Vector2 radius,
        int cylinderStep,
        float trunkSplitChance, float splitChance,
        float splitRotate, float splitRadiusFactor, float nonSplitRotate,
        float branchLength, float branchLengthFactor,
        int leavesCount, int startLeaveDepth,
        bool crossRenderLeaves,
        Vector2 leavesDim, Vector2 leavesScale,
        Vector3 leavesRotationRange, Vector3 leavesRotationOffset
        )
    {
        this.depth = depth;
        this.radius = radius;
        this.cylinderStep = cylinderStep;
        this.trunkSplitChance = trunkSplitChance;
        this.splitChance = splitChance;
        this.splitRotate = splitRotate;
        this.splitRadiusFactor = splitRadiusFactor;
        this.nonSplitRotate = nonSplitRotate;
        this.branchLength = branchLength;
        this.branchLengthFactor = branchLengthFactor;
        this.leavesCount = leavesCount;
        this.startLeaveDepth = startLeaveDepth;
        this.crossRenderLeaves = crossRenderLeaves;
        this.leavesDim = leavesDim;
        this.leavesScale = leavesScale;
        this.leavesRotationOffset = leavesRotationOffset;
        this.leavesRotationRange = leavesRotationRange;
    }

    public GenericTreeGeneration() { }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        TryInit(cylinderStep);
        float randRadius = Utils.RandomRange(rand, radius);
        Grow(meshBuilder, rand, Matrix4x4.identity, depth, new State(0, randRadius, branchLength, -1));
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
        public float length;
        public int lastVertInd;
        public State(int split, float radius, float length, int lastVertInd)
        {
            this.radius = radius;
            this.length = length;
            this.split = split;
            this.lastVertInd = lastVertInd;
        }
    }

    void Grow(MeshBuilder meshBuilder, System.Random random, Matrix4x4 cumTrans, int currentDepth, State state)
    {
        if (currentDepth == 0) { return; }
        float splitSample = state.split == 0 ? trunkSplitChance : splitChance;
        bool split = currentDepth < depth && random.NextDouble() < splitSample;
        float height = split ? state.length : state.length * branchLengthFactor;
        for (int i = 0; i < (split ? 2 : 1); i++)
        {
            Matrix4x4 newTrans = cumTrans * Utils.RandomRotation(random, split ? splitRotate : nonSplitRotate);
            float branchRadius = i == 0 ? state.radius : state.radius * splitRadiusFactor;
            float radius = Mathf.Max(0.003f, branchRadius * (currentDepth - 1) / (depth - 1));
            if (currentDepth == depth && i == 0) { maxDims.Add(radius); }
            TempMesh cylinder = TransformMesh(UNIT_CYLINDER, newTrans * Matrix4x4.Scale(new Vector3(radius, height, radius)));
            meshBuilder.AddMesh(cylinder, 0);
            int newLastVertInd = meshBuilder.GetLastVertInd();
            if (state.lastVertInd > 0 && i == 0)
            {
                meshBuilder.MergeCylinders(state.lastVertInd, newLastVertInd, cylinder.vertices.Count);
            }
            if (currentDepth > 1)
            {
                newTrans = newTrans * Matrix4x4.Translate(new Vector3(0f, 0.9f * height, 0f));
                State newState = new State(split ? state.split : state.split + 1, branchRadius, height, newLastVertInd);
                Grow(meshBuilder, random, newTrans, currentDepth - 1, newState);
            }

            if (currentDepth <= startLeaveDepth)
            {
                for (int j = 0; j < leavesCount; j++)
                {
                    AddLeaf(meshBuilder, random, newTrans, height, currentDepth == 1 && j == 0);
                }
            }
        }
    }

    void AddLeaf(MeshBuilder meshBuilder, System.Random random, Matrix4x4 cumTrans, float height, bool forceAtEnd)
    {
        float scale = Utils.RandomRange(random, leavesScale);
        Matrix4x4 trans = cumTrans *
                        Matrix4x4.Translate(new Vector3(0f, forceAtEnd ? height : Utils.RandomRange(random, height), 0f)) *
                        Utils.RandomRotation(random, leavesRotationRange, leavesRotationOffset) *
                        Matrix4x4.Scale(Vector3.one * scale);
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
        if (crossRenderLeaves)
        {
            TempMesh leafLeft = TransformMesh(
            CreateQuad(new Vector3(0f, 0.5f * leavesDim.x, leavesDim.y), Vector3.down * leavesDim.x, Vector3.back * leavesDim.y),
            trans
        );
            meshBuilder.AddMesh(leafLeft, 1);
            TempMesh leafRight = TransformMesh(
                CreateQuad(new Vector3(0f, -0.5f * leavesDim.x, leavesDim.y), Vector3.up * leavesDim.x, Vector3.back * leavesDim.y),
                trans
            );
            meshBuilder.AddMesh(leafRight, 1);
        }
    }
}
