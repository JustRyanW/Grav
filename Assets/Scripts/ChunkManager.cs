using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Settings")]
    public Vector2Int chunkSize = new Vector2Int(16, 16);
    public int renderDistance = 2;
    public float surfaceValue = 0;
    public bool smoothed = true;
    public Material material;

    [Header("Noise")]
    public bool regenerateTerrain = false;
    public bool randomizeSeed = false;
    public int seed = 0;
    public float scale = 20f;

    [Header("Brush")]
    public float brushSize = 5f;

    [Header("Debug")]
    public bool drawDebug = false;

    [Header("Private")]
    List<Chunk> chunks = new List<Chunk>();
    List<Chunk> updateQueue = new List<Chunk>();

    void Start()
    {

    }

    void Update()
    {
        Transform cam = Camera.main.transform;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input != Vector2.zero)
            cam.position += (Vector3)input;



        Vector2Int coord = Vec2Floor(cam.position / (Vector2)chunkSize);
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
        GameObject chunkObject = new GameObject("Chunk");
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.coord = coord;
        chunks.Add(chunk);
        SetChunk(chunk);
    }

    void SetChunk(Chunk chunk)
    {
        chunk.gameObject.transform.parent = this.transform;
        chunk.gameObject.transform.position = (Vector2)chunk.coord * chunkSize;
        chunk.chunkSize = chunkSize;
        chunk.surfaceValue = surfaceValue;
        chunk.smoothed = smoothed;
        chunk.GetComponent<MeshRenderer>().material = material;

        chunk.scale = scale;

        chunk.brushSize = brushSize;

        chunk.drawDebug = drawDebug;

		chunk.regenerateTerrain = true;
    }

    Vector2Int Vec2Floor(Vector2 i)
    {
        return new Vector2Int(Mathf.FloorToInt(i.x), Mathf.FloorToInt(i.y));
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
			foreach (Chunk chunk in chunks)
			{
				SetChunk(chunk);
			}
        }
    }
}
