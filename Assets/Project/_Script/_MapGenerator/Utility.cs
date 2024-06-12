using System.Collections;
using System.Collections.Generic;

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