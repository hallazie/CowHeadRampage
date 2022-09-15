using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Astar
{
    public GridVertex[,] vertexList;
    public Astar(Vector3Int[,] grid, int columns, int rows)
    {
        vertexList = new GridVertex[columns, rows];
    }
    private bool IsValidPath(Vector3Int[,] grid, GridVertex start, GridVertex end)
    {
        if (end == null)
            return false;
        if (start == null)
            return false;
        if (end.Height >= 1)
            return false;
        return true;
    }
    public List<GridVertex> CreatePath(Vector3Int[,] grid, Vector2Int start, Vector2Int end, int length)
    {

        GridVertex End = null;
        GridVertex Start = null;
        var columns = vertexList.GetUpperBound(0) + 1;
        var rows = vertexList.GetUpperBound(1) + 1;
        vertexList = new GridVertex[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                vertexList[i, j] = new GridVertex(grid[i, j].x, grid[i, j].y, grid[i, j].z);
            }
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                vertexList[i, j].AddNeighboors(vertexList, i, j);
                if (vertexList[i, j].X == start.x && vertexList[i, j].Y == start.y)
                    Start = vertexList[i, j];
                else if (vertexList[i, j].X == end.x && vertexList[i, j].Y == end.y)
                    End = vertexList[i, j];
            }
        }
        if (!IsValidPath(grid, Start, End))
            return null;
        List<GridVertex> OpenSet = new List<GridVertex>();
        List<GridVertex> ClosedSet = new List<GridVertex>();

        OpenSet.Add(Start);

        while (OpenSet.Count > 0)
        {
            //Find shortest step distance in the direction of your goal within the open set
            int winner = 0;
            for (int i = 0; i < OpenSet.Count; i++)
                if (OpenSet[i].F < OpenSet[winner].F)
                    winner = i;
                else if (OpenSet[i].F == OpenSet[winner].F)//tie breaking for faster routing
                    if (OpenSet[i].H < OpenSet[winner].H)
                        winner = i;

            var current = OpenSet[winner];

            //Found the path, creates and returns the path
            if (End != null && OpenSet[winner] == End)
            {
                List<GridVertex> Path = new List<GridVertex>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != null)
                {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                if (length - (Path.Count - 1) < 0)
                {
                    Path.RemoveRange(0, (Path.Count - 1) - length);
                }
                return Path;
            }

            OpenSet.Remove(current);
            ClosedSet.Add(current);

            //Finds the next closest step on the grid
            var neighboors = current.Neighboors;
            for (int i = 0; i < neighboors.Count; i++)//look threw our current spots neighboors (current spot is the shortest F distance in openSet
            {
                var n = neighboors[i];
                if (!ClosedSet.Contains(n) && n.Height < 1)//Checks to make sure the neighboor of our current tile is not within closed set, and has a height of less than 1
                {
                    var tempG = current.G + 1;//gets a temp comparison integer for seeing if a route is shorter than our current path

                    bool newPath = false;
                    if (OpenSet.Contains(n)) //Checks if the neighboor we are checking is within the openset
                    {
                        if (tempG < n.G)//The distance to the end goal from this neighboor is shorter so we need a new path
                        {
                            n.G = tempG;
                            newPath = true;
                        }
                    }
                    else//if its not in openSet or closed set, then it IS a new path and we should add it too openset
                    {
                        n.G = tempG;
                        newPath = true;
                        OpenSet.Add(n);
                    }
                    if (newPath)//if it is a newPath caclulate the H and F and set current to the neighboors previous
                    {
                        n.H = Heuristic(n, End);
                        n.F = n.G + n.H;
                        n.previous = current;
                    }
                }
            }

        }
        return null;
    }

    private int Heuristic(GridVertex a, GridVertex b, string method = "euclidean")
    {
        if (method == "manhattan")
        {
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            return 1 * (dx + dy);
        }
        else
        {
            // euclidean by default
            return Mathf.CeilToInt(Mathf.Pow(a.X - b.X, 2) + Mathf.Pow(a.Y - b.Y, 2));
        }

    }
}
public class GridVertex
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public int Height = 0;
    public List<GridVertex> Neighboors;
    public GridVertex previous = null;

    public GridVertex(int x, int y, int height)
    {
        X = x;
        Y = y;
        F = 0;
        G = 0;
        H = 0;
        Neighboors = new List<GridVertex>();
        Height = height;
    }

    public void AddNeighboors(GridVertex[,] grid, int x, int y)
    {
        // up + left + right + down
        if (x < grid.GetUpperBound(0))
            Neighboors.Add(grid[x + 1, y]);
        if (x > 0)
            Neighboors.Add(grid[x - 1, y]);
        if (y < grid.GetUpperBound(1))
            Neighboors.Add(grid[x, y + 1]);
        if (y > 0)
            Neighboors.Add(grid[x, y - 1]);

        // 45 degrees
        if (x < grid.GetUpperBound(0) && y < grid.GetUpperBound(1))
            Neighboors.Add(grid[x + 1, y + 1]);
        if (x > 0 && y > 0)
            Neighboors.Add(grid[x - 1, y - 1]);
        if (x < grid.GetUpperBound(0) && y > 0)
            Neighboors.Add(grid[x + 1, y - 1]);
        if (x > 0 && y < grid.GetUpperBound(1))
            Neighboors.Add(grid[x - 1, y + 1]);
    }
}