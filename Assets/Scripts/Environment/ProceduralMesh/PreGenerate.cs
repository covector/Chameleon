using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PreGenerate<T> : ProceduralAsset where T : class
{
    protected static List<Mesh> s_preGenerated = new List<Mesh>();
    public List<Mesh> preGenerated { get => s_preGenerated; set => s_preGenerated = value; }

    protected void Init(int count)
    {
        if (s_preGenerated.Count == 0)
        {
            for (int i = 0; i < count; i++)
            {
                MeshBuilder meshBuilder = new MeshBuilder(MaterialCount(), Random.Range(0, 10000));
                Edit(meshBuilder);
                s_preGenerated.Add(meshBuilder.Build());
            }
        }
    }

    public override void Generate(int seed)
    {
        this.seed = seed;
        this.rand = new System.Random(seed);
        Init(PreGenCount());
        GetComponent<MeshFilter>().mesh = s_preGenerated[this.rand.Next(s_preGenerated.Count)];
    }

    public virtual int PreGenCount() { return 10; }
}