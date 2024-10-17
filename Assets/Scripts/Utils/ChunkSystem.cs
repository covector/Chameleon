using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkSystem : MonoBehaviour
{
    public float chunkSize { get; }
    public int loadRadius { get; }
    public int unloadRadius { get; }
    protected int sqrLoadRadius { get; }
    protected int sqrUnloadRadius { get; }
    public float loadInterval { get; }
    public float unloadInterval { get; }
    protected Queue<Vector2Int> unloadKeys = new Queue<Vector2Int>();
    protected Queue<Pair<Vector2Int, bool>> loadKeys = new Queue<Pair<Vector2Int, bool>>();
    protected float unloadChunkInterval = 0f;
    protected float loadChunkInterval = 0f;
    protected float elapsedUnloadTime = float.PositiveInfinity;
    protected float elapsedLoadTime = float.PositiveInfinity;
    public Transform player;
    protected Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    protected bool loadOrPrune = true;
    protected static Vector2Int[] neighbourhood = new Vector2Int[] {
        new Vector2Int(0, 0),
        new Vector2Int(0, -1), new Vector2Int(0, 1),new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(1, 1)
    };

    public ChunkSystem(float chunkSize = 10f, int loadRadius = 3, int unloadRadius = 5, float loadInterval = 0f, float unloadInterval = 0f)
    {
        this.chunkSize = chunkSize;
        this.loadRadius = loadRadius;
        this.sqrLoadRadius = loadRadius * loadRadius;
        this.unloadRadius = unloadRadius;
        this.sqrUnloadRadius = unloadRadius * unloadRadius;
        this.loadInterval = loadInterval;
        this.unloadInterval = unloadInterval;
    }

    void Update()
    {
        Vector2Int playerLoc = Utils.GetChunkIndFromCoord(player.position, chunkSize);
        if (loadOrPrune) {
            LoadChunks(playerLoc);
        } else
        {
            PruneChunks(playerLoc);
        }
    }

    void LoadChunks(Vector2Int playerLoc)
    {
        if (loadKeys.Count == 0)
        {
            Utils.MidPointCircle(loadRadius, (int x, int y) =>
            {
                Pair<Vector2Int, bool> pair = new Pair<Vector2Int, bool>(playerLoc + new Vector2Int(x, y), x >= -1 && x <= 1 && y >= -1 && y <= 1);
                if (CanLoadChunk(pair.left, pair.right))
                {
                    loadKeys.Enqueue(pair);
                }
            });
            if (loadKeys.Count > 0)
            {
                loadChunkInterval = loadInterval / loadKeys.Count;
            }
            elapsedLoadTime = 0f;
        }
        if (loadKeys.Count > 0)
        {
            elapsedLoadTime += Time.deltaTime / 2f;
            if (elapsedLoadTime > loadChunkInterval)
            {
                Pair<Vector2Int, bool> pair = loadKeys.Dequeue();
                LoadChunk(pair.left, pair.right);
                elapsedLoadTime -= loadChunkInterval;
            }
        }
    }

    void PruneChunks(Vector2Int playerLoc)
    {
        if (unloadKeys.Count == 0)
        {
            foreach (KeyValuePair<Vector2Int, GameObject> entry in chunks)
            {
                if ((entry.Key - playerLoc).sqrMagnitude > sqrUnloadRadius)
                {
                    unloadKeys.Enqueue(entry.Key);
                }
            }
            if (unloadKeys.Count > 0)
            {
                unloadChunkInterval = unloadInterval / unloadKeys.Count;
            }
            elapsedUnloadTime = 0f;
        }
        if (unloadKeys.Count > 0)
        {
            elapsedUnloadTime += Time.deltaTime / 2f;
            if (elapsedUnloadTime > unloadChunkInterval)
            {
                Vector2Int key = unloadKeys.Dequeue();
                UnloadChunk(key);
                elapsedUnloadTime -= unloadChunkInterval;
            }
        }
    }

    protected abstract bool CanLoadChunk(Vector2Int chunkInd, bool playerInChunk);
    protected abstract void LoadChunk(Vector2Int chunkInd, bool playerInChunk);
    protected abstract void UnloadChunk(Vector2Int chunkInd);

    // returns true if have things within radius
    public abstract bool CheckSpawnVicinity(Vector2 position, float squareRadius);
}
