using System.Collections.Generic;
using UnityEngine;

public class MappingChunk : MonoBehaviour
{
    Vector2Int chunkInd;
    static float size = 10f;
    static int step = 25;
    int chunkSeed;
    static int terrainSeed;
    System.Random rand;
    List<GameObject> assets = new List<GameObject>();

    public static void Init(float size, int step, ChunkGeneration.Perlin[] perlins, int seed)
    {
        ChunkGeneration.Init(size, step, perlins, seed);
        terrainSeed = seed;
    }

    public void Create(Vector2Int chunkInd)
    {
        this.chunkInd = chunkInd;
        this.chunkSeed = terrainSeed + chunkInd.x * 727 - chunkInd.y * 757;  // arbitrary
        name = "Chunk_" + chunkInd.ToString();  // For debug
        GetComponent<MeshRenderer>().material.SetVector("_Seed", new Vector4(chunkInd.x, chunkInd.y));
    }

    public void GenerateGround()
    {
        GetComponent<MeshFilter>().mesh = ChunkGeneration.CreateGroundMesh(transform.position, size, step);
    }

    void PlaceAssets(AssetTemplate.MutableParam[] param)
    {
        System.Random rand = new System.Random(chunkSeed);
        float offset = size / 2f;
        for (int i = 0; i < param.Length; i++) 
        {
            AssetTemplate.MutableParam p = param[i];
            List<Vector2> points = p.SamplePoints(size, transform.position, rand.Next(10000));
            foreach (Vector2 point in points)
            {
                float globalX = point.x + transform.position.x - offset;
                float globalZ = point.y + transform.position.z - offset;
                if (!p.FilterPoint(globalX, globalZ, terrainSeed + 696 * i)) { continue; }
                GameObject asset = p.CreateObject();
                asset.transform.SetPositionAndRotation(new Vector3(globalX, ChunkGeneration.GetGroudLevel(globalX, globalZ, 1) - 0.1f, globalZ), Quaternion.identity);
                asset.transform.parent = transform;
                ProceduralAsset procedural = asset.GetComponent<ProceduralAsset>();
                asset.name = "Asset_" + assets.Count;  // For debug
                procedural.Generate(rand.Next(10000));
                assets.Add(asset);
            }
        }
    }

    public void UpdateAssets(AssetTemplate template)
    {
        foreach (GameObject asset in assets)
        {
            Destroy(asset);
        }
        assets = new List<GameObject>();

        PlaceAssets(template.GetParams());
    }
}
