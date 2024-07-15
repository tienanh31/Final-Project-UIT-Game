using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapData
{
    public Cell[,] ArrayNoiseValue;
    public List<Cell> LargestArea;

    public Vector3 PlayerPosition;
    public Vector3 GatePosition;

    public List<EnemyData> EnemieDatas;
    public List<TrapData> TrapDatas;
}
