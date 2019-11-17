using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquareData {

    //  2----3----4 
    //  |         | 
    //  1         5 
    //  |         | 
    //  0----7----6 
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

    //  1---------2 
    //  |         | 
    //  |         |
    //  |         | 
    //  0---------3
    public static readonly int[,] triangulations = new int[16, 12]  // Vertex: 0123
    {                                                               // Binary: 1248
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},           //    00 = 0000
        {0, 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1},              //    01 = 1000
        {2, 3, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1},              //    02 = 0100
        {0, 2, 3, 3, 7, 0, -1, -1, -1, -1, -1, -1},                 //    03 = 1100
        {4, 5, 3, -1, -1, -1, -1, -1, -1, -1 , -1, -1},             //    04 = 0010
        {0, 1, 7, 4, 5, 3, -1, -1, -1, -1, -1, -1},                 //    05 = 1010
        {2, 4, 1, 4, 5, 1, -1, -1, -1, -1, -1, -1},                 //    06 = 0110
        {0, 2, 4, 0, 4, 7, 4, 5, 7, -1, -1, -1},                    //    07 = 1110
        {6, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1},              //    08 = 0001
        {6, 0, 5, 0, 1, 5, -1, -1, -1, -1, -1, -1},                 //    09 = 1001
        {2, 3, 1, 6, 7, 5, -1, -1, -1, -1, -1, -1},                 //    10 = 0101
        {0, 2, 6, 2, 3, 6, 6, 3, 5, -1, -1, -1},                    //    11 = 1101
        {4, 6, 3, 6, 7, 3, -1, -1, -1, -1, -1, -1},                 //    12 = 0011
        {0, 4, 6, 0, 1, 4, 4, 1, 3, -1, -1, -1},                    //    13 = 1011
        {2, 4, 6, 2, 6, 7, 2, 7, 1, -1, -1, -1},                    //    14 = 0111
        {0, 2, 4, 0, 4, 6, -1, -1, -1, -1, -1, -1},                 //    15 = 1111
    };
}

public class Square {
    Vector3 position;
}

public class Voxels : MonoBehaviour {
    
}
