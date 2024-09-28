using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkGeneration : MonoBehaviour
{
    float size = 10f;
    int step = 25;
    Perlin[] perlins;
    Material groundMaterial;

    [System.Serializable]
    public struct Perlin
    {
        public float frequency;
        public float amplitude;
    }

    private void Awake()
    {
    }

    public void Init(float size, int step, Perlin[] perlins, Material groundMaterial)
    {
        this.size = size;
        this.step = step;
        this.perlins = perlins;
        this.groundMaterial = groundMaterial;
        GenerateChunk();
    }

    void GenerateChunk()
    {
        GetComponent<MeshFilter>().mesh = CreateGroundMesh(size, step);
        GetComponent<MeshRenderer>().materials = new Material[] { groundMaterial };
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
