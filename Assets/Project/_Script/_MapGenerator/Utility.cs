﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utility
{
    public static List<List<Cell>> FindAllGrounds(Cell[,] grid)
    {
        List<List<Cell>> result = new List<List<Cell>>();

        int n = grid.GetLength(0);
        int m = grid.GetLength(1);

        int[,] mark = new int[n, m];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                var value = BFS(ref mark, grid, CellType.Ground, n, m, i, j);
                if (value.Count > 0)
                {
                    result.Add(value);
                }
            }
        }

        return result;
    }

    public static List<List<Cell>> FindWatersInGround(Cell[,] grid, List<Cell> ground)
    {
        List<List<Cell>> result = new List<List<Cell>>();

        int minX = ground[0].Id.x;
        int minY = ground[0].Id.y;
        int maxX = ground[0].Id.x;
        int maxY = ground[0].Id.y;

        foreach (var cell in ground)
        {
            if (cell.Id.x < minX)
            {
                minX = cell.Id.x;
            }
            else if (cell.Id.x > maxX)
            {
                maxX = cell.Id.x;
            }

            if (cell.Id.y < minY)
            {
                minY = cell.Id.y;
            }
            else if (cell.Id.y > maxY)
            {
                maxY = cell.Id.y;
            }

        }

        //UnityEngine.Debug.Log($"minx = {minX}, maxX = {maxX}, miny = {minY}, maxy = {maxY}");

        int[,] mark = new int[maxX + 1, maxY + 1];
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                var value = BFSWithLimit(ref mark, grid, CellType.Water, maxX, maxY, i, j, minX, minY);
                if (value.Count > 1
                    && CheckIsPond(grid, value, minX, minY, maxX, maxY))
                {
                    value.Sort((e1, e2) => e1.Compare(e2));
                    result.Add(value);
                }
            }
        }

        return result;
    }

    public static List<Cell> BFS(ref int[,] mark, Cell[,] grid, CellType type, int n, int m, int i, int j)
    {
        List<Cell> result = new List<Cell>();

        if (mark[i, j] == 1 || grid[i, j].Type != type)
        {
            return result;
        }

        Queue<System.Tuple<int, int>> q = new Queue<System.Tuple<int, int>>();
        q.Enqueue(new System.Tuple<int, int>(i, j));
        mark[i, j] = 1;

        result.Add(grid[i, j]);

        while (q.Count != 0)
        {
            var element = q.Peek();
            int x = element.Item1, y = element.Item2;
            q.Dequeue();

            //top
            int p1 = x;
            int p2 = y - 1;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }

            //bot
            p1 = x;
            p2 = y + 1;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }

            //left
            p1 = x - 1;
            p2 = y;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }

            //right
            p1 = x + 1;
            p2 = y;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }
        }

        return result;
    }

    public static List<Cell> BFSWithLimit(ref int[,] mark, Cell[,] grid, CellType type, int n, int m, int i, int j, int minX, int minY)
    {
        List<Cell> result = new List<Cell>();

        if (mark[i, j] == 1 || grid[i, j].Type != type)
        {
            return result;
        }

        Queue<System.Tuple<int, int>> q = new Queue<System.Tuple<int, int>>();
        q.Enqueue(new System.Tuple<int, int>(i, j));
        mark[i, j] = 1;

        result.Add(grid[i, j]);

        while (q.Count != 0)
        {
            var element = q.Peek();
            int x = element.Item1, y = element.Item2;
            q.Dequeue();

            //top
            int p1 = x;
            int p2 = y - 1;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type
                && p1 >= minX && p2 >= minY)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }

            //bot
            p1 = x;
            p2 = y + 1;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type
                && p1 >= minX && p2 >= minY)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }

            //left
            p1 = x - 1;
            p2 = y;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type
                && p1 >= minX && p2 >= minY)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }

            //right
            p1 = x + 1;
            p2 = y;
            if (IsContain(n, m, p1, p2)
                && mark[p1, p2] != 1
                && grid[p1, p2].Type == type
                && p1 >= minX && p2 >= minY)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }
        }

        return result;
    }

    #region Finding shortest path

    public static List<Cell> BfsShortestPath(Cell[,] grid, Cell start, Cell end)
    {
        int cols = grid.GetLength(0);
        int rows = grid.GetLength(1);
        bool[,] visited = new bool[cols, rows];
        Queue<(Cell, List<Cell>)> queue = new Queue<(Cell, List<Cell>)>();
        visited[start.Id.x, start.Id.y] = true;
        queue.Enqueue((start, new List<Cell> { start }));

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();
            if (current == end)
            {
                return path;
            }

            int[][] directions = { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
            foreach (var direction in directions)
            {
                int nextRow = current.Id.x + direction[0];
                int nextCol = current.Id.y + direction[1];
                if (nextRow >= 0 && nextRow < rows && nextCol >= 0 && nextCol < cols &&
                    grid[nextRow, nextCol].Type == CellType.Ground && !visited[nextRow, nextCol])
                {
                    visited[nextRow, nextCol] = true;
                    var newPath = new List<Cell>(path) { grid[nextRow, nextCol] };
                    queue.Enqueue((grid[nextRow, nextCol], newPath));
                }
            }
        }

        return null;
    }

    static int Heuristic(Cell a, Cell b)
    {
        return (int)Math.Sqrt(Math.Pow(a.Id.x - b.Id.x, 2) + Math.Pow(a.Id.y - b.Id.y, 2));
    }

    static List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
    {
        var totalPath = new List<Cell> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    public static List<Cell> AStarSearch(Cell[,] matrix, Cell start, Cell end)
    {
        var openSet = new List<(int x, int y, int gScore, int fScore)> { (start.Id.x, start.Id.y, 0, Heuristic(start, end)) };
        var cameFrom = new Dictionary<Cell, Cell>();
        var gScore = new Dictionary<Cell, int> { [start] = 0 };

        while (openSet.Count > 0)
        {
            var tempCurrent = openSet.OrderBy(item => item.fScore).First();
            var current = matrix[tempCurrent.x, tempCurrent.y];

            if (current.Id.x == end.Id.x && current.Id.y == end.Id.y)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(tempCurrent);
            var neighbors = new List<(int x, int y)>
            {
                (current.Id.x - 1, current.Id.y),
                (current.Id.x + 1, current.Id.y),
                (current.Id.x, current.Id.y - 1),
                (current.Id.x, current.Id.y + 1),
                (current.Id.x - 1, current.Id.y - 1),
                (current.Id.x - 1, current.Id.y + 1),
                (current.Id.x + 1, current.Id.y - 1),
                (current.Id.x + 1, current.Id.y + 1)
                //(current.Id.x - 1, current.Id.y),
                //(current.Id.x + 1, current.Id.y),
                //(current.Id.x, current.Id.y - 1),
                //(current.Id.x, current.Id.y + 1)
            }.Where(n =>
                n.x >= 0 && n.x < matrix.GetLength(0) && n.y >= 0 && n.y < matrix.GetLength(1) &&
                matrix[n.x, n.y].Type == CellType.Ground
            );

            foreach (var neighbor in neighbors)
            {
                Cell cell = matrix[neighbor.x, neighbor.y];

                int tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(cell) 
                    || tentativeGScore < gScore[cell])
                {
                    cameFrom[cell] = current;
                    gScore[cell] = tentativeGScore;
                    int fScore = tentativeGScore + Heuristic(cell, end);
                    if (!openSet.Any(item => item.x == neighbor.x && item.y == neighbor.y))
                    {
                        openSet.Add((neighbor.x, neighbor.y, tentativeGScore, fScore));
                    }
                }
            }
        }
        return null; // No path found
    }
    #endregion
    public static Vector3 FindPointOnLine(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector3 result = Vector3.zero;

        var lineVector = end - start;
        var pointVector = point - start;

        var dot = Vector3.Dot(pointVector, lineVector);
        var lengthSqr = lineVector.sqrMagnitude;

        var param = Mathf.Clamp01(dot / lengthSqr);

        result = start + lineVector * param;

        return result;
    }

    public static bool IsGoodToPlace(Cell position, float radius, Cell[,] grid)
    {
        int x = position.Id.x;
        int y = position.Id.y;

        int n = grid.GetLength(0);
        int m = grid.GetLength(1);

        for (int i = 0; i < Mathf.Ceil(radius); i++)
        {
            // top
            if (y - i < 0 || grid[x, y - i].Type != CellType.Ground)
            {
                return false;
            }

            // bot
            if (y + i >= m || grid[x, y + i].Type != CellType.Ground)
            {
                return false;
            }

            // left
            if (x - i < 0 || grid[x - i, y].Type != CellType.Ground)
            {
                return false;
            }

            // right
            if (x + i >= n || grid[x + i, y].Type != CellType.Ground)
            {
                return false;
            }
        }

        return true;
    }

    public static Cell FindPointFarAway(List<Cell> list, float distance, Vector3 point)
    {
        foreach (var cell in list)
        {
            if (Vector3.Distance(cell.GetPosition(), point) > distance
                && cell.Type == CellType.Ground)
            {
                return cell;
            }
        }

        return null;
    }

    public static Cell FindPointClosetTo(List<Cell> list, float distance, Vector3 point)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(list[i].GetPosition(), point) > distance
                && list[i].Type == CellType.Ground)
            {
                return list[i];
            }
        }

        return null;
    }

    public static Cell FindClosetWater(Cell[,] grid, int x, int y)
    {
        if (grid[x - 1, y].Type == CellType.Ground)
        {
            for (int i = x - 1; i >= 0; i--)
            {
                if (grid[i, y].Type == CellType.Water)
                {
                    return grid[i, y];
                }
            }
        }
        else if (grid[x + 1, y].Type == CellType.Ground)
        {
            for (int i = x + 1; i < grid.GetLength(0); i++)
            {
                if (grid[i, y].Type == CellType.Water)
                {
                    return grid[i, y];
                }
            }
        }
        else if (grid[x, y - 1].Type == CellType.Ground)
        {
            for (int i = y - 1; i >= 0; i--)
            {
                if (grid[x, i].Type == CellType.Water)
                {
                    return grid[x, i];
                }
            }
        }
        else
        {
            for (int i = y + 1; i < grid.GetLength(1); i++)
            {
                if (grid[x, i].Type == CellType.Water)
                {
                    return grid[x, i];
                }
            }
        } 
            

        return null;
    }

    public static List<Cell> ClosestPond(Vector3 start, Vector3 end, List<List<Cell>> fallOffs)
    {
        Vector3 position = fallOffs[0][0].GetPosition();
        float minDistance = Vector3.Distance(position, 
                            FindPointOnLine(position, start, end));

        int min = 0;
        for (int i = 1; i < fallOffs.Count; i++)
        {
            position = fallOffs[i][0].GetPosition();
            float distance = Vector3.Distance(position, FindPointOnLine(position, start, end));
            if (distance < minDistance)
            {
                minDistance = distance;
                min = i;
            }
        }

        return fallOffs[min];
    }

    public static List<Cell> ClosestPondToPond(Vector3 start, Vector3 end, List<List<Cell>> fallOffs, List<Cell> pond)
    {
        if (fallOffs.Count < 2)
        {
            return null;
        }

        int result = -1;
        int index = fallOffs.FindIndex(e => e[0].Id == pond[0].Id);

        for (int i = 0; i < fallOffs.Count; i++)
        {
            if (index == i)
            {
                continue;
            }

            if (result == -1)
            {
                result = i;
                continue;
            }

            if (Vector3.Distance(fallOffs[index][0].GetPosition(), fallOffs[i][0].GetPosition()) 
                < Vector3.Distance(fallOffs[index][0].GetPosition(), fallOffs[result][0].GetPosition()))
            {
                result = i;
            }
        }

        return fallOffs[result];
    }

    public static bool CheckIsPond(Cell[,] grid, List<Cell> cells, int minX, int minY, int maxX, int maxY)
    {
        foreach (var cell in cells)
        {
            int i = cell.Id.x;
            int j = cell.Id.y;

            int count = 0;
            // check left
            for (int c = i; c >= minX; c--)
            {
                if (grid[c, j].Type == CellType.Ground)
                {
                    count++;
                    break;
                }
            }

            if (count == 0)
            {
                return false;
            }

            // check right
            for (int c = i; c <= maxX; c++)
            {
                if (grid[c, j].Type == CellType.Ground)
                {
                    count++;
                    break;
                }
            }

            if (count == 1)
            {
                return false;
            }

            // check top
            for (int c = j; c >= minY; c--)
            {
                if (grid[i, c].Type == CellType.Ground)
                {
                    count++;
                    break;
                }
            }

            if (count == 2)
            {
                return false;
            }

            // check bot
            for (int c = j; c <= maxY; c++)
            {
                if (grid[i, c].Type == CellType.Ground)
                {
                    count++;
                    break;
                }
            }

            if (count == 3)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsContain(int n, int m, int x, int y)
    {
        return x >= 0 && y >= 0 && x < n && y < m;
    }
}

public class PerlinNoise2D
{
    private static PerlinNoise2D _instance;

    public static PerlinNoise2D Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PerlinNoise2D();
            }

            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    private PerlinNoise2D()
    {
        for (int i = 0; i < 256; i++)
        {
            p[256 + i] = p[i] = permutation[i];
        }
    }

    public float Noise(float x, float y)
    {
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;
        int g1 = p[p[xi] + yi];
        int g2 = p[p[xi + 1] + yi];
        int g3 = p[p[xi] + yi + 1];
        int g4 = p[p[xi + 1] + yi + 1];

        float xf = x - (float)Math.Floor(x);
        float yf = y - (float)Math.Floor(y);

        float d1 = Grad(g1, xf, yf);
        float d2 = Grad(g2, xf - 1, yf);
        float d3 = Grad(g3, xf, yf - 1);
        float d4 = Grad(g4, xf - 1, yf - 1);

        float u = Fade(xf);
        float v = Fade(yf);

        float x1Inter = Lerp(u, d1, d2);
        float x2Inter = Lerp(u, d3, d4);
        float yInter = Lerp(v, x1Inter, x2Inter);

        return yInter;

    }

    private float Lerp(float amount, float left, float right)
    {
        return ((1 - amount) * left + amount * right);
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Grad(int hash, float x, float y)
    {
        switch (hash & 3)
        {
            case 0: return x + y;
            case 1: return -x + y;
            case 2: return x - y;
            case 3: return -x - y;
            default: return 0;
        }
    }

    int[] p = new int[512];
    int[] permutation = { 151,160,137,91,90,15,
               131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
               190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
               88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
               77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
               102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
               135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
               5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
               223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
               129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
               251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
               49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
               138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
               };

}
