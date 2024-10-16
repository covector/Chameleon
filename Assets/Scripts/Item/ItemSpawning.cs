using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawning : ChunkSystem
{
    public TerrainGeneration tgen;
    public GameObject itemPrefab;
    public const float spacing = 22f;
    static Vector2Int[] neighbourhood = new Vector2Int[] {
        new Vector2Int(0, -1), new Vector2Int(0, 1),new Vector2Int(1, 0), new Vector2Int(-1, 0)
    };

    public ItemSpawning() : base(spacing, 2, 3) { }

    protected override bool CanLoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        return true;
    }

    protected override void LoadChunk(Vector2Int chunkInd, bool playerInChunk)
    {
        if (!chunks.ContainsKey(chunkInd))
        {
            // create
            CreateItem(chunkInd);
        } else
        {
            // restore
            if (!playerInChunk)
            {
                GameObject chunk = chunks[chunkInd];
                if (chunk.GetComponent<ItemPickUp>().pickedUp)
                {
                    chunk.GetComponent<ItemPickUp>().pickedUp = false;
                    chunk.transform.position = GetSpawnLocation(chunkInd);
                }
            }
        }
    }

    protected void CreateItem(Vector2Int chunkInd)
    {
        GameObject item = Instantiate(itemPrefab, GetSpawnLocation(chunkInd), Quaternion.identity, transform);
        chunks.Add(chunkInd, item);
    }

    protected Vector3 GetSpawnLocation(Vector2Int chunkInd)
    {
        Vector2 candidateLoc = Vector2.zero;
        // search neighboring chunk so as to space out more
        for (int i = 0; i < 10; i++)
        {
            candidateLoc = new Vector2(chunkSize * (chunkInd.x + Random.Range(0f, 1f)), chunkSize * (chunkInd.y + Random.Range(0f, 1f)));
            if (tgen.CheckSpawnVicinity(candidateLoc, 2f)) { continue; }
            bool retry = false;
            foreach (Vector2Int key in neighbourhood)
            {
                if (!chunks.ContainsKey(chunkInd + key)) { continue; }
                GameObject chunk = chunks[chunkInd + key];
                float sqrDist = (candidateLoc - Utils.ToVector2(chunk.transform.position)).sqrMagnitude;
                if (sqrDist < 100f) { retry = true; break; }
            }
            if (!retry) { break; }
        }

        return new Vector3(
            candidateLoc.x,
            tgen.GetGroudLevel(candidateLoc.x, candidateLoc.y, 1),
            candidateLoc.y
        );
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
        return (Utils.ToVector2(chunks[chunkInd].transform.position) - pos).sqrMagnitude < squareRadius;
    }
}