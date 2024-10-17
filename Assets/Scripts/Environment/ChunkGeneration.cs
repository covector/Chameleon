using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkGeneration : MonoBehaviour
{
    Vector2Int chunkInd;
    static float size = 10f;
    static int step = 25;
    static Perlin[] perlins;
    static int terrainSeed;
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

    public static void Init(float size, int step, Perlin[] perlins, int seed)
    {
        ChunkGeneration.step = step;
        ChunkGeneration.perlins = perlins;
        ChunkGeneration.terrainSeed = seed;
        ChunkGeneration.size = size;
    }

    public void Create(Vector2Int chunkInd)
    {
        this.chunkInd = chunkInd;
        this.chunkSeed = terrainSeed + chunkInd.x * 727 - chunkInd.y * 757;  // arbitrary
        this.rand = new System.Random(chunkSeed);
        name = "Chunk_" + chunkInd.ToString();  // For debug
        GenerateChunk();
    }

    void GenerateChunk()
    {
        GetComponent<MeshFilter>().mesh = CreateGroundMesh(size, step);

        PlaceAssets();
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
                float y = GetGroudLevel(globalX, globalZ);
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

    public static float GetGroudLevel(float x, float z, int levels = 1)
    {
        float y = 0;
        for (int i = 0; i < perlins.Length; i++)
        {
            y += perlins[i].amplitude * Mathf.PerlinNoise(x * perlins[i].frequency + 1000f + terrainSeed, z * perlins[i].frequency + 1000f + terrainSeed);
            if (i >= levels - 1) { break; }
        }
        return y;
    }

    public static Quaternion GetTangentRotation(float x, float z, float yaw = 0f, int levels = 1, float delta = 0.5f)
    {
        Vector3 xDelta = Matrix4x4.Rotate(Quaternion.Euler(0f, yaw, 0f)) * Vector3.right * delta;
        Vector3 zDelta = Matrix4x4.Rotate(Quaternion.Euler(0f, yaw, 0f)) * Vector3.forward * delta;
        return Quaternion.Euler(
            Mathf.Atan((GetGroudLevel(x + zDelta.x, z + zDelta.z, levels) - GetGroudLevel(x - zDelta.x, z - zDelta.z, levels)) / (-2f * delta)) * Mathf.Rad2Deg,
            yaw,
            Mathf.Atan((GetGroudLevel(x + xDelta.x, z + xDelta.z, levels) - GetGroudLevel(x - xDelta.x, z - xDelta.z, levels)) / (2f * delta)) * Mathf.Rad2Deg
        );
    }

    public bool CheckSpawnVicinity(Vector2 pos, float offset)
    {
        foreach (GameObject g in assets)
        {
            float radius = g.GetComponent<ProceduralAsset>().MaxDim() + offset;
            if ((Utils.ToVector2(g.transform.position) - pos).sqrMagnitude < radius * radius) {
                return true;
            }
        }
        return false;
    }

    void PlaceAssets()
    {
        float offset = size / 2f;
        foreach (GameObject prefab in assetPrefabs) {
            ProceduralAsset procedural = prefab.GetComponent<ProceduralAsset>();
            List<Vector2> points = procedural.SamplePoints(size, transform.position, rand.Next(10000));
            foreach (Vector2 point in points)
            {
                float globalX = point.x + transform.position.x - offset;
                float globalZ = point.y + transform.position.z - offset;
                GameObject asset = Instantiate(prefab, new Vector3(globalX, GetGroudLevel(globalX, globalZ, 1) - 0.1f, globalZ), Quaternion.identity, transform);
                asset.name = "Asset_" + assets.Count;  // For debug
                procedural.Generate(rand.Next(10000));
                float radius = procedural.MaxDim() + ItemSpawning.vicinityRadiusOffset;
                if (itemSpawning.CheckSpawnVicinity(new Vector2(globalX, globalZ), radius * radius))
                {
                    Destroy(asset);
                }
                else
                {
                    assets.Add(asset);
                }
            }
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
