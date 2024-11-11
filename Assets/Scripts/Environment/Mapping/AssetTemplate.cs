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
        public Vector2Int depth;
        public Vector2 radius;
        public float trunkSplitChance;
        public float splitChance;
        public float splitRotate;
        public float nonSplitRotate;
        public Vector2 trunkHeight;
        public Vector2 branchLength;
        public int leavesCount;
        public bool crossRenderLeaves;
        public Vector2 leavesDim;
        public Vector2 leavesScale;
        public Vector3 leavesRotationOffset;
        public Vector3 leavesRotationRange;
        public float nonEndLeafChance;
        public int cylinderStep;
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
            this.nonSplitRotate = treeParam.nonSplitRotate;
            this.trunkHeight = treeParam.trunkHeight;
            this.branchLength = treeParam.branchLength;
            this.leavesCount = treeParam.leavesCount;
            this.crossRenderLeaves = treeParam.crossRenderLeaves;
            this.leavesDim = treeParam.leavesDim;
            this.leavesScale = treeParam.leavesScale;
            this.leavesRotationOffset = treeParam.leavesRotationOffset;
            this.leavesRotationRange = treeParam.leavesRotationRange;
            this.nonEndLeafChance = treeParam.nonEndLeafChance;
            this.cylinderStep = treeParam.cylinderStep;
        }

        public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
        {
            return new List<Vector2>();
        }

        public override int PreGenCount() { return 0; }
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
    [Header("Small Trees")]
    [SerializeField]
    public MutableTreeParam smallTreeParam;

    public MutableParam[] GetParams()
    {
        MutableParam[] param = new MutableParam[4];

        param[0] = treeParam;
        param[1] = rockParam;
        param[2] = bushParam;
        param[3] = smallTreeParam;

        return param;
    }
}
