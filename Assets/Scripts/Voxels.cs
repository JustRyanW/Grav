using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquareData {

    //  2----3----4 
    //  |         | 
    //  1         5 
    //  |         | 
    //  0----7----6 
    public static readonly Vector2[] vertices = new Vector2[8]
    {
        new Vector2(0, 0),
        new Vector2(0, 0.5f),
        new Vector2(0, 1),
        new Vector2(0.5f, 1),
        new Vector2(1, 1),
        new Vector2(1, 0.5f),
        new Vector2(1, 0),
        new Vector2(0.5f, 0)
    };

    public static readonly int[,] triangulations = new int[16, 12]
    {
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 2, 3, 3, 7, 0, -1, -1, -1, -1, -1, -1},
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
        { }
    };

}

public class Square {

   
    Vector3 position;
}

public class Voxels : MonoBehaviour {
    
}
