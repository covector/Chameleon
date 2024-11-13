using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public abstract class GenericTreeGeneration<T> : PreGenerate<T> where T : class
{
    public MeshRenderer[] meshRenderers;
    protected int depth;
    protected Vector2 radius;
    protected int cylinderStep;
    protected float trunkSplitChance;
    protected float trunkRotate;
    protected int minTrunkDepth;
    protected float splitChance;
    protected float splitChanceFactor;
    protected float branchRotate;
    protected float branchRotateFactor;
    protected float branchRadiusFactor;
    protected float minBranchRadius;
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

    private static List<int> materialCount = new List<int> { 1, 2 };
    protected override List<int> MaterialCount() { return materialCount; }
    protected override MeshRenderer[] Renderers() { return meshRenderers; }

    public GenericTreeGeneration(
        int depth,
        Vector2 radius,
        int cylinderStep,
        float trunkSplitChance, int minTrunkDepth, float trunkRotate,
        float splitChance, float splitChanceFactor,
        float branchRotate, float branchRotateFactor,
        float branchRadiusFactor, float minBranchRadius, float branchLength, float branchLengthFactor,
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
        this.minTrunkDepth = minTrunkDepth;
        this.trunkRotate = trunkRotate;
        this.splitChance = splitChance;
        this.splitChanceFactor = splitChanceFactor;
        this.branchRotate = branchRotate;
        this.branchRotateFactor = branchRotateFactor;
        this.branchRadiusFactor = branchRadiusFactor;
        this.minBranchRadius = minBranchRadius;
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

    protected override void Edit(List<MeshBuilder> builders)
    {
        TryInit(cylinderStep);
        float randRadius = Utils.RandomRange(rand, radius);
        Grow(builders, rand, Matrix4x4.identity, depth, new State(0, randRadius, branchLength, splitChance, branchRotate, -1));
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, chunkSize / 2f, seed: seed);
        return fpds.fill();
    }

    struct State
    {
        public int split;
        public float radius;
        public float length;
        public float splitChance;
        public float branchRotate;
        public int lastVertInd;
        public State(int split, float radius, float length, float splitChance, float branchRotate, int lastVertInd)
        {
            this.radius = radius;
            this.length = length;
            this.split = split;
            this.splitChance = splitChance;
            this.branchRotate = branchRotate;
            this.lastVertInd = lastVertInd;
        }
    }

    void Grow(List<MeshBuilder> builders, System.Random random, Matrix4x4 cumTrans, int currentDepth, State state)
    {
        if (currentDepth == 0) { return; }
        bool split = false;
        if (depth - currentDepth >= minTrunkDepth) {
            float splitSample = state.split == 0 ? trunkSplitChance : state.splitChance;
            split = currentDepth < depth && random.NextDouble() < splitSample;
        }
        float height = split ? state.length : state.length * branchLengthFactor;
        float brotate = split ? state.branchRotate : trunkRotate;
        for (int i = 0; i < (split ? 2 : 1); i++)
        {
            Matrix4x4 newTrans = cumTrans * Utils.RandomRotation(random, new Vector3(brotate, 0, brotate));
            float branchRadius = i == 0 ? state.radius : state.radius * branchRadiusFactor;
            float radius = Mathf.Max(minBranchRadius, branchRadius * (currentDepth - 1) / (depth - 1));
            if (currentDepth == depth && i == 0) { maxDims.Add(radius); }
            TempMesh cylinder = TransformMesh(UNIT_CYLINDER, newTrans * Matrix4x4.Scale(new Vector3(radius, height, radius)));
            bool startLeaving = currentDepth <= startLeaveDepth;  // lol
            MeshBuilder builder = builders[startLeaving ? 1 : 0];
            builder.AddMesh(cylinder);
            int newLastVertInd = builder.GetLastVertInd();
            bool justStartedLeaving = currentDepth == startLeaveDepth;
            if (state.lastVertInd > 0 && i == 0)
            {
                builder.MergeCylinders(state.lastVertInd, newLastVertInd, cylinder.vertices.Count, justStartedLeaving ? builders[0] : builder);
            }
            if (currentDepth > 1)
            {
                newTrans = newTrans * Matrix4x4.Translate(new Vector3(0f, 0.9f * height, 0f));
                State newState = new State(
                    split ? state.split : state.split + 1,
                    branchRadius,
                    height,
                    state.split == 0 ? state.splitChance : state.splitChance * splitChanceFactor,
                    state.split == 0 ? state.branchRotate : state.branchRotate * branchRotateFactor,
                    newLastVertInd
                );
                Grow(builders, random, newTrans, currentDepth - 1, newState);
            }

            if (startLeaving)
            {
                for (int j = 0; j < leavesCount; j++)
                {
                    AddLeaf(builders[1], random, newTrans, height, currentDepth == 1 && j == 0);
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
