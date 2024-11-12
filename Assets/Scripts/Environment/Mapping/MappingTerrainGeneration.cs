using System;
using System.Collections.Generic;
using UnityEngine;

public class MappingTerrainGeneration : MonoBehaviour
{
    public AssetTemplate template;
    public float refreshPeriod = 0.1f;
    public bool update;
    int updateChunkInd = 0;
    int step;
    float chunkSize;
    ChunkGeneration.Perlin[] perlins;
    public int seed;
    void Start()
    {
        this.step = template.terrain_step;
        this.chunkSize = template.terrain_chunkSize;
        this.perlins = template.terrain_perlins;
        MappingChunk.Init(chunkSize, step, perlins, seed);
    }

    protected static Vector2Int[] chunkInds = new Vector2Int[] {
        new Vector2Int(0, 0),
        new Vector2Int(0, -1), new Vector2Int(0, 1),new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(1, 1)
    };
    private List<MappingChunk> chunks = new List<MappingChunk>();

    private float counter = float.PositiveInfinity;
    void Update()
    {
        if (counter > refreshPeriod)
        {
            MappingChunk.Init(chunkSize, step, perlins, seed);
            UpdateTerrain(updateChunkInd);
            counter = 0.0f;
            updateChunkInd = (updateChunkInd + 1) % chunkInds.Length;
        }
        if (update)
        {
            counter += Time.deltaTime;
        }
    }

    void UpdateTerrain(int ind)
    {
        if (chunks.Count == 0) { CreateChunks(); }
        chunks[ind].UpdateAssets(template);
    }

    void CreateChunks()
    {
        foreach (Vector2Int chunkInd in chunkInds) {
            GameObject chunk = new GameObject();
            chunk.AddComponent<MeshRenderer>().material = template.groundMaterial;
            chunk.AddComponent<MeshFilter>();
            chunk.transform.SetPositionAndRotation(new Vector3(chunkSize * chunkInd.x, 0f, chunkSize * chunkInd.y), Quaternion.identity);
            chunk.transform.parent = transform;
            chunk.AddComponent<MappingChunk>();
            chunk.GetComponent<MappingChunk>().Create(chunkInd);
            chunk.GetComponent<MappingChunk>().GenerateGround();
            chunks.Add(chunk.GetComponent<MappingChunk>());
        }
    }
}
