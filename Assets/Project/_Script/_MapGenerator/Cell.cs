using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public CellType Type;

    public bool IsContainTree = false;

    public bool IsContainEnemy = false;

    public float NoiseValue => _noiseValue;

    public Vector2Int Id => _id;

    private float _noiseValue = 0f;

    private Vector2Int _id;

    public Vector3 GetPosition() => new Vector3(_id.x, 0.6f, _id.y);

    public Cell(CellType type, float noiseValue, Vector2Int id)
    {
        Type = type;
        this._noiseValue = noiseValue;
        this._id = id;
    }

    public bool IsNearby(Cell cell)
    {
        int sqrt = (_id.x + _id.y) - (cell.Id.x + cell.Id.y);

        if (sqrt < 0)
        {
            sqrt = -sqrt;
        }

        return sqrt == 1;
    }

    public override bool Equals(object obj)
    {
        Cell cell = obj as Cell;

        if (cell == null)
        {
            return false;
        }

        return cell._id == this._id;
    }

    public int Compare(Cell cell)
    {
        int total1 = _id.x + _id.y;
        int total2 = cell.Id.x + cell.Id.y;

        if (total1 < total2)
        {
            return -1;
        }
        else if (total1 == total2)
        {
            return 0;
        }

        return 1;
    }

    public float Distance(Cell cell)
    {
        return Vector2.Distance(_id, cell._id);
    }

    public bool IsBorder(Cell[,] grid)
    {
        //top
        int x = _id.x;
        int y = _id.y - 1;
        if (y < 0 || grid[x, y].Type == CellType.Water)
        {
            return true;
        }

        // bot
        x = _id.x;
        y = _id.y + 1;
        if (y >= grid.GetLength(1) || grid[x, y].Type == CellType.Water)
        {
            return true;
        }

        // left
        x = _id.x - 1;
        y = _id.y;
        if (x < 0 || grid[x, y].Type == CellType.Water)
        {
            return true;
        }

        // right
        x = _id.x + 1;
        y = _id.y;
        if (x >= grid.GetLength(0) || grid[x, y].Type == CellType.Water)
        {
            return true;
        }

        return false;
    }
}

public enum CellType
{
    Ground,
    Water,
}