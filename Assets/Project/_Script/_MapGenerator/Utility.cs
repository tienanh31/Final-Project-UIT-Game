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
                var value = BFS(ref mark, grid, n, m, i, j);
                if (value.Count > 0)
                {
                    result.Add(value);
                }
            }
        }

        return result;
    }

    public static List<Cell> BFS(ref int[,] mark, Cell[,] grid, int n, int m, int i, int j)
    {
        List<Cell> result = new List<Cell>();

        if (mark[i, j] == 1 || grid[i, j].Type == CellType.Water)
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
                && grid[p1, p2].Type == CellType.Ground)
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
                && grid[p1, p2].Type == CellType.Ground)
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
                && grid[p1, p2].Type == CellType.Ground)
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
                && grid[p1, p2].Type == CellType.Ground)
            {
                mark[p1, p2] = 1;
                q.Enqueue(new System.Tuple<int, int>(p1, p2));
                result.Add(grid[p1, p2]);
            }
        }

        return result;
    }

    public static bool IsContain(int n, int m, int x, int y)
    {
        return x >= 0 && y >= 0 && x < n && y < m;
    }
}