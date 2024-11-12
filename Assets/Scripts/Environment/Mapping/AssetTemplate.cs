using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetTemplate", menuName = "Scriptable Objects/AssetTemplate")]
public class AssetTemplate : ScriptableObject
{
    #region static definitions
    public interface MutableParam {
        public GameObject CreateObject();
        public List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed);
        public bool FilterPoint(float globalX, float globalZ, int maskSeed);
    }
    public interface MutableAsset
    {
        public void EditParam(MutableParam param);
    }

    [System.Serializable]
    public enum SamplingAlgo
    {
        POISSON_DISC, JITTER_GRID, JITTER_POISSON
    }
    [System.Serializable]
    public struct SamplingParam
    {
        public SamplingAlgo algorithm;
        public float spacing;
        public float strength;
        public Vector2Int jitterCount;
        public bool perlinFiltering;
        public float filterSize;
        public float filterThreshold;

        public List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
        {
            switch (algorithm)
            {
                case SamplingAlgo.POISSON_DISC:
                    FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(chunkSize, chunkSize, spacing, seed: seed);
                    return fpds.fill();
                case SamplingAlgo.JITTER_GRID:
                    float offset = chunkSize / 2f;
                    JitterGridSampling jgs = new JitterGridSampling(chunkSize, chunkSize, spacing, strength, globalPosition - new Vector3(offset, 0, offset), seed);
                    return jgs.fill();
                case SamplingAlgo.JITTER_POISSON:
                    JitterPoissonSampling jps = new JitterPoissonSampling(chunkSize, chunkSize, spacing, strength, jitterCount, seed: seed);
                    return jps.fill();
            }
            return new List<Vector2>();
        }

        public bool FilterPoint(float globalX, float globalZ, int maskSeed)
        {
            if (!perlinFiltering) { return true; }
            return Mathf.PerlinNoise(globalX * filterSize, globalZ * filterSize + maskSeed / 1000) > filterThreshold;
        }
    }

    [System.Serializable]
    public struct MutableTreeParam : MutableParam
    {
        public bool enabled;
        public int depth;
        public Vector2 radius;
        public int cylinderStep;
        public float trunkSplitChance;
        public float splitChance;
        public float splitRotate;
        public float splitRadiusFactor;
        public float nonSplitRotate;
        public float branchLength;
        public float branchLengthFactor;
        public int leavesCount;
        public int startLeaveDepth;
        public bool crossRenderLeaves;
        public Vector2 leavesDim;
        public Vector2 leavesScale;
        public Vector3 leavesRotationRange;
        public Vector3 leavesRotationOffset;
        public Material[] materials;
        public SamplingParam sampling;

        public GameObject CreateObject()
        {
            GameObject go = new GameObject();
            go.AddComponent<MeshRenderer>().materials = materials;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MutableTree>().EditParam(this);
            return go;
        }

        public List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
        {
            if (!enabled) { return  new List<Vector2>(); }
            return sampling.SamplePoints(chunkSize, globalPosition, seed);
        }

        public bool FilterPoint(float globalX, float globalZ, int maskSeed)
        {
            return sampling.FilterPoint(globalX, globalZ, maskSeed);
        }
    }

    public class MutableTree: GenericTreeGeneration<MutableTree>, MutableAsset
    {      
        public MutableTree() : base() {}

        public void EditParam(MutableParam param)
        {
            MutableTreeParam treeParam = (MutableTreeParam)param;
            this.depth = treeParam.depth;
            this.radius = treeParam.radius;
            this.trunkSplitChance = treeParam.trunkSplitChance;
            this.splitChance = treeParam.splitChance;
            this.splitRotate = treeParam.splitRotate;
            this.splitRadiusFactor = treeParam.splitRadiusFactor;
            this.nonSplitRotate = treeParam.nonSplitRotate;
            this.branchLength = treeParam.branchLength;
            this.branchLengthFactor = treeParam.branchLengthFactor;
            this.leavesCount = treeParam.leavesCount;
            this.crossRenderLeaves = treeParam.crossRenderLeaves;
            this.leavesDim = treeParam.leavesDim;
            this.leavesScale = treeParam.leavesScale;
            this.leavesRotationOffset = treeParam.leavesRotationOffset;
            this.leavesRotationRange = treeParam.leavesRotationRange;
            this.startLeaveDepth = treeParam.startLeaveDepth;
            this.cylinderStep = treeParam.cylinderStep;
        }

        public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
        {
            return new List<Vector2>();
        }

        public override int PreGenCount() { return 0; }
        public override float RenderRadiusSquare() { return -1; }
    }

    [System.Serializable]
    public struct MutableRockParam : MutableParam
    {
        public bool enabled;
        public Material[] materials;
        public SamplingParam sampling;

        public GameObject CreateObject()
        {
            GameObject go = new GameObject();
            go.AddComponent<MeshRenderer>().materials = materials;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MutableRock>().EditParam(this);
            return go;
        }

        public List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
        {
            if (!enabled) { return new List<Vector2>(); }
            return sampling.SamplePoints(chunkSize, globalPosition, seed);
        }

        public bool FilterPoint(float globalX, float globalZ, int maskSeed)
        {
            return sampling.FilterPoint(globalX, globalZ, maskSeed);
        }
    }

    public class MutableRock : RockGeneration, MutableAsset
    {
        public MutableRock() : base() { }

        public void EditParam(MutableParam param)
        {
            MutableRockParam rockParam = (MutableRockParam)param;
        }

        public override int PreGenCount() { return 0; }
        public override float RenderRadiusSquare() { return -1; }
    }

    [System.Serializable]
    public struct MutableStrawParam : MutableParam
    {
        public bool enabled;
        public Vector2 segmentLength;
        public Vector2 width;
        public int segments;
        public int duplicate;
        public float duplicateSpread;
        public float yFactor;
        public Material[] materials;
        public SamplingParam sampling;

        public GameObject CreateObject()
        {
            GameObject go = new GameObject();
            go.AddComponent<MeshRenderer>().materials = materials;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MutableStraw>().EditParam(this);
            return go;
        }

        public List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
        {
            if (!enabled) { return new List<Vector2>(); }
            return sampling.SamplePoints(chunkSize, globalPosition, seed);
        }

        public bool FilterPoint(float globalX, float globalZ, int maskSeed)
        {
            return sampling.FilterPoint(globalX, globalZ, maskSeed);
        }
    }

    public class MutableStraw : StrawGeneration, MutableAsset
    {
        public MutableStraw() : base() { }

        public void EditParam(MutableParam param)
        {
            MutableStrawParam strawParam = (MutableStrawParam)param;
            this.segmentLength = strawParam.segmentLength;
            this.segments = strawParam.segments;
            this.width = strawParam.width;
            this.yFactor = strawParam.yFactor;
            this.duplicate = strawParam.duplicate;
            this.duplicateSpread = strawParam.duplicateSpread;
        }

        public override int PreGenCount() { return 0; }
        public override float RenderRadiusSquare() { return -1; }
    }
    #endregion

    [Header("Terrain")]
    public int terrain_step = 25;
    public float terrain_chunkSize = 10f;
    public ChunkGeneration.Perlin[] terrain_perlins;
    public Material groundMaterial;
    [Header("Trees")]
    [SerializeField]
    public MutableTreeParam treeParam;
    [Header("Rocks")]
    [SerializeField]
    public MutableRockParam rockParam;
    [Header("Bushes")]
    [SerializeField]
    public MutableTreeParam bushParam;
    [Header("New Bushes")]
    [SerializeField]
    public MutableTreeParam newBushParam;
    [Header("Small Trees")]
    [SerializeField]
    public MutableTreeParam smallTreeParam;
    [Header("Straw")]
    [SerializeField]
    public MutableStrawParam straw;

    public MutableParam[] GetParams()
    {
        MutableParam[] param = new MutableParam[6];

        param[0] = treeParam;
        param[1] = rockParam;
        param[2] = bushParam;
        param[3] = newBushParam;
        param[4] = smallTreeParam;
        param[5] = straw;

        return param;
    }
}
