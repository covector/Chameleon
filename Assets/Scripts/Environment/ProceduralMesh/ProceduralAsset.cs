using System.Collections.Generic;
using UnityEngine;
using static Utils;

public abstract class ProceduralAsset : MonoBehaviour
{
    protected System.Random rand;
    protected float maxDim = 0f;

    protected List<Mesh> BuildMesh()
    {
        List<Mesh> list = new List<Mesh>();
        List<MeshBuilder> builders = new List<MeshBuilder>();
        foreach (int mc in MaterialCount())
        {
            builders.Add(new MeshBuilder(mc));
        }
        Edit(builders);
        foreach (MeshBuilder builder in builders)
        {
            list.Add(builder.Build(RecalculateNormals()));
        }
        return list;
    }

    private float updater = float.PositiveInfinity;
    private void Update()
    {
        if (updater > 0.5f)
        {
            updater = 0f;
            MeshRenderer[] renderers = Renderers();
            List<float> rrs = RenderRadiusSquare();
            bool cached = false;
            float dist = 0;
            for (int i = 0; i < rrs.Count; i++)
            {
                if (rrs[i] > 0)
                {
                    if (!cached)
                    {
                        dist = (ToVector2(Camera.main.transform.position) - ToVector2(transform.position)).sqrMagnitude;
                        cached = true;
                    }
                    renderers[i].enabled = dist < rrs[i];
                }
            }

        }
        updater += Time.deltaTime;
    }

    public abstract void Generate(int seed);

    public abstract AssetID ID();
    private static List<int> defaultMaterialCount = new List<int> { 1 };
    protected virtual List<int> MaterialCount() { return defaultMaterialCount; }
    protected abstract void Edit(List<MeshBuilder> meshBuilders);
    protected abstract MeshRenderer[] Renderers();
    public virtual float MaxDim() { return maxDim; }
    protected virtual bool RecalculateNormals() { return false; }
    public abstract List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed);
    public virtual bool FilterPoint(float globalX, float globalZ, int maskSeed) { return true; }
    public virtual bool ItemSpawnCheck() { return false; }
    public virtual bool CollisionCheck() { return false; }
    public virtual bool IntersectionCheck() { return false; }
    public virtual void OnIntersect(float sqrSpeed) { }
    public virtual bool RotateToGround() { return false; }
    public virtual float SpawnYOffset() { return -0.1f; }
    private static List<float> defaultRRS = new List<float> { -1f };
    public virtual List<float> RenderRadiusSquare() { return defaultRRS; }
    public abstract void ClearPreGen();
    public virtual int PreGenCount() { return 20; }
}
