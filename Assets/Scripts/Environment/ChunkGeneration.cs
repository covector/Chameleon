using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkGeneration : MonoBehaviour
{
    Vector2Int chunkInd;
    float size = 10f;
    int step = 25;
    Perlin[] perlins;
    int terrainSeed;
    int chunkSeed;
    System.Random rand;
    public GameObject[] assetPrefabs;
    List<GameObject> assets = new List<GameObject>();
    public ItemSpawning itemSpawning;

    [System.Serializable]
    public struct Perlin
    {
        public float frequency;
        public float amplitude;
    }

    public void Init(Vector2Int chunkInd, float size, int step, Perlin[] perlins, int seed)
    {
        this.chunkInd = chunkInd;
        this.size = size;
        this.step = step;
        this.perlins = perlins;
        this.terrainSeed = seed;
        this.chunkSeed = seed + chunkInd.x * 727 - chunkInd.y * 757;  // arbitrary
        this.rand = new System.Random(chunkSeed);
        GenerateChunk();
    }

    void GenerateChunk()
    {
        GetComponent<MeshFilter>().mesh = CreateGroundMesh(size, step);

        PlaceTrees();
        PlaceRocks();
    }

    Mesh CreateGroundMesh(float size, int step)
    {
        Mesh mesh = new Mesh { name = "Procedural Ground" };
        int gridPt = step + 2;
        int gridSqr = step + 1;

        Vector3[] vertices = new Vector3[gridPt * gridPt];
        for (int i = 0; i < gridPt; i++)
        {
            for (int j = 0; j < gridPt; j++)
            {
                float x = size * i / gridSqr - size / 2f;
                float z = size * j / gridSqr - size / 2f;
                float globalX = x + transform.position.x;
                float globalZ = z + transform.position.z;
                float y = GetGroudLevel(globalX, globalZ, perlins, terrainSeed);
                vertices[i * gridPt + j] = new Vector3(x, y, z);
            }
        }
        mesh.vertices = vertices;

        int[] triangles = new int[gridSqr * gridSqr * 6];
        for (int i = 0; i < gridSqr; i++)
        {
            for (int j = 0; j < gridSqr; j++)
            {
                int triInd = (i * gridSqr + j) * 6;
                triangles[triInd] = i * gridPt + j;
                triangles[triInd + 1] = (i + 1) * gridPt + (j + 1);
                triangles[triInd + 2] = (i + 1) * gridPt + j;
                triangles[triInd + 3] = i * gridPt + j;
                triangles[triInd + 4] = i * gridPt + (j + 1);
                triangles[triInd + 5] = (i + 1) * gridPt + (j + 1);
            }
        }
        mesh.triangles = triangles;

        Vector3[] normals = new Vector3[gridPt * gridPt];
        for (int i = 0; i < gridPt; i++)
        {
            for (int j = 0; j < gridPt; j++)
            {
                normals[i * gridPt + j] = Vector3.up;
            }
        }
        mesh.normals = normals;

        Vector2[] uv = new Vector2[gridPt * gridPt];
        for (int i = 0; i < gridPt; i++)
        {
            for (int j = 0; j < gridPt; j++)
            {
                uv[i * gridPt + j] = new Vector2((float)i / gridSqr - 0.5f, (float)j / gridSqr - 0.5f);
            }
        }
        mesh.uv = uv;

        mesh.RecalculateBounds();

        return mesh;
    }

    public static float GetGroudLevel(float x, float z, Perlin[] perlins, int seed, int levels = 1)
    {
        float y = 0;
        for (int i = 0; i < perlins.Length; i++)
        {
            y += perlins[i].amplitude * Mathf.PerlinNoise(x * perlins[i].frequency + 1000f + seed, z * perlins[i].frequency + 1000f + seed);
            if (i >= levels - 1) { break; }
        }
        return y;
    }

    public bool CheckSpawnVicinity(Vector2 pos, float squareRadius)
    {
        foreach (GameObject g in assets)
        {
            if ((Utils.ToVector2(g.transform.position) - pos).sqrMagnitude < squareRadius) {
                return true;
            }
        }
        return false;
    }

    void PlaceTrees()
    {
        FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(this.size, this.size, this.size / 2f, seed: rand.Next(10000));
        float offset = this.size / 2f;
        List<Vector2> points = fpds.fill();
        foreach (Vector2 point in points)
        {
            float globalX = point.x + transform.position.x - offset;
            float globalZ = point.y + transform.position.z - offset;
            if (itemSpawning.CheckSpawnVicinity(new Vector2(globalX, globalZ), 2f)) { continue; }
            GameObject tree = Instantiate(assetPrefabs[0], new Vector3(globalX, GetGroudLevel(globalX, globalZ, perlins, terrainSeed, 1) - 0.1f, globalZ), Quaternion.identity, transform);
            assets.Add(tree);
            tree.GetComponent<TreeGeneration>().Generate(rand.Next(10000));
        }
    }

    void PlaceRocks()
    {
        float offset = this.size / 2f;
        JitterGridSampling fpds = new JitterGridSampling(this.size, this.size, this.size / 4f, this.size / 1.2f, transform.position - new Vector3(offset, 0, offset), seed: rand.Next(10000));
        //FastPoissonDiskSampling fpds = new FastPoissonDiskSampling(this.size, this.size, this.size / 4f, seed: rand.Next(10000));

        List<Vector2> points = fpds.fill();
        foreach (Vector2 point in points)
        {
            float globalX = point.x + transform.position.x - offset;
            float globalZ = point.y + transform.position.z - offset;
            if (itemSpawning.CheckSpawnVicinity(new Vector2(globalX, globalZ), 2f)) { continue; }
            GameObject rock = Instantiate(assetPrefabs[1], new Vector3(globalX, GetGroudLevel(globalX, globalZ, perlins, terrainSeed, 1) - 0.1f, globalZ), Quaternion.identity, transform);
            assets.Add(rock);
            rock.GetComponent<RockGeneration>().Generate(rand.Next(10000));
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject go in assets)
        {
            Destroy(go);
        }
    }
}
