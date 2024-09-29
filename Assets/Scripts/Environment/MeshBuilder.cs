using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MeshBuilder
{
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector3> m_Normals = new List<Vector3>();
    private List<Vector2> m_UVs = new List<Vector2>();
    private List<List<int>> m_Indices;
    public System.Random Random;

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

    public void AddTriangle(int ind1, int ind2, int ind3, int materialInd)
    {
        m_Indices[materialInd].Add(ind1);
        m_Indices[materialInd].Add(ind2);
        m_Indices[materialInd].Add(ind3);
    }

    public void AddQuad(Vector3 start, Vector3 vec1, Vector3 vec2, int materialInd, Matrix4x4 transform)
    {
        Vector3 corner1 = transform.MultiplyPoint3x4(start);
        Vector3 corner2 = transform.MultiplyPoint3x4(start + vec1);
        Vector3 corner3 = transform.MultiplyPoint3x4(start + vec1 + vec2);
        Vector3 corner4 = transform.MultiplyPoint3x4(start + vec2);
        int currentInd = m_Vertices.Count;
        m_Vertices.Add(corner1);
        m_Vertices.Add(corner2);
        m_Vertices.Add(corner3);
        m_Vertices.Add(corner4);

        AddTriangle(currentInd, currentInd + 2, currentInd + 1, materialInd);
        AddTriangle(currentInd, currentInd + 3, currentInd + 2, materialInd);

        Vector3 normal = Vector3.Cross(corner2 - corner1, corner4 - corner1).normalized;
        m_Normals.Add(normal);
        m_Normals.Add(normal);
        m_Normals.Add(normal);
        m_Normals.Add(normal);

        m_UVs.Add(new Vector2(0, 0));
        m_UVs.Add(new Vector2(1, 0));
        m_UVs.Add(new Vector2(1, 1));
        m_UVs.Add(new Vector2(0, 1));
    }
    public void AddQuad(Vector3 start, Vector3 vec1, Vector3 vec2, int materialInd)
    {
        AddQuad(start, vec1, vec2, materialInd, Matrix4x4.identity);
    }

    public void AddCylinder(float radius, float height, int step, int materialInd, Matrix4x4 transform)
    {
        int currentInd = m_Vertices.Count;
        for (int i = 0; i <= step; i++)
        {
            float sin = Mathf.Sin(Mathf.PI * 2f * i / step);
            float cos = Mathf.Cos(Mathf.PI * 2f * i / step);

            m_Vertices.Add(transform.MultiplyPoint3x4(new Vector3(radius * cos, 0, radius * sin)));
            m_Vertices.Add(transform.MultiplyPoint3x4(new Vector3(radius * cos, height, radius * sin)));

            if (i != step)
            {
                int offset = currentInd + i * 2;
                AddTriangle(offset, offset + 1, offset + 2, materialInd);
                AddTriangle(offset + 2, offset + 1, offset + 3, materialInd);
            }

            Vector3 normal = transform.MultiplyVector(new Vector3(radius * cos, 0, radius * sin)).normalized;
            m_Normals.Add(normal);
            m_Normals.Add(normal);

            m_UVs.Add(new Vector2(1f * i / step, 0));
            m_UVs.Add(new Vector2(1f * i / step, 1));
        }
    }

    public Mesh Build()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = m_Vertices.ToArray();
        mesh.normals = m_Normals.ToArray();
        mesh.uv = m_UVs.ToArray();
        mesh.subMeshCount = SubmeshCount();
        for (int i = 0; i < m_Indices.Count; i++)
        {
            mesh.SetTriangles(m_Indices[i].ToArray(), i);
        }

        mesh.RecalculateBounds();

        return mesh;
    }
}
