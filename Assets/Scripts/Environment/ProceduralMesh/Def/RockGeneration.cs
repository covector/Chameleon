using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MeshBuilder;

public class RockGeneration : PreGenerate<RockGeneration>
{
    private float maxDim;

    public override float MaxDim()
    {
        return maxDim;
    }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        //TempMesh plane = CreatePlane(Vector3.up, Vector3.left, Vector3.up, 5, 5);
        //TempMesh plane = TransformMesh(CreateCube(1f, 3), Matrix4x4.Translate(new Vector3(0f, 1f, 0f)));
         //TempMesh plane = TransformMesh(UNIT_CUBE, Matrix4x4.Scale(new Vector3(2f, 2f, 2f)));
        //TempMesh plane = TransformMesh(UNIT_CUBESPHERE, RandomTransform(rand));
        TempMesh plane = TransformMesh(VoronoiDisplace(UNIT_CUBESPHERE, 0.1f, 1f), RandomTransform(rand));
        //TempMesh plane = TransformMesh(PerlinDisplace(UNIT_CUBESPHERE, 1f, 1f), RandomTransform(rand));
        

        meshBuilder.AddMesh(plane, 0);
    }

    Matrix4x4 RandomTransform(System.Random random)
    {
        float scale = 0.8f * (float)random.NextDouble() + 0.4f;
        maxDim = scale;
        return Matrix4x4.Scale(new Vector3(
            scale,
            scale * (0.6f * (float)random.NextDouble() + 0.4f),
            scale * (0.6f * (float)random.NextDouble() + 0.4f)
        )) *
        Matrix4x4.Rotate(Quaternion.Euler(
            0f,
            360f * (float)random.NextDouble(),
            0f
        ));
    }

    private TempMesh VoronoiDisplace(TempMesh mesh, float density, float intensity)
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

    private TempMesh PerlinDisplace(TempMesh mesh, float frequency, float intensity)
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
}
