using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public Vector2Int chunkSize = new Vector2Int(16, 16);
    public float surfaceValue = 0;
    [Space]
    public bool smoothed;

    [SerializeField]
    float[,] voxelMap;
    Vector2[,,] edgeMap;
    public List<Vector3> verts = new List<Vector3>();
    public List<int> indices = new List<int>();

    Mesh mesh;
    MeshFilter meshFilter;

    public bool drawDebug = true;

    public float speed = 0.2f;
    public Vector2 offset = Vector2.zero;
    public float scale = 5f;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        GenerateVoxelMap();
        ClearMesh();
        MarchSquares();
        CreateMesh();
    }

    private void Update()
    {
        //noiseScale = (Mathf.Sin(Time.time * speed) - offset) * scale;

        GenerateVoxelMap();
        ClearMesh();
        MarchSquares();
        CreateMesh();
    }

    void MarchSquares()
    {
        for (int x = 0; x < chunkSize.x - 1; x++)
        {
            for (int y = 0; y < chunkSize.y - 1; y++)
            {
                MarchSquare(x, y);
            }
        }
    }

    void MarchSquare(int x, int y)
    {
        int triangulationIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            int xOffset = SquareData.vertices[i].x;
            int yOffset = SquareData.vertices[i].y;

            if (voxelMap[x + xOffset, y + yOffset] >= surfaceValue)
                triangulationIndex |= 1 << i;   
        }
        // Debug.Log("X:" + x + " Y:" + y + " Triang:" + triangulationIndex + " Surf:" + voxelMap[x, y]);

        if (triangulationIndex == 0)
            return;

        int vertIndex = 0;
        for (int tri = 0; tri < 4; tri++)
        {
            for (int vert = 0; vert < 3; vert++)
            {
                int index = SquareData.triangulations[triangulationIndex, vertIndex];
                if (index == -1)
                    return;

                Vector2Int pos = new Vector2Int(x, y);

                if (index < 4)
                {
                    verts.Add((Vector2)pos + SquareData.vertices[index]);
                }
                else
                {
                    index = index - 4;
                    Vector2Int a = pos + SquareData.edges[index, 0];
                    Vector2Int b = pos + SquareData.edges[index, 1];

                    float ia = voxelMap[a.x, a.y];
                    float ib = voxelMap[b.x, b.y];

                    float c = Mathf.InverseLerp(ia, ib, surfaceValue); 

                    verts.Add(Vector2.Lerp(a, b, c));
                }
                   

                indices.Add(verts.Count - 1);

                vertIndex++;
            }
        }
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

    void GenerateVoxelMap()
    {
        voxelMap = new float[chunkSize.x, chunkSize.y];

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                //voxelMap[x, y] = Mathf.PerlinNoise(0.2f + (float)x / chunkSize.x * noiseScale, 0.4f + (float)y / chunkSize.y * noiseScale) - 0.5f;

                voxelMap[x, y] = Perlin3D(x / scale + offset.x, y / scale + offset.y, Time.time * speed) - 0.5f;
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

    private void OnDrawGizmos()
    {
        if (drawDebug && Application.isPlaying)
        {
            for (int x = 0; x < chunkSize.x; x++)
            {
                for (int y = 0; y < chunkSize.y; y++)
                {
                    if (voxelMap[x, y] >= surfaceValue)
                        Gizmos.DrawSphere(new Vector2(x, y), 0.1f);
                }
            }
        }
    }
}
