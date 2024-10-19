using System.Collections.Generic;
using UnityEngine;

public abstract class PreGenerate<T> : ProceduralAsset where T : class
{
    protected static List<Mesh> s_preGenerated = new List<Mesh>();
    protected static List<float> s_maxDims = new List<float>();
    public List<Mesh> preGenerated { get => s_preGenerated; }
    public List<float> maxDims { get => s_maxDims; }

    protected void Init(int count)
    {
        if (s_preGenerated.Count == 0)
        {
            for (int i = 0; i < count; i++)
            {
                MeshBuilder meshBuilder = new MeshBuilder(MaterialCount(), Random.Range(0, 10000));
                Edit(meshBuilder);
                s_preGenerated.Add(meshBuilder.Build(RecalculateNormals()));
            }
            if (ItemSpawnCheck() && s_maxDims.Count != s_preGenerated.Count)
            {
                Debug.LogWarning("MaxDim is not equal to preGenCount!");
            }
        }
    }

    public override void Generate(int seed)
    {
        this.seed = seed;
        this.rand = new System.Random(seed);
        Init(PreGenCount());
        int index = this.rand.Next(preGenerated.Count);
        GetComponent<MeshFilter>().mesh = preGenerated[index];
        maxDim = maxDims[index];
    }

    public virtual int PreGenCount() { return 10; }
}