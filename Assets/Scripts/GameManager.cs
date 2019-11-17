using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    Rigidbody2D[] attractors;

    [Range(0, 5)] public float gravityDisplayScale;
    public bool drawGrid;

    private void Start()
    {
        FindAttractors();        
    }

    private void Update()
    {
        if (drawGrid) DrawGrid();
    }

    private void FindAttractors()
    {
        GameObject[] attractorGameObjects = GameObject.FindGameObjectsWithTag("Attractor");
        attractors = new Rigidbody2D[attractorGameObjects.Length];
        for (int i = 0; i < attractors.Length; i++)
        {
            attractors[i] = attractorGameObjects[i].GetComponent<Rigidbody2D>();
        }
        Calculations.attractors = attractors;
    }

    public void DrawGrid()
    {
        Vector2[,] warpedPositions = new Vector2[71, 45];
        for (int x = -35; x < 36; x++)
        {
            for (int y = -22; y < 23; y++)
            {
                Vector2 position = new Vector2(x, y) / 2;
                warpedPositions[x + 35, y + 22] = Calculations.Gravity(position, gravityDisplayScale, true) + position;
            }
        }
        for (int x = -34; x < 35; x++)
        {
            for (int y = -21; y < 22; y++)
            {
                Vector2 position = warpedPositions[x + 35, y + 22];
                Vector2[] relativePosition = new Vector2[4];
                relativePosition[0] = warpedPositions[x + 35, y + 23] - warpedPositions[x + 35, y + 22];
                relativePosition[1] = warpedPositions[x + 36, y + 22] - warpedPositions[x + 35, y + 22];
                relativePosition[2] = warpedPositions[x + 35, y + 21] - warpedPositions[x + 35, y + 22];
                relativePosition[3] = warpedPositions[x + 34, y + 22] - warpedPositions[x + 35, y + 22];

                for (int i = 0; i < relativePosition.Length; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(position, relativePosition[i], relativePosition[i].magnitude);
                    if (hit.collider != null) Debug.DrawLine(position, hit.point);
                    else if (i < 2) Debug.DrawRay(position, relativePosition[i]);
                }
            }
        }
    }
}
