using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class ProceduralAsset : MonoBehaviour
{
    protected System.Random rand;
    protected int seed;

    public virtual int MaterialCount() { return 1; }

    public virtual void Generate(int seed)
    {
        this.seed = seed;
        this.rand = new System.Random(seed);
        MeshBuilder meshBuilder = new MeshBuilder(MaterialCount(), seed);
        Edit(meshBuilder);
        GetComponent<MeshFilter>().mesh = meshBuilder.Build();
    }

    protected abstract void Edit(MeshBuilder meshBuilder);
    public virtual float MaxDim() { return 0f; }
}
