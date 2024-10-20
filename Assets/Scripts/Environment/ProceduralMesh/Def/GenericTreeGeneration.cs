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
    protected bool crossRenderLeaves;
    protected Vector2 leavesDim;
    protected Vector2 leavesScale;
    protected Vector3 leavesRotationOffset;
    protected Vector3 leavesRotationRange;
    protected float nonEndLeafChance;
    protected int cylinderStep;

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
        Vector2Int depth,
        Vector2 radius,
        int cylinderStep,
        float trunkSplitChance, float splitChance,
        float splitRotate, float nonSplitRotate,
        Vector2 trunkHeight, Vector2 branchLength,
        int leavesCount, float nonEndLeafChance,
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
        this.nonSplitRotate = nonSplitRotate;
        this.trunkHeight = trunkHeight;
        this.branchLength = branchLength;
        this.leavesCount = leavesCount;
        this.nonEndLeafChance = nonEndLeafChance;
        this.crossRenderLeaves = crossRenderLeaves;
        this.leavesDim = leavesDim;
        this.leavesScale = leavesScale;
        this.leavesRotationOffset = leavesRotationOffset;
        this.leavesRotationRange = leavesRotationRange;
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        TryInit(cylinderStep);
        int randDepth = Utils.RandomRange(rand, depth);
        float randRadius = Utils.RandomRange(rand, radius);
        Grow(meshBuilder, rand, Matrix4x4.identity, randDepth, new State(0, randRadius, randDepth, -1, 0f));
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
        public float uvOffset;
        public State(int split, float radius, int totalDepth, int lastVertInd, float uvOffset)
        {
            this.radius = radius;
            this.split = split;
            this.totalDepth = totalDepth;
            this.lastVertInd = lastVertInd;
            this.uvOffset = uvOffset;
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
            for (int u = 0; u < cylinder.uvs.Count; u++)
            {
                cylinder.uvs[u] = new Vector2(cylinder.uvs[u].x, cylinder.uvs[u].y + state.uvOffset);
            }
            meshBuilder.AddMesh(cylinder, 0);
            int newLastVertInd = meshBuilder.GetLastVertInd();
            if (state.lastVertInd > 0 && i == 0)
            {
                meshBuilder.MergeCylinders(state.lastVertInd, newLastVertInd, cylinder.vertices.Count);
            }
            if (depth > 1)
            {
                newTrans = newTrans * Matrix4x4.Translate(new Vector3(0f, 0.9f * height, 0f));
                State newState = new State(split ? state.split : state.split + 1, state.radius, state.totalDepth, newLastVertInd, i == 0 ? state.uvOffset + 1 : 0);
                Grow(meshBuilder, random, newTrans, depth - 1, newState);
            }

            for (int j = 0; j < leavesCount; j++)
            {
                if (depth > 1 && (float)random.NextDouble() > nonEndLeafChance) { continue; }
                AddLeaf(meshBuilder, random, newTrans, height, depth == 1 && j == 0);
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
