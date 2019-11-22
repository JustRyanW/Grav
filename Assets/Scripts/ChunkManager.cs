using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    public Vector2Int chunkSize = new Vector2Int(16, 16);
    public int renderDistance = 2;

    public Material material;

    List<Chunk> chunks = new List<Chunk>();

    void Start()
    {

    }

    void Update()
    {
        Vector2Int coord = Vec2Floor(Camera.main.transform.position / (Vector2)chunkSize);
        GenerateChunks(coord, renderDistance);
        CleanupChunks(coord, renderDistance);
    }

    void GenerateChunks(Vector2Int coord, int renderDistance)
    {
        for (int x = coord.x - renderDistance; x < coord.x + renderDistance; x++)
        {
            for (int y = coord.y - renderDistance; y < coord.y + renderDistance; y++)
            {
                bool chunkAtPos = false;
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.coord == new Vector2Int(x, y))
                        chunkAtPos = true;

                }
                if (!chunkAtPos)
                {
                    CreateChunk(new Vector2Int(x, y));
                }
            }
        }
    }

    void CleanupChunks(Vector2Int coord, int rd)
    {

        foreach (Chunk chunk in chunks)
        {
            Vector2Int c = chunk.coord;
            if (c.x > coord.x + rd || c.y > coord.y + rd || c.x < coord.x - rd || c.y < coord.y - rd)
            {
                Destroy(chunk.gameObject);
                chunks.Remove(chunk);
				CleanupChunks(coord, rd);
				return;
            }
        }
    }

    void CreateChunk(Vector2Int coord)
    {
        GameObject chunk = new GameObject("Chunk");
        chunk.transform.parent = this.transform;
        chunk.transform.position = (Vector2)coord * chunkSize;

        Chunk chunkScript = chunk.AddComponent<Chunk>();
        chunkScript.chunkSize = chunkSize;
        chunkScript.coord = coord;
        chunks.Add(chunkScript);

        chunk.GetComponent<MeshRenderer>().material = material;
    }

    Vector2Int Vec2Floor(Vector2 i)
    {
        return new Vector2Int(Mathf.FloorToInt(i.x), Mathf.FloorToInt(i.y));
    }
}
