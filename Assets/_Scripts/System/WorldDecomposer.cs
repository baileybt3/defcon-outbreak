using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDecomposer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int terrainWidth = 80;
    [SerializeField] private int terrainLength = 80;
    [SerializeField] private int nodeSize = 1;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastHeight = 20f;
    [SerializeField] private float raycastDistance = 40f;
    [SerializeField] private LayerMask blockingLayers = ~0; //everything by default

    private int[,] worldData;
    private int rows;
    private int cols;

    private void Start()
    {

        rows = terrainWidth / nodeSize;
        cols = terrainLength / nodeSize;

        worldData = new int[rows, cols];

        DecomposeWorld();
    }


    public void DecomposeWorld()
    {
        //Center the grid on center Emtpy gameObject
        Vector3 center = transform.position;

        float startX = center.x - terrainWidth / 2f;
        float startZ = center.z - terrainLength / 2f;

        float nodeCenterOffset = nodeSize / 2f;


        for (int row = 0; row < rows; row++)
        {

            for (int col = 0; col < cols; col++)
            {

                float x = startX + nodeCenterOffset + (nodeSize * col);
                float z = startZ + nodeCenterOffset + (nodeSize * row);

                Vector3 startPos = new Vector3(x, 20f, z);


                // Does our raycast hit anything at this point in the map
                RaycastHit hit;


                // Does the ray intersect any objects
                if (Physics.Raycast(startPos, Vector3.down, out hit, raycastDistance, blockingLayers))
                {

                    Debug.DrawRay(startPos, Vector3.down * raycastDistance, Color.red, 50000);
                    worldData[row, col] = 1;

                }
                else
                {
                    Debug.DrawRay(startPos, Vector3.down * raycastDistance, Color.green, 50000);
                    worldData[row, col] = 0;
                }
            }
        }

    }

    public int[,] GetWorldData()
    {
        return worldData;
    }

    public int Rows => rows;
    public int Cols => cols;
    public int NodeSize => nodeSize;
}
