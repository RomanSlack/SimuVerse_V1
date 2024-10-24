using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public float nodeSize;
    public LayerMask obstacleLayer; // Ensure this layer includes your walls

    private Node[,] grid;

    private void Start()
    {
        CreateGrid();
    }

    // This function is called automatically by Unity when the GameObject is selected
     private void OnDrawGizmos()
    {
        if (grid != null)
        {
            if (gridWidth > 0 && gridHeight > 0 && grid.Length == gridWidth * gridHeight)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    for (int y = 0; y < gridHeight; y++)
                    {
                        // Make the grid 80% transparent
                        Gizmos.color = grid[x, y].walkable ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
                        Gizmos.DrawCube(grid[x, y].worldPosition, Vector3.one * (nodeSize - 0.1f));
                    }
                }
            }
            else
            {
                Debug.LogWarning("Grid has not been properly initialized or grid dimensions are invalid.");
            }
        }
    }


    public void CreateGrid()
    {
        // Ensure gridWidth and gridHeight are positive numbers
        if (gridWidth <= 0 || gridHeight <= 0)
        {
            Debug.LogError("Grid width and height must be positive values.");
            return;
        }

        grid = new Node[gridWidth, gridHeight]; // Initialize the grid array
        Vector3 centerPosition = transform.position;

        // Calculate the bottom-left point of the grid based on the center
        Vector3 bottomLeft = centerPosition - Vector3.right * (gridWidth * nodeSize) / 2 - Vector3.up * (gridHeight * nodeSize) / 2;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeSize) + Vector3.up * (y * nodeSize);

                // Use Physics2D.OverlapCircle to check if this node collides with a wall/obstacle
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeSize / 2, obstacleLayer);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 gridOrigin = transform.position - Vector3.right * (gridWidth * nodeSize) / 2 - Vector3.up * (gridHeight * nodeSize) / 2;
        Vector3 relativePosition = worldPosition - gridOrigin;

        int x = Mathf.Clamp(Mathf.RoundToInt(relativePosition.x / nodeSize), 0, gridWidth - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(relativePosition.y / nodeSize), 0, gridHeight - 1);

        return grid[x, y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the node itself

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
}

// This is the Node class. It should be placed outside the GridManager class
public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
