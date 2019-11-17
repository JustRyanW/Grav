using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap : MonoBehaviour {

    public Vector2Int voxelMapSize;
    public float surface;
    [Space]
    public bool autoRefresh;
    public bool smoothed;

    Vertex[,] vertices;
    public List<Vector3> meshVerts = new List<Vector3>();
    public List<int> indices = new List<int>();

    Mesh mesh;

    public class Vertex
    {
        public Vector2 position;
        public float value;
        public int index;

        public Vertex(Vector2 position)
        {
            this.position = position;
        }
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMap();
        GenerateMesh();
    }

    void Update()
    {

    }

    void GenerateMap()
    {
        foreach (Vertex vertex in vertices)
        {
            vertex.value = -Vector2.Distance(Vector2.zero, vertex.position);
        }
    }

    void GenerateMesh()
    {
        vertices = new Vertex[voxelMapSize.x + 1, voxelMapSize.y + 1];

        int index = 0;
        for (int x = 0; x < voxelMapSize.x; x++)
        {
            for (int y = 0; y < voxelMapSize.y; y++)
            {
                vertices[x, y].index = index;
                meshVerts.Add(vertices[x, y].position);
                index++;
               // MarchCube(vertices[x, y]);
            }
        }
        mesh.vertices = meshVerts.ToArray();
        mesh.triangles = indices.ToArray();
    }

    void Clear()
    {

    }
/*
    void MarchCube(Vertex vertex)
    {
        int triangulationIndex = 0;
        for (int i = 0; i < 4; i++)
            if (vertices[vertOffset[i,0].x, vertOffset[i,0].y].value >= surface)
                triangulationIndex |= 1 << i;     

        if (triangulationIndex == 0 || triangulationIndex == 15)
            return;

        int vertIndex = 0;
        for (int tri = 0; tri < 4; tri++)
        {
            for (int vert = 0; vert < 3; vert++)
            {
                int index = triangulationTable[triangulationIndex, vertIndex];
                if (index == -1)
                    return;
                vertOffset[]
                    //fuckin code some shitt get the lerp between the edge retard
                indices.Add(vertices[,].index);
                vertIndex++;
            }
        }
    }

    static readonly Vector2Int[,] vertOffset = new Vector2Int[4, 2]
    {
        { new Vector2Int(0, 0), new Vector2Int(1, 0)},
        { new Vector2Int(1, 0), new Vector2Int(1, 1)},
        { new Vector2Int(1, 1), new Vector2Int(0, 1)},
        { new Vector2Int(0, 1), new Vector2Int(0, 0)}
    };

    static readonly int[,] triangulationTable = new int[,]
    {
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        { },
        { },
        { },
        { },
        { },
        { },
        { },
        { },
        { },
        { },
        { },
        { },
        { },
    } 
    */
}
