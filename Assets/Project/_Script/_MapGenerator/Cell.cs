using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public CellType Type;

    public bool IsContainTree = false;

    public float NoiseValue => _noiseValue;

    private float _noiseValue = 0f;

    public Cell(CellType type, float noiseValue)
    {
        Type = type;
        this._noiseValue = noiseValue;
    }

}

public enum CellType
{
    Ground,
    Water,
}