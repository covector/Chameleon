using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector2> m_UVs = new List<Vector2>();
    private List<List<int>> m_Indices;
    public System.Random Random;

    public struct TempMesh
    {
        public List<Vector3> vertices;
        public List<Vector2> uvs;
        public List<int> indices;
        public TempMesh(List<Vector3> vertices, List<Vector2> uvs, List<int> indices)
        {
            this.vertices = vertices;
            this.uvs = uvs;
            this.indices = indices;
        }
    }

    public MeshBuilder(int materialCount = 1, int seed = 0)
    {
        m_Indices = new List<List<int>>();
        for (int i = 0; i < materialCount; i++)
        {
            m_Indices.Add(new List<int>());
        }
        Random = new System.Random(seed);
    }

    public int SubmeshCount()
    {
        return m_Indices.Count;
    }

    public static void AddTriangle(List<int> indices, int ind1, int ind2, int ind3)
    {
        indices.Add(ind1);
        indices.Add(ind2);
        indices.Add(ind3);
    }

    public static TempMesh CreateQuad(Vector3 start, Vector3 vec1, Vector3 vec2)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();

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

        return new TempMesh(vertices, uv, inds);
    }

    public static TempMesh CreateCylinder(float radius, float height, int step)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> inds = new List<int>();
        List<Vector2> uv = new List<Vector2>();

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
        }

        return new TempMesh(vertices, uv, inds);
    }

    public void AddMesh(TempMesh mesh, int materialInd = 0)
    {
        int currentInd = m_Vertices.Count;
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            m_Vertices.Add(mesh.vertices[i]);
        }
        for (int i = 0; i < mesh.indices.Count; i++)
        {
            m_Indices[materialInd].Add(mesh.indices[i] + currentInd);
        }
        for (int i = 0; i < mesh.uvs.Count; i++)
        {
            m_UVs.Add(mesh.uvs[i]);
        }
    }

    public static TempMesh TransformMesh(TempMesh mesh, Matrix4x4 transform)
    {
        TempMesh newMesh = mesh;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            vertices.Add(transform.MultiplyPoint3x4(mesh.vertices[i]));
        }
        newMesh.vertices = vertices;
        return newMesh;
    }

    public Mesh Build()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = m_Vertices.ToArray();
        mesh.uv = m_UVs.ToArray();
        mesh.subMeshCount = SubmeshCount();
        for (int i = 0; i < m_Indices.Count; i++)
        {
            mesh.SetTriangles(m_Indices[i].ToArray(), i);
        }

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
