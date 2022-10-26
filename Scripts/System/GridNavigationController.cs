using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridNavigationController : MonoBehaviour
{
    public Tilemap tilemap;
    public Vector3Int[,] gridVertexList;
    public Vector2Int start;

    private Astar astar;
    List<GridVertex> shortestPath = new List<GridVertex>();
    List<System.Tuple<int, int>> tileIndexList = new List<System.Tuple<int, int>>();
    new Camera camera;
    BoundsInt bounds;

    void Start()
    {
        tilemap.CompressBounds();
        bounds = tilemap.cellBounds;
        camera = Camera.main;

        CreateGrid();
        astar = new Astar(gridVertexList, bounds.size.x, bounds.size.y);
    }

    public void CreateGrid()
    {
        gridVertexList = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    gridVertexList[i, j] = new Vector3Int(x, y, 0);
                    tileIndexList.Add(new System.Tuple<int, int>(x, y));
                }
                else
                {
                    gridVertexList[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }

    public Vector3 FindRandomDestination()
    {
        int randomIndex = Random.Range(0, tileIndexList.Count);
        System.Tuple<int, int> index = tileIndexList[randomIndex];
        Vector3 position = tilemap.CellToWorld(new Vector3Int(index.Item1, index.Item2, 0));
        position.x += 0.5f;
        position.y += 0.5f;
        return position;
    }

    public Queue<Vector3> FindVertexQueue(Vector3 start, Vector3 end)
    {
        Queue<Vector3> vertexPositionQueue = new Queue<Vector3>();
        Vector3Int startGridCell = tilemap.WorldToCell(start);
        Vector3Int endGridCell = tilemap.WorldToCell(end);

        if (shortestPath != null && shortestPath.Count > 0)
            shortestPath.Clear();

        shortestPath = astar.CreatePath(gridVertexList, (Vector2Int)startGridCell, (Vector2Int)endGridCell, 1000);
        if (shortestPath == null)
            return vertexPositionQueue;
        for (int i = shortestPath.Count - 1; i >= 0; i--)
        {
            if (i == shortestPath.Count - 1 || i == shortestPath.Count - 2)
                continue;
            Vector3 position = tilemap.CellToWorld(new Vector3Int(shortestPath[i].X, shortestPath[i].Y, 0));
            vertexPositionQueue.Enqueue(new Vector3(position.x + 0.25f, position.y + 0.25f, 0));
        }
        return vertexPositionQueue;
    }
}
