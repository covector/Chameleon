using System.Collections.Generic;
using UnityEngine;
using static Utils;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class ProceduralAsset : MonoBehaviour
{
    protected System.Random rand;
    protected int seed;
    protected float maxDim = 0f;
    private MeshRenderer meshRenderer;
    private float updater = float.PositiveInfinity;

    public virtual int MaterialCount() { return 1; }

    public virtual void Generate(int seed)
    {
        this.seed = seed;
        this.rand = new System.Random(seed);
        MeshBuilder meshBuilder = new MeshBuilder(MaterialCount());
        Edit(meshBuilder);
        GetComponent<MeshFilter>().mesh = meshBuilder.Build(RecalculateNormals());
        foreach (Material mat in GetComponent<MeshRenderer>().materials)
        {
            mat.SetFloat("_Seed", seed);
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (updater > 0.5f)
        {
            updater = 0f;
            float rrs = RenderRadiusSquare();
            if (rrs > 0)
            {
                GetComponent<MeshRenderer>().enabled = (ToVector2(Camera.main.transform.position) - ToVector2(transform.position)).sqrMagnitude < rrs;
            }
        }
        updater += Time.deltaTime;
    }

    protected abstract void Edit(MeshBuilder meshBuilder);
    public virtual float MaxDim() { return maxDim; }
    public virtual bool RecalculateNormals() { return false; }
    public abstract List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed);
    public virtual bool FilterPoint(float globalX, float globalZ, int maskSeed) { return true; }
    public virtual bool ItemSpawnCheck() { return false; }
    public virtual bool CollisionCheck() { return false; }
    public virtual bool IntersectionCheck() { return false; }
    public virtual void OnIntersect(float sqrSpeed) { }
    public virtual bool RotateToGround() { return false; }
    public virtual float SpawnYOffset() { return -0.1f; }
    public virtual float RenderRadiusSquare() { return -1f; }
}
