using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkSystem : MonoBehaviour
{
    public float chunkSize { get; }
    public int loadRadius { get; }
    public int unloadRadius { get; }
    protected int sqrLoadRadius { get; }
    protected int sqrUnloadRadius { get; }
    public Transform player;
    protected Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    float elapsedTime = float.PositiveInfinity;

    public ChunkSystem(float chunkSize = 10f, int loadRadius = 3, int unloadRadius = 5)
    {
        this.chunkSize = chunkSize;
        this.loadRadius = loadRadius;
        this.sqrLoadRadius = loadRadius * loadRadius;
        this.unloadRadius = unloadRadius;
        this.sqrUnloadRadius = unloadRadius * unloadRadius;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 1f)
        {
            LoadChunks();
            elapsedTime = 0f;
        }
    }

    void LoadChunks()
    {
        Vector2Int playerLoc = Utils.GetChunkIndFromCoord(player.position, chunkSize);

        // remove chunks
        List<Vector2Int> removeKeys = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, GameObject> entry in chunks)
        {
            if ((entry.Key - playerLoc).sqrMagnitude > sqrUnloadRadius)
            {
                removeKeys.Add(entry.Key);
            }
        }
        foreach (Vector2Int removeKey in removeKeys)
        {
            UnloadChunk(removeKey);
        }

        // create chunks
        Utils.MidPointCircle(loadRadius, (int x, int y) =>
        {
            LoadChunk(playerLoc + new Vector2Int(x, y), x >= -1 && x <= 1 && y >= -1 && y <= 1);
        });
    }

    protected abstract void LoadChunk(Vector2Int chunkInd, bool playerInChunk);
    protected abstract void UnloadChunk(Vector2Int chunkInd);
}
