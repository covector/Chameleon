using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent (typeof(MeshCollider))]
public class TerrainGeneration : MonoBehaviour
{
    public float size = 10f;
    public int step = 25;
    public int colStep = 20;
    public Perlin[] perlins;

    [System.Serializable]
    public struct Perlin
    {
        public float frequency;
        public float amplitude;
    }

    void Start()
    //void Update()
    {
        GenerateTerrain(size, step, colStep);
    }

    void GenerateTerrain(float size, int step, int colStep)
    {
        GetComponent<MeshFilter>().mesh = CreateTerrainMesh(size, step);
        GetComponent<MeshCollider>().sharedMesh = CreateTerrainMesh(size, colStep);
    }

    Mesh CreateTerrainMesh(float size, int step)
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

        return mesh;
    }

    public float GetGroudLevel(float x, float z, int levels = 0)
    {
        float y = 0;
        for (int i = 0; i < perlins.Length; i++)
        {
            y += perlins[i].amplitude * Mathf.PerlinNoise(x * perlins[i].frequency + 1000f, z * perlins[i].frequency + 1000f);
            if (i == levels - 1) { break; }
        }
        return y;
    }
}
