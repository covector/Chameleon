using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class TerrainGeneration : ChunkSystem
{
    public int step = 25;
    public ChunkGeneration.Perlin[] perlins;
    public GameObject chunkPrefab;
    public int seed;

    public TerrainGeneration() : base(10f, 3) { }

    protected override void LoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        if (!chunks.ContainsKey(chunkInd)) {
            CreateChunk(chunkInd);
        }
    }

    protected void CreateChunk(Vector2Int chunkInd)
    {
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkSize * chunkInd.x, 0f, chunkSize * chunkInd.y), Quaternion.identity, transform);
        chunk.GetComponent<ChunkGeneration>().Init(chunkInd, chunkSize, step, perlins, seed);
        chunks.Add(chunkInd, chunk);
    }

    protected override void UnloadChunk(Vector2Int chunkInd)
    {
        if (!chunks.ContainsKey(chunkInd)) { return; }
        GameObject chunk = chunks[chunkInd];
        Destroy(chunk);
        chunks.Remove(chunkInd);
    }

    public float GetGroudLevel(float x, float z, int levels = 0)
    {
        return ChunkGeneration.GetGroudLevel(x, z, perlins, seed, levels);
    }
}
