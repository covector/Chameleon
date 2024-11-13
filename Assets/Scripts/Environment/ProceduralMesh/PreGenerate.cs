using System.Collections.Generic;
using UnityEngine;

public abstract class PreGenerate<T> : ProceduralAsset where T : class
{
    protected static List<List<Mesh>> s_preGenerated = new List<List<Mesh>>();
    protected static List<float> s_maxDims = new List<float>();
    public List<float> maxDims { get => s_maxDims; }

    protected void InitPreGen(int count)
    {
        if (s_preGenerated.Count == 0)
        {
            for (int i = 0; i < count; i++)
            {
                s_preGenerated.Add(BuildMesh());
            }
            if (ItemSpawnCheck() && s_maxDims.Count != s_preGenerated.Count)
            {
                Debug.LogWarning("MaxDim is not equal to preGenCount!");
            }
        }
    }

    public void ReloadPreGen()
    {
        s_preGenerated = new List<List<Mesh>>();
        s_maxDims = new List<float>();
    }

    public override void Generate(int seed)
    {
        MeshRenderer[] renderers = Renderers();
        rand = new System.Random(seed);
        List<Mesh> list = new List<Mesh>();
        if (PreGenCount() == 0)
        {
            list = BuildMesh();
        }
        else
        {
            InitPreGen(PreGenCount());
            int index = rand.Next(s_preGenerated.Count);
            list = s_preGenerated[index];
            maxDim = maxDims[index];
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer mr = renderers[i];
            mr.GetComponent<MeshFilter>().mesh = list[i];
            foreach (Material mat in mr.materials)
            {
                mat.SetFloat("_Seed", seed);
            }
        }
    }

    protected virtual int PreGenCount() { return 20; }
}