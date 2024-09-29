using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class TerrainGeneration : MonoBehaviour
{
    public float chunkSize = 10f;
    public int step = 25;
    public ChunkGeneration.Perlin[] perlins;
    public Transform player;
    public GameObject chunkPrefab;
    Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    int counter = 0;
    public int seed;

    void Update()
    {
        if (counter++ % 30 == 0)
        {
            GenerateTerrain();
        }
    }

    void GenerateTerrain()
    {
        LoadChunks();
    }

    void LoadChunks()
    {
        int sqrRadius = 25;
        Vector2Int playerLoc = GetChunkIndFromCoord(player.position);

        // remove chunks
        List<Vector2Int> removeKeys = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, GameObject> entry in chunks)
        {
            if ((entry.Key - playerLoc).sqrMagnitude > sqrRadius)
            {
                removeKeys.Add(entry.Key);
            }
        }
        foreach(Vector2Int removeKey in removeKeys)
        {
            DeleteChunk(removeKey);
        }

        // create chunks
        for (int i = 0; i < sqrRadius; i++)
        {
            for (int j = 0; j < sqrRadius; j++)
            {
                if (i < j) { continue; }
                if (i * i + j * j > sqrRadius) { break; }
                {
                    TryCreateChunk(playerLoc + new Vector2Int(i, j));
                    TryCreateChunk(playerLoc + new Vector2Int(i, -j));
                    TryCreateChunk(playerLoc + new Vector2Int(-i, -j));
                    TryCreateChunk(playerLoc + new Vector2Int(-i, j));
                    TryCreateChunk(playerLoc + new Vector2Int(j, i));
                    TryCreateChunk(playerLoc + new Vector2Int(j, -i));
                    TryCreateChunk(playerLoc + new Vector2Int(-j, -i));
                    TryCreateChunk(playerLoc + new Vector2Int(-j, i));

                }
            }
        }
    }

    void TryCreateChunk(Vector2Int chunkInd)
    {
        if (!chunks.ContainsKey(chunkInd)) {
            CreateChunk(chunkInd);
        }
    }

    void CreateChunk(Vector2Int chunkInd)
    {
        GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkSize * chunkInd.x, 0f, chunkSize * chunkInd.y), Quaternion.identity, transform);
        chunk.GetComponent<ChunkGeneration>().Init(chunkInd, chunkSize, step, perlins, seed);
        chunks.Add(chunkInd, chunk);
    }

    void CreateChunk(int x, int z)
    {
        CreateChunk(new Vector2Int(x, z));
    }

    void DeleteChunk(Vector2Int chunkInd)
    {
        if (!chunks.ContainsKey(chunkInd)) { return; }
        GameObject chunk = chunks.GetValueOrDefault(chunkInd);
        Destroy(chunk);
        chunks.Remove(chunkInd);
    }
    void DeleteChunk(int x, int z)
    {
        DeleteChunk(new Vector2Int(x, z));
    }

    Vector2Int GetChunkIndFromCoord(float x, float z)
    {
        return new Vector2Int(Mathf.FloorToInt(x / chunkSize), Mathf.FloorToInt(z / chunkSize));
    }

    Vector2Int GetChunkIndFromCoord(Vector3 loc)
    {
        return GetChunkIndFromCoord(loc.x, loc.z);
    }

    public float GetGroudLevel(float x, float z, int levels = 0)
    {
        return ChunkGeneration.GetGroudLevel(x, z, perlins, seed, levels);
    }
}
