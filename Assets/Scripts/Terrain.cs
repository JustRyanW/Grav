using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrian : MonoBehaviour
{

    [Header("Settings")]
    public Vector2Int chunkSize = new Vector2Int(8, 8);
    public int renderDistance = 2;
    public float surfaceValue = 0.5f;
    public bool smoothed = true;
    public Material material;

    [Header("Private")]
    List<Chunk> chunks = new List<Chunk>();
    List<Chunk> updateQueue = new List<Chunk>();

    private void Start() {
        
    }

    private void Update() {
        
    }


}

public class Chunk
{
    float[,] voxelMap;
    Vector3[,,] vertexMap;
    int[,,] indexMap;

    public List<Vector3> verts = new List<Vector3>();
    public List<int> indices = new List<int>();

    Mesh mesh;
    MeshFilter meshFilter;

    // methods
}
