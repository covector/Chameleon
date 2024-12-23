using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector2> m_UVs = new List<Vector2>();
    private List<List<int>> m_Indices;
    private List<Vector3> m_Normals = new List<Vector3>();

    public List<Vector3> Vertices { get { return m_Vertices; } }
    public List<Vector2> UVs { get { return m_UVs; } }
    private List<List<int>> Indices { get { return m_Indices; } }
    private List<Vector3> Normals { get { return m_Normals; } }

    public struct TempMesh
    {
        public List<Vector3> vertices;
        public List<Vector2> uvs;
        public List<int> indices;
        public List<Vector3> normals;
        public TempMesh(List<Vector3> vertices, List<Vector2> uvs, List<int> indices, List<Vector3> normals)
        {
            this.vertices = vertices;
            this.uvs = uvs;
            this.indices = indices;
            this.normals = normals;
        }
    }

    public MeshBuilder(int materialCount = 1)
    {
        m_Indices = new List<List<int>>();
        for (int i = 0; i < materialCount; i++)
        {
            m_Indices.Add(new List<int>());
        }
    }

    public int SubmeshCount()
    {
        return m_Indices.Count;
    }

    public int GetLastVertInd()
    {
        return m_Vertices.Count - 1;
    }

    public static void AddTriangle(List<int> indices, int ind1, int ind2, int ind3)
    {
        indices.Add(ind1);
        indices.Add(ind2);
        indices.Add(ind3);
    }

    public static void AddQuad(List<int> indices, int ind1, int ind2, int ind3, int ind4)
    {
        AddTriangle(indices, ind1, ind4, ind2);
        AddTriangle(indices, ind1, ind3, ind4);
    }

    public static TempMesh CreateQuad(Vector3 start, Vector3 vec1, Vector3 vec2)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        Vector3 corner1 = start;
        Vector3 corner2 = start + vec1;
        Vector3 corner3 = corner2 + vec2;
        Vector3 corner4 = start + vec2;
        vertices.Add(corner1);
        vertices.Add(corner2);
        vertices.Add(corner3);
        vertices.Add(corner4);

        AddTriangle(inds, 0, 2, 1);
        AddTriangle(inds, 0, 3, 2);

        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0, 1));

        Vector3 normal = Vector3.Cross(vec2, vec1).normalized;
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        return new TempMesh(vertices, uv, inds, normals);
    }

    public static TempMesh CreateQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        AddTriangle(inds, 0, 2, 1);
        AddTriangle(inds, 0, 3, 2);

        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(1, 1));
        uv.Add(new Vector2(0, 1));

        Vector3 normal = Vector3.Cross(v4 - v1, v2 - v1).normalized;
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        return new TempMesh(vertices, uv, inds, normals);
    }

    public static TempMesh CreatePlane(Vector3 start, Vector3 vec1, Vector3 vec2, int step1, int step2)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        Vector3 currentVec = start;
        Vector3 quadVec1 = vec1 / step1;
        Vector3 quadVec2 = vec2 / step2;
        Vector3 normal = Vector3.Cross(quadVec2, quadVec1).normalized;
        float uv1 = 0;
        float uv2 = 0;
        float uvStep1 = 1f / step1;
        float uvStep2 = 1f / step2;

        for (int i = 0; i <= step2; i++)
        {
            uv1 = 0;
            Vector3 currentVecInner = currentVec;
            for (int j = 0; j <= step1; j++)
            {
                vertices.Add(currentVecInner);
                normals.Add(normal);
                uv.Add(new Vector2(uv1, uv2));
                if (i != 0 && j != 0)
                {
                    AddTriangle(inds, (i - 1) * (step1 + 1) + j - 1, i * (step1 + 1) + j, (i - 1) * (step1 + 1) + j);
                    AddTriangle(inds, (i - 1) * (step1 + 1) + j - 1, i * (step1 + 1) + j - 1, i * (step1 + 1) + j);
                }
                currentVecInner += quadVec1;
                uv1 += uvStep1;
            }
            currentVec += quadVec2;
            uv2 += uvStep2;
        }

        return new TempMesh(vertices, uv, inds, normals);
    }

    public static TempMesh CreateCylinder(float radius, float height, int step)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i <= step; i++)
        {
            float sin = Mathf.Sin(Mathf.PI * 2f * i / step);
            float cos = Mathf.Cos(Mathf.PI * 2f * i / step);

            vertices.Add(new Vector3(radius * cos, 0, radius * sin));
            vertices.Add(new Vector3(radius * cos, height, radius * sin));

            if (i != step)
            {
                int offset = i * 2;
                AddTriangle(inds, offset, offset + 1, offset + 2);
                AddTriangle(inds, offset + 2, offset + 1, offset + 3);
            }

            uv.Add(new Vector2(1f * i / step, 0));
            uv.Add(new Vector2(1f * i / step, 1));

            Vector3 normal = new Vector3(cos, 0, sin);
            normals.Add(normal);
            normals.Add(normal);
        }

        return new TempMesh(vertices, uv, inds, normals);
    }

    public void MergeCylinders(int lastVertInd1, int lastVertInd2, int vertexCount)
    {
        MergeCylinders(lastVertInd1, lastVertInd2, vertexCount, this);
    }

    public void MergeCylinders(int lastVertInd1, int lastVertInd2, int vertexCount, MeshBuilder builder1)
    {
        int step = vertexCount / 2;

        for (int i = 0; i < step; i++)
        {
            int ind1 = (lastVertInd1 - 2 * step + 1) + (2 * i + 1);
            int ind2 = (lastVertInd2 - 2 * step + 1) + (2 * i);
            Vector3 midPt = (builder1.m_Vertices[ind1] + m_Vertices[ind2]) / 2f;
            builder1.Vertices[ind1] = midPt;
            m_Vertices[ind2] = midPt;
        }
    }

    public static TempMesh CreateCube(float length, int step)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        float halfLength = length / 2;
        TempMesh[] sidePlanes = new TempMesh[6];
        sidePlanes[0] = CreatePlane(new Vector3(-halfLength, halfLength, -halfLength), length * Vector3.right, length * Vector3.forward, step, step);   // top
        sidePlanes[1] = CreatePlane(new Vector3(halfLength, -halfLength, -halfLength), length * Vector3.left, length * Vector3.forward, step, step);    // bottom
        sidePlanes[2] = CreatePlane(new Vector3(-halfLength, -halfLength, halfLength), length * Vector3.up, length * Vector3.right, step, step);        // front
        sidePlanes[3] = CreatePlane(new Vector3(halfLength, -halfLength, halfLength), length * Vector3.up, length * Vector3.back, step, step);          // right
        sidePlanes[4] = CreatePlane(new Vector3(halfLength, -halfLength, -halfLength), length * Vector3.up, length * Vector3.left, step, step);         // back
        sidePlanes[5] = CreatePlane(new Vector3(-halfLength, -halfLength, -halfLength), length * Vector3.up, length * Vector3.forward, step, step);     // left

        int offset = 0;
        int planePoints = (step + 1) * (step + 1);
        foreach (TempMesh plane in sidePlanes)
        {
            vertices.AddRange(plane.vertices);
            foreach (int ind in plane.indices) { inds.Add(ind + offset); }
            offset += planePoints;
            uv.AddRange(plane.uvs);
            normals.AddRange(plane.normals);
        }
       
        return new TempMesh(vertices, uv, inds, normals);
    }

    public static TempMesh CreateCubeSphere(float radius, int step)
    {
        TempMesh cube = CreateCube(2f * radius, step);
        for (int i = 0; i < cube.vertices.Count; i++)
        {
            cube.vertices[i] = cube.vertices[i].normalized;
            cube.normals[i] = cube.vertices[i];
        }
        return cube;
    }

    public static TempMesh VoronoiDisplace(TempMesh mesh, System.Random rand, float density, float intensity)
    {
        TempMesh newMesh = mesh;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        // Sample random points
        List<Vector3> randomPt = new List<Vector3>();
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            if (rand.NextDouble() < density)
            {
                randomPt.Add(mesh.vertices[i]);
            }
        }
        // Displace based on squared distance to nearest random point
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            float minDist = float.MaxValue;
            Vector3 minDistNormal = Vector3.zero;
            for (int j = 0; j < randomPt.Count; j++)
            {
                float dist = Vector3.SqrMagnitude(mesh.vertices[i] - randomPt[j]);
                if (dist < minDist) { minDist = dist; minDistNormal = mesh.normals[i]; }
            }
            vertices.Add(mesh.vertices[i] + intensity * minDist * newMesh.normals[i]);
            normals.Add(minDistNormal);
            //vertices[i] += intensity * Mathf.Sqrt(minDist) * newMesh.normals[i];
        }
        newMesh.vertices = vertices;
        newMesh.normals = normals;
        return newMesh;
    }

    public static TempMesh PerlinDisplace(TempMesh mesh, float frequency, float intensity)
    {
        TempMesh newMesh = mesh;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            vertices.Add(mesh.vertices[i] + intensity * (Mathf.PerlinNoise(frequency * newMesh.uvs[i].x, frequency * newMesh.uvs[i].y) - 0.5f) * newMesh.normals[i]);
        }
        newMesh.vertices = vertices;
        return newMesh;
    }

    public static TempMesh RandomDisplace(TempMesh mesh, System.Random rand, float intensity)
    {
        TempMesh newMesh = mesh;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            vertices.Add(mesh.vertices[i] + Utils.RandomRange(rand, -intensity, intensity) * newMesh.normals[i]);
        }
        newMesh.vertices = vertices;
        return newMesh;
    }

    public void AddMesh(TempMesh mesh, int materialInd = 0)
    {
        int currentInd = m_Vertices.Count;
        m_Vertices.AddRange(mesh.vertices);
        for (int i = 0; i < mesh.indices.Count; i++)
        {
            m_Indices[materialInd].Add(mesh.indices[i] + currentInd);
        }
        m_UVs.AddRange(mesh.uvs);
        m_Normals.AddRange(mesh.normals);
    }

    public static TempMesh TransformMesh(TempMesh mesh, Matrix4x4 transform)
    {
        TempMesh newMesh = mesh;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            vertices.Add(transform.MultiplyPoint3x4(mesh.vertices[i]));
            normals.Add(transform.MultiplyVector(mesh.normals[i]).normalized);
        }
        newMesh.vertices = vertices;
        newMesh.normals = normals;
        return newMesh;
    }

    public Mesh Build(bool recalculateNormals)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = m_Vertices.ToArray();
        mesh.uv = m_UVs.ToArray();
        mesh.subMeshCount = SubmeshCount();
        for (int i = 0; i < m_Indices.Count; i++)
        {
            mesh.SetTriangles(m_Indices[i].ToArray(), i);
        }
        mesh.normals = m_Normals.ToArray();

        mesh.RecalculateBounds();
        if (recalculateNormals) { mesh.RecalculateNormals(); }
        mesh.RecalculateTangents();
        return mesh;
    }
}
