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

    public TerrainGeneration() : base(10f, 3, 3) {}
    
    void Start()
    {
        ChunkGeneration.Init(chunkSize, step, perlins, seed);
    }

    protected override bool CanLoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        return !chunks.ContainsKey(chunkInd) || !chunks[chunkInd].GetComponent<MeshRenderer>().enabled;
    }

    protected override void LoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        if (CanLoadChunk(chunkInd, playerInChunk)) {
            CreateChunk(chunkInd);
        }
    }

    protected void CreateChunk(Vector2Int chunkInd)
    {
        if (!chunks.ContainsKey(chunkInd))
        {
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkSize * chunkInd.x, 0f, chunkSize * chunkInd.y), Quaternion.identity, transform);
            chunk.GetComponent<ChunkGeneration>().itemSpawning = itemSpawning;
            chunk.GetComponent<ChunkGeneration>().Create(chunkInd);
            chunks.Add(chunkInd, chunk);
        } else
        {
            ShowChunk(chunks[chunkInd]);
        }
        
    }

    protected override void UnloadChunk(Vector2Int chunkInd, float squareRadius)
    {
        if (!chunks.ContainsKey(chunkInd)) { return; }
        GameObject chunk = chunks[chunkInd];
        if (squareRadius < 64)
        {
            if (!chunks[chunkInd].GetComponent<MeshRenderer>().enabled) { return; }
            HideChunk(chunk);
        } else
        {
            Destroy(chunk);
            chunks.Remove(chunkInd);
        }
    }

    private void HideChunk(GameObject chunk)
    {
        chunk.GetComponent<MeshRenderer>().enabled = false;
        foreach (Transform child in chunk.transform)
        {
            child.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void ShowChunk(GameObject chunk)
    {
        chunk.GetComponent<MeshRenderer>().enabled = true;
        foreach (Transform child in chunk.transform)
        {
            child.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public override bool CheckSpawnVicinity(Vector2 pos, float offset)
    {
        foreach (Vector2Int neighbour in neighbourhood)
        {
            Vector2Int chunkInd = Utils.GetChunkIndFromCoord(pos, chunkSize) + neighbour;
            if (!chunks.ContainsKey(chunkInd)) { continue; }
            if (chunks[chunkInd].GetComponent<ChunkGeneration>().CheckSpawnVicinity(pos, offset)) { return true; }
        }
        return false;
    }

    public Vector2 CheckCollision(Vector2 pos, float offset = 0f)
    {
        Vector2 newPos = pos;
        foreach (Vector2Int neighbour in neighbourhood)
        {
            Vector2Int chunkInd = Utils.GetChunkIndFromCoord(newPos, chunkSize) + neighbour;
            if (!chunks.ContainsKey(chunkInd)) { continue; }
            newPos = chunks[chunkInd].GetComponent<ChunkGeneration>().CheckCollision(newPos, offset);
        }
        return newPos;
    }

    public void CheckIntersection(Vector2 pos, float sqrSpeed, float offset = 0f)
    {
        foreach (Vector2Int neighbour in neighbourhood)
        {
            Vector2Int chunkInd = Utils.GetChunkIndFromCoord(pos, chunkSize) + neighbour;
            if (!chunks.ContainsKey(chunkInd)) { continue; }
            chunks[chunkInd].GetComponent<ChunkGeneration>().CheckIntersection(pos, sqrSpeed, offset);
        }
    }
}
