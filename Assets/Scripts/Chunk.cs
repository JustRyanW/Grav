using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public Vector2Int chunkSize = new Vector2Int(32, 32);
    public Vector2Int coord;
    public float surfaceValue = 0;
    public bool smoothed = true;

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
    float[,] voxelMap;
    List<Vector3> verts = new List<Vector3>();
    List<int> indices = new List<int>();

    Mesh mesh;
    MeshFilter meshFilter;

    bool initialized = false;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        GenerateNoiseMap();
        ClearMesh();
        MarchSquares();
        CreateMesh();

        initialized = true;
    }

    private void Update()
    {
        // add a check if the brush pos plus or minus the max size is inside the chunk
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (Input.GetButton("Fire1"))
        {
            DrawBrush(true, mousePos, brushSize);
        }
        else if (Input.GetButton("Fire2"))
        {
            DrawBrush(false, mousePos, brushSize);
        }
    }

    void DrawBrush(bool addTerrain, Vector2 pos, float brushSize)
    {
        Vector2Int minMaxX = new Vector2Int((int)(pos.x - brushSize), (int)(pos.x + brushSize));
        Vector2Int minMaxY = new Vector2Int((int)(pos.y - brushSize), (int)(pos.y + brushSize));
        Vec2IntClamp(ref minMaxX, 0, chunkSize.x + 1);
        Vec2IntClamp(ref minMaxY, 0, chunkSize.y + 1);

        for (int x = minMaxX.x; x < minMaxX.y; x++)
        {
            for (int y = minMaxY.x; y < minMaxY.y; y++)
            {
                float halfBrushSize = brushSize / 2;
                float dist = Vector2.Distance(new Vector2(x, y), pos);
                if (addTerrain)
                {
                    if (dist <= brushSize && voxelMap[x, y] < halfBrushSize - dist)
                    {
                        voxelMap[x, y] = halfBrushSize - dist;
                    }
                }
                else
                {
                    if (dist <= brushSize && voxelMap[x, y] > -halfBrushSize + dist)
                    {
                        voxelMap[x, y] = -halfBrushSize + dist;
                    }
                }
            }
        }
        UpdateMesh();
    }

    void UpdateMesh()
    {
        ClearMesh();
        MarchSquares();
        CreateMesh();
    }

    void MarchSquares()
    {
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                MarchSquare(x, y);
            }
        }
    }

    void MarchSquare(int x, int y)
    {
        // Finds the triangle index of the current cube from each of its corners
        int triangulationIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            int xOffset = SquareData.vertices[i].x;
            int yOffset = SquareData.vertices[i].y;

            if (voxelMap[x + xOffset, y + yOffset] >= surfaceValue)
                triangulationIndex |= 1 << i;
        }

        // Returns if the square is empty
        if (triangulationIndex == 0)
            return;

        // Loops through each tri in a cube
        int vertIndex = 0;
        for (int tri = 0; tri < 4; tri++)
        {
            // Loops thorugh each vert in a tri
            for (int vert = 0; vert < 3; vert++)
            {
                // lots of optimisation here

                // Gets the index of the current vertex
                int index = SquareData.triangulations[triangulationIndex, vertIndex];
                if (index == -1)
                    return;

                Vector2Int pos = new Vector2Int(x, y);
                Vector2 vertPos;

                // Checks if the vert is a corner or an endge
                if (index % 2 == 0)
                {
                    // Add corner vert
                    index = index / 2;
                    vertPos = (Vector2)pos + SquareData.vertices[index];
                }
                else
                {
                    // Get corners to lerp between
                    index = (index - 1) / 2;
                    Vector2Int a = pos + SquareData.edges[index, 0];
                    Vector2Int b = pos + SquareData.edges[index, 1];

                    float surface;
                    if (smoothed)
                    {
                        // Lerps between corners relitve to their value
                        float ia = voxelMap[a.x, a.y];
                        float ib = voxelMap[b.x, b.y];
                        surface = Mathf.InverseLerp(ia, ib, surfaceValue);
                    }
                    else
                    {
                        // Lerp halfway
                        surface = 0.5f;
                    }

                    vertPos = Vector2.Lerp(a, b, surface);
                }

                indices.Add(VertForIndice(vertPos));
                vertIndex++;
            }
        }
    }

    int VertForIndice(Vector3 vert)
    {
        // Loop through all the vertices
        for (int i = 0; i < verts.Count; i++)
        {
            // If a vert matches ours then return this one
            if (verts[i] == vert)
                return i;
        }

        // If no matches are found, add this vert to the list and return the last index;
        verts.Add(vert);
        return verts.Count - 1;
    }

    void ClearMesh()
    {
        mesh.Clear();
        verts = new List<Vector3>();
        indices = new List<int>();
    }

    void CreateMesh()
    {
        mesh.vertices = verts.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
    }

    void GenerateNoiseMap()
    {
        if (randomizeSeed)
            seed = Random.Range(0, 100000);

        voxelMap = new float[chunkSize.x + 1, chunkSize.y + 1];

        for (int x = 0; x <= chunkSize.x; x++)
        {
            for (int y = 0; y <= chunkSize.y; y++)
            {
                voxelMap[x, y] = (Perlin3D((x + transform.position.x) / scale, (y + transform.position.y) / scale, seed) - 0.5f) * 10;
                if (voxelMap[x, y] < -0.5f)
                    voxelMap[x, y] = -0.5f;
                else if (voxelMap[x, y] > 0.5f)
                    voxelMap[x, y] = 0.5f;
            }
        }
    }

    float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);

        float xz = Mathf.PerlinNoise(x, z);
        float zy = Mathf.PerlinNoise(z, y);
        float yx = Mathf.PerlinNoise(y, x);

        float xyz = xy + yz + zx + xz + zy + yx;
        return xyz / 6f;
    }

    void Vec2IntClamp(ref Vector2Int vector, int min, int max)
    {
        vector.x = (int)Mathf.Clamp(vector.x, min, max);
        vector.y = (int)Mathf.Clamp(vector.y, min, max);
    }

    private void OnDrawGizmos()
    {
        if (drawDebug && Application.isPlaying)
        {
            for (int x = 0; x <= chunkSize.x; x++)
            {
                for (int y = 0; y <= chunkSize.y; y++)
                {
                    Gizmos.color = Color.Lerp(Color.red, Color.green, Mathf.InverseLerp(-0.5f, 0.5f, voxelMap[x, y]));
                    Gizmos.DrawCube(new Vector2(x, y) + (Vector2)transform.position, Vector3.one * 0.2f);
                }
            }
        }
    }

    private void OnValidate()
    {
        if (initialized)
        {
            if (regenerateTerrain)
            {
                GenerateNoiseMap();
                regenerateTerrain = false;
            }
            UpdateMesh();
            }
    }
}
