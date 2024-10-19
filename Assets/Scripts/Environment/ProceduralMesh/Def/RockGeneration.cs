using System.Collections.Generic;
using UnityEngine;
using static MeshBuilder;

public class RockGeneration : PreGenerate<RockGeneration>
{
    public override bool ItemSpawnCheck() { return true; }

    protected override void Edit(MeshBuilder meshBuilder)
    {
        TempMesh plane = TransformMesh(VoronoiDisplace(UNIT_CUBESPHERE, 0.1f, 1f), RandomTransform(rand));

        meshBuilder.AddMesh(plane, 0);
    }

    public override List<Vector2> SamplePoints(float chunkSize, Vector3 globalPosition, int seed)
    {
        float offset = chunkSize / 2f;
        JitterGridSampling jgs = new JitterGridSampling(chunkSize, chunkSize, chunkSize / 4f, chunkSize / 1.2f, globalPosition - new Vector3(offset, 0, offset), seed);
        return jgs.fill();
    }

    public override bool FilterPoint(float globalX, float globalZ, int maskSeed)
    {
        const float size = 1f;
        const float threshold = 0.4f;
        return Mathf.PerlinNoise(globalX * size, globalZ * size + maskSeed/1000) > threshold;
    }

    Matrix4x4 RandomTransform(System.Random random)
    {
        float scale = Utils.RandomRange(random, 0.4f, 1.2f);
        maxDims.Add(scale);
        return Matrix4x4.Scale(new Vector3(
            scale,
            scale * Utils.RandomRange(random, 0.4f, 1f),
            scale * Utils.RandomRange(random, 0.4f, 1f)
        )) *
        Utils.RandomRotation(random, Vector3.up * 360f);
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
