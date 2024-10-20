using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class ProceduralAsset : MonoBehaviour
{
    protected System.Random rand;
    protected int seed;
    protected float maxDim = 0f;

    public virtual int MaterialCount() { return 1; }

    public virtual void Generate(int seed)
    {
        this.seed = seed;
        this.rand = new System.Random(seed);
        MeshBuilder meshBuilder = new MeshBuilder(MaterialCount(), seed);
        Edit(meshBuilder);
        GetComponent<MeshFilter>().mesh = meshBuilder.Build(RecalculateNormals());
        foreach (Material mat in GetComponent<MeshRenderer>().materials)
        {
            mat.SetFloat("_Seed", seed);
        }
    }

    protected abstract void Edit(MeshBuilder meshBuilder);
    public float MaxDim() { return maxDim; }
    public virtual bool RecalculateNormals() { return false; }
    public abstract List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed);
    public virtual bool FilterPoint(float globalX, float globalZ, int maskSeed) { return true; }
    public virtual bool ItemSpawnCheck() { return false; }
    public virtual bool CollisionCheck() { return false; }
}
