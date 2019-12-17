using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{

    [Header("Settings")]
    public Vector2Int size = new Vector2Int(8, 8);
    public int renderDistance = 2;
    public float surfaceValue = 0.5f;
    public bool smoothed = true;
    public Material material;

    [Header("Private")]
    List<Chunk> chunks = new List<Chunk>();
    List<Chunk> updateQueue = new List<Chunk>();

    [Header("Noise")]
    public float scale = 10;
    public Vector2 offset = Vector2.zero;
    public int seed = 0;

    private void Start()
    {
        Chunk newChunk = new Chunk(transform, size, "Chunk " + chunks.Count);
        chunks.Add(newChunk);
    }

    private void Update()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.GenerateTerrain(seed, scale, offset);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.DrawDebug();
        }
    }
}

public class Chunk
{
    float[,] terrainMap;
    Vector3[,,] vertexMap;
    int[,,] indexMap;

    public List<Vector3> verts = new List<Vector3>();
    public List<int> indices = new List<int>();

    GameObject chunk;
    Mesh mesh;
    MeshFilter meshFilter;

    public Chunk(Transform parent, Vector2Int size, string name = "Chunk")
    {
        chunk = new GameObject(name);
        chunk.transform.parent = parent;
        meshFilter = chunk.AddComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        mesh = new Mesh();

        terrainMap = new float[size.x,size.y];
    }

    public void GenerateTerrain(int seed, float scale, Vector3 offset)
    {
        for (int x = 0; x < terrainMap.GetLength(0); x++)
        {
            for (int y = 0; y < terrainMap.GetLength(1); y++)
            {
                terrainMap[x, y] = Marching.Perlin3D(x, y, seed * scale, scale, offset) - 0.5f; // May need to scale for repeating noise
            }
        }
    }

    public void DrawDebug()
    {
        for (int x = 0; x < terrainMap.GetLength(0); x++)
        {
            for (int y = 0; y < terrainMap.GetLength(1); y++)
            {
                Gizmos.color = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(-0.5f, 0.5f, terrainMap[x, y]));
                Gizmos.DrawCube(new Vector2(x, y), Vector3.one * 1);
            }
        }
    }


}

static class Marching
{

    //  1----5----2
    //  |         |
    //  4         6
    //  |         |
    //  0----7----3
    public static readonly Vector2Int[] vertices = new Vector2Int[4]
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0)
    };

    public static readonly Vector2Int[,] edges = new Vector2Int[4, 2]
    {
        {vertices[0], vertices[1]},
        {vertices[1], vertices[2]},
        {vertices[2], vertices[3]},
        {vertices[3], vertices[0]}
    };

    public static readonly Vector2Int[] offsets = new Vector2Int[3] {
        new Vector2Int(0, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1)
    };

    //  1---------2
    //  |         |
    //  |         |
    //  |         |
    //  0---------3
    public static readonly int[,] triangulations = new int[16, 9]   // Vertex: 0123
    {                                                               // Binary: 1248
        {-1, -1, -1, -1, -1, -1, -1, -1, -1},                       //    00 = 0000
        {0, 4, 7, -1, -1, -1, -1, -1, -1},                          //    01 = 1000
        {1, 5, 4, -1, -1, -1, -1, -1, -1},                          //    02 = 0100
        {0, 1, 5, 5, 7, 0, -1, -1, -1},                             //    03 = 1100
        {2, 6, 5, -1, -1, -1, -1, -1, -1},                          //    04 = 0010
        {0, 4, 7, 2, 6, 5, -1, -1, -1},                             //    05 = 1010
        {1, 2, 4, 2, 6, 4, -1, -1, -1},                             //    06 = 0110
        {0, 1, 2, 0, 2, 7, 2, 6, 7},                                //    07 = 1110
        {3, 7, 6, -1, -1, -1, -1, -1, -1},                          //    08 = 0001
        {3, 0, 6, 0, 4, 6, -1, -1, -1},                             //    09 = 1001
        {1, 5, 4, 3, 7, 6, -1, -1, -1},                             //    10 = 0101
        {0, 1, 5, 0, 5, 6, 0, 6, 3},                                //    11 = 1101
        {2, 3, 7, 2, 7, 5, -1, -1, -1},                             //    12 = 0011
        {0, 2, 3, 0, 4, 2, 2, 4, 5},                                //    13 = 1011
        {1, 2, 4, 2, 7, 4, 2, 3, 7},                                //    14 = 0111
        {0, 1, 2, 0, 2, 3, -1, -1, -1},                             //    15 = 1111
    };

    public static float Perlin3D(float x, float y, float z, float scale = 1f, Vector3? offset = null)
    {
        if (offset != null)
        {
            Vector3 o = (Vector3)offset;
            x += o.x; y += o.y; z += o.z;
        }

        if (scale > 0.001f)
        {
            x /= scale; y /= scale; z /= scale;
        }

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);

        float xz = Mathf.PerlinNoise(x, z);
        float zy = Mathf.PerlinNoise(z, y);
        float yx = Mathf.PerlinNoise(y, x);

        float xyz = xy + yz + zx + xz + zy + yx;
        return xyz / 6f;
    }
}