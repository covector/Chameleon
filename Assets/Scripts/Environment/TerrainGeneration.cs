using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class TerrainGeneration : ChunkSystem
{
    public int step = 25;
    public ChunkGeneration.Perlin[] perlins;
    public GameObject chunkPrefab;
    public int seed;
    public ItemSpawning itemSpawning;

    public TerrainGeneration() : base(10f, 3, 5) {}
    
    void Start()
    {
        ChunkGeneration.Init(chunkSize, step, perlins, seed);
    }

    protected override bool CanLoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        return !chunks.ContainsKey(chunkInd);
    }

    protected override void LoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        if (CanLoadChunk(chunkInd, playerInChunk)) {
            CreateChunk(chunkInd);
        }
    }

    protected void CreateChunk(Vector2Int chunkInd)
    {
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkSize * chunkInd.x, 0f, chunkSize * chunkInd.y), Quaternion.identity, transform);
        chunk.GetComponent<ChunkGeneration>().itemSpawning = itemSpawning;
        chunk.GetComponent<ChunkGeneration>().Create(chunkInd);
        chunks.Add(chunkInd, chunk);
    }

    protected override void UnloadChunk(Vector2Int chunkInd)
    {
        if (!chunks.ContainsKey(chunkInd)) { return; }
        GameObject chunk = chunks[chunkInd];
        Destroy(chunk);
        chunks.Remove(chunkInd);
    }

    public override bool CheckSpawnVicinity(Vector2 pos, float squareRadius)
    {
        Vector2Int chunkInd = Utils.GetChunkIndFromCoord(pos, chunkSize);
        if (!chunks.ContainsKey(chunkInd)) { return false; }
        return chunks[chunkInd].GetComponent<ChunkGeneration>().CheckSpawnVicinity(pos, squareRadius);
    }
}
