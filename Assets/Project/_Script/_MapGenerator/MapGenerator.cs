using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

public class MapGenerator
{
    int _numberOfCorners = 10;

    float _treeNoiseScale = 0.5f;

    float _grassNoiseScale = 0.5f;

    float _waterLevel = 0.5f;
    float _scale = 0.1f;
    int _size = 100;

    Cell[,] _grid;
    List<List<Cell>> _grounds;
    private List<List<Cell>> _fallOffs;

    private Vector3 _playerPosition;
    private Vector3 _gatePosition;

    private Cell _start;
    private Cell _end;

    private List<EnemyData> _enemies;
    private List<TrapData> _traps;

    private float MaxNoiseValue = 0f;

    private List<PatrolScope> _patrolScopes;

    private int _mapType = 0;

    public MapGenerator()
    {

    }

    public MapGenerator(int size, int type, Dictionary<GameConfig.ENEMY, int> enemies)
    {
        _size = size;
        _mapType = type;
    }

    public MapData GenerateMap(int size, int type, Dictionary<GameConfig.ENEMY, int> enemies)
    {
        _size = size;
        _mapType = type;

        float[,] noiseMap = new float[_size, _size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * _scale + xOffset, y * _scale + yOffset) * 2f;
                noiseMap[x, y] = noiseValue;
                if (noiseValue > MaxNoiseValue)
                {
                    MaxNoiseValue = noiseValue;
                }
            }
        }

        float[,] falloffMap = new float[_size, _size];
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                float xv = x / (float)_size * 2 - 1;
                float yv = y / (float)_size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(1.5f - 1.5f * v, 3f));//Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        _grid = new Cell[_size, _size];
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                bool isWater = noiseValue < _waterLevel;
                CellType cellType = CellType.Ground;
                if (isWater)
                {
                    cellType = CellType.Water;
                }

                Cell cell = new Cell(cellType, noiseValue, new Vector2Int(x, y));
                _grid[x, y] = cell;
            }
        }

        CalculateStartAndEndPoint();

        GenerateEnemies(enemies);

        var mapData = new MapData();
        mapData.PlayerPosition = _playerPosition;
        mapData.GatePosition = _gatePosition;
        mapData.EnemieDatas = _enemies;
        mapData.TrapDatas = _traps;
        mapData.ArrayNoiseValue = _grid;
        if (_grounds.Count > 0)
        {
            mapData.LargestArea = _grounds[_grounds.Count - 1];
        }

        return mapData;
    }

    void CalculateStartAndEndPoint()
    {
        _grounds = Utility.FindAllGrounds(_grid);
        _grounds.Sort((e1, e2) => e1.Count.CompareTo(e2.Count));

        foreach (var element in _grounds)
        {
            element.Sort((e1, e2) => e1.Compare(e2));

            string debug = $"Size {element.Count}: ";
            foreach (var i in element)
            {
                debug += i.Id + "\t";
            }

            //Debug.Log(debug);
        }

        int size = _grounds.Count;
        if (size > 0)
        {
            var largestArea = _grounds[size - 1];
            var n = largestArea.Count;

            var startPointCell = largestArea[Random.Range(0, n / 10)];
            while (startPointCell.IsContainTree)
            {
                startPointCell = largestArea[Random.Range(0, n / 10)];
            }

            _start = startPointCell;
            _playerPosition = startPointCell.GetPosition();
            _playerPosition.y = MaxNoiseValue;

            var endPointCell = largestArea[Random.Range(n - n / 10, n)];
            while (endPointCell.IsContainTree)
            {
                endPointCell = largestArea[Random.Range(n - n / 10, n)];
            }
            _end = endPointCell;
            _gatePosition = endPointCell.GetPosition();

            _fallOffs = Utility.FindWatersInGround(_grid, _grounds[size - 1]);
            //Debug.Log(Falloffs.Count);
            foreach (var w in _fallOffs)
            {
                string debug = "";
                foreach (var c in w)
                {
                    debug += c.Id + " || ";
                }

                //Debug.Log(debug);
            }
        }
    }

    void GenerateEnemies(Dictionary<GameConfig.ENEMY, int> enemies)
    {
        _enemies = new List<EnemyData>();
        _patrolScopes = new List<PatrolScope>();

        var largestArea = _grounds[_grounds.Count - 1];
        int startRandom = largestArea.Count / 5;
        //int endRandom = largestArea.Count - largestArea.Count / 10;

        for (int i = 2; i <= 5; i++)
        {
            var patrolScope = new PatrolScope();

            Vector2 previous = Vector2.zero;
            while (patrolScope.Corners.Count < _numberOfCorners)
            {
                var randomCell = largestArea[Random.Range(startRandom * (i - 1), startRandom * i)];

                if (previous == Vector2.zero
                    || Vector2.Distance(randomCell.GetPosition(), previous) > 3)
                {
                    if (!randomCell.IsContainTree
                        && !randomCell.IsBorder(_grid))
                    {
                        patrolScope.Corners.Add(randomCell.GetPosition());
                        previous = randomCell.GetPosition();
                    }
                }
            }

            patrolScope.Initialize();
            _patrolScopes.Add(patrolScope);
        }

        
        int total = enemies.Sum(e => e.Value);
        //Debug.Log("Total enemy: " + total);

        foreach (var enemyType in enemies)
        {
            if (enemyType.Key == GameConfig.ENEMY.TRAP)
            {
                GenerateTrap(enemyType.Value);
                continue;
            }

            int value = enemyType.Value;
            int size = _patrolScopes.Count - 1;
            while (value > 0)
            {
                EnemyData enemy = new EnemyData();
                enemy.Type = (int)enemyType.Key;
                enemy.PatrolScope = _patrolScopes[size];
                enemy.Position = _patrolScopes[size].Corners[Random.Range(0, _patrolScopes[size].Corners.Count)];

                _enemies.Add(enemy);

                value--;
                size--;
                if (size < 0)
                {
                    size = _patrolScopes.Count - 1;
                }
            }
        }

    }

    private void GenerateTrap(int total)
    {
        var shortest = Utility.BfsShortestPath(_grid, _start, _end);
        string debug = "";
        foreach(var cell in shortest)
        {
            debug += cell.Id + "\n";
        }

        Debug.Log(debug);

        _traps = new List<TrapData>();

        var falloffs = _fallOffs;

        var largestArea = _grounds[_grounds.Count - 1];

        int size = largestArea.Count;

        switch (_mapType)
        {
            case 0:
            case 1:
                {
                    if (total >= 1)
                    {
                        Vector3 position = Vector3.zero;

                        var cell = Utility.FindPointFarAway(shortest, 10, _playerPosition);
                        if (cell != null)
                        {
                            int currentIndex = shortest.FindIndex(e => e.Id == cell.Id);
                            while (!Utility.IsGoodToPlace(cell, 4.5f, _grid))
                            {
                                currentIndex++;
                                if (currentIndex < shortest.Count)
                                {
                                    cell = shortest[currentIndex];
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (!Utility.IsGoodToPlace(cell, 4.5f, _grid))
                            {
                                Debug.Log("Not match condition");
                            }

                            position = cell.GetPosition();
                        }

                        TrapData trap = new TrapData();
                        trap.StartPosition = position;
                        trap.Name = typeof(Mud).Name;

                        _traps.Add(trap);
                    }

                    int old = 0;
                    if (total >= 2)
                    {
                        var falloff = Utility.ClosestPond(_playerPosition, _gatePosition, falloffs);
                        old = falloffs.FindIndex(e => e[0].Id == falloff[0].Id);

                        foreach (var cell in falloff)
                        {
                            TrapData trap = new TrapData();
                            trap.StartPosition = cell.GetPosition();
                            trap.Name = typeof(Pit).Name;

                            _traps.Add(trap);
                        }

                    }
                    if (total >= 3)
                    {
                        if (falloffs.Count > 1)
                        {
                            var falloff = Utility.ClosestPondToPond(_playerPosition,
                                _gatePosition, falloffs, falloffs[old]);
                            foreach (var c in falloff)
                            {
                                TrapData trap = new TrapData();
                                trap.StartPosition = c.GetPosition();
                                trap.Name = typeof(Pit).Name;

                                _traps.Add(trap);
                            }
                        }


                        Vector3 position = Vector3.zero;

                        var cell = Utility.FindPointClosetTo(shortest, 10, _gatePosition);
                        if (cell != null)
                        {
                            position = cell.GetPosition();
                        }

                        Debug.Log("Hammer pos: " + position);
                        _traps.Add(new TrapData()
                        {
                            StartPosition = position,
                            Name = typeof(Hammer).Name
                        });
                    }
                }
                break;

            case 2:
                {
                    if (total >= 1)
                    {
                        Vector3 position = Vector3.zero;

                        var cell = Utility.FindPointFarAway(shortest, 10, _playerPosition);
                        if (cell != null)
                        {
                            position = cell.GetPosition();
                        }

                        _traps.Add(new TrapData()
                        {
                            StartPosition = position,
                            Name = typeof(IceBoom).Name
                        });
                    }

                    if (total >= 2)
                    {
                        if (falloffs.Count > 1)
                        {
                            var falloff = Utility.ClosestPond(_playerPosition, _gatePosition, falloffs);
                            int old = falloffs.FindIndex(e => e[0].Id == falloff[0].Id);

                            var nextFalloff = Utility.ClosestPondToPond(_playerPosition,
                                    _gatePosition, falloffs, falloffs[old]);

                            Vector3 start = falloff[0].GetPosition();
                            Vector3 end = nextFalloff[0].GetPosition();

                            Debug.Log($"Start: {start}, End: {end}");
                            _traps.Add(new TrapData()
                            {
                                StartPosition = start,
                                EndPosition = end,
                                Name = typeof(Iceberg).Name
                            });
                        }
                        else if (falloffs.Count == 1)
                        {
                            var falloff = Utility.ClosestPond(_playerPosition, _gatePosition, falloffs);

                            Vector3 start = falloff[0].GetPosition();

                            Vector3 end = Utility.FindClosetWater(_grid, falloff[0].Id.x, falloff[0].Id.y).GetPosition();

                            Debug.Log($"Start: {start}, End: {end}");
                            _traps.Add(new TrapData()
                            {
                                StartPosition = start,
                                EndPosition = end,
                                Name = typeof(Iceberg).Name
                            });
                        }
                        else
                        {
                            Cell cell = shortest[shortest.Count / 3];

                            Cell cellStart = Utility.FindClosetWater(_grid, cell.Id.x, cell.Id.y);
                            Vector3 start = cellStart.GetPosition();

                            Vector3 end = Utility.FindClosetWater(_grid, cellStart.Id.x, cellStart.Id.y).GetPosition();

                            Debug.Log($"Start: {start}, End: {end}");
                            _traps.Add(new TrapData()
                            {
                                StartPosition = start,
                                EndPosition = end,
                                Name = typeof(Iceberg).Name
                            });
                        }
                    }
                    if (total >= 3)
                    {
                        Vector3 position = Vector3.zero;

                        var cell = Utility.FindPointClosetTo(shortest, 10, _gatePosition);
                        if (cell != null)
                        {
                            position = cell.GetPosition();
                        }

                        _traps.Add(new TrapData()
                        {
                            StartPosition = position,
                            Name = typeof(IceBoom).Name
                        });

                        _traps.Add(new TrapData()
                        {
                            StartPosition = Vector3.zero,
                            EndPosition = Vector3.zero,
                            Name = typeof(IceRain).Name
                        });
                    }
                }
                break;

            case 3:
                {
                    if (total >= 1)
                    {
                        Vector3 position = Vector3.zero;

                        var cell = Utility.FindPointFarAway(shortest, 10, _playerPosition);
                        if (cell != null)
                        {
                            position = cell.GetPosition();
                        }

                        _traps.Add(new TrapData()
                        {
                            StartPosition = position,
                            Name = typeof(FlameBoom).Name
                        });
                    }

                    if (total >= 2)
                    {
                        if (falloffs.Count > 0)
                        {
                            var falloff = Utility.ClosestPond(_playerPosition, _gatePosition, falloffs);

                            Vector3 start = falloff[0].GetPosition();
                            Vector3 end = start;

                            int x = falloff[0].Id.x;
                            int y = falloff[0].Id.y;
                            if (_grid[x - 1, y].Type == CellType.Ground)
                            {
                                end = _grid[x - 1, y].GetPosition();
                            }
                            else if (_grid[x + 1, y].Type == CellType.Ground)
                            {
                                end = _grid[x + 1, y].GetPosition();
                            }
                            else if (_grid[x, y - 1].Type == CellType.Ground)
                            {
                                end = _grid[x, y - 1].GetPosition();
                            }
                            else
                            {
                                end = _grid[x, y + 1].GetPosition();
                            }

                            _traps.Add(new TrapData()
                            {
                                StartPosition = start,
                                EndPosition = end,
                                Name = typeof(FlameThrower).Name
                            });
                        }
                    }
                    if (total >= 3)
                    {
                        Vector3 position = Vector3.zero;

                        var cell = Utility.FindPointClosetTo(shortest, 10, _gatePosition);
                        if (cell != null)
                        {
                            position = cell.GetPosition();
                        }

                        _traps.Add(new TrapData()
                        {
                            StartPosition = position,
                            Name = typeof(FlameBoom).Name
                        });

                        var endPoint = _gatePosition;

                        float minDistance = Vector3.Distance(falloffs[0][0].GetPosition(), endPoint);

                        cell = Utility.FindPointClosetTo(shortest, 20, endPoint);

                        position = cell.GetPosition();

                        _traps.Add(new TrapData()
                        {
                            StartPosition = position,
                            Name = typeof(FireCarpet).Name
                        });
                    }
                }
                break;
        }

        #region Old Version
        //var falloffs = _fallOffs;

        //var largestArea = _grounds[_grounds.Count - 1];

        //int size = largestArea.Count;

        //switch (_mapType)
        //{
        //    case 0:
        //    case 1:
        //        {
        //            if (total >= 1)
        //            {
        //                Vector3 position = largestArea[UnityEngine.Random.Range(size / 5, size / 7)].GetPosition();

        //                TrapData trap = new TrapData();
        //                trap.StartPosition = position;
        //                trap.Name = typeof(Mud).Name;

        //                _traps.Add(trap);
        //            }

        //            int random = UnityEngine.Random.Range(0, falloffs.Count);
        //            if (total >= 2)
        //            {
        //                var falloff = falloffs[random];
        //                foreach (var cell in falloff)
        //                {
        //                    TrapData trap = new TrapData();
        //                    trap.StartPosition = cell.GetPosition();
        //                    trap.Name = typeof(Pit).Name;

        //                    _traps.Add(trap);
        //                }

        //            }
        //            if (total >= 3)
        //            {
        //                if (falloffs.Count > 1)
        //                {
        //                    int newRandom = UnityEngine.Random.Range(0, falloffs.Count);
        //                    while (newRandom == random)
        //                    {
        //                        newRandom = UnityEngine.Random.Range(0, falloffs.Count);
        //                    }

        //                    var falloff = falloffs[newRandom];
        //                    foreach (var cell in falloff)
        //                    {
        //                        TrapData trap = new TrapData();
        //                        trap.StartPosition = cell.GetPosition();
        //                        trap.Name = typeof(Pit).Name;

        //                        _traps.Add(trap);
        //                    }
        //                }

        //                Vector3 position = largestArea[UnityEngine.Random.Range(size / 4, size / 5)].GetPosition();

        //                _traps.Add(new TrapData()
        //                {
        //                    StartPosition = position,
        //                    Name = typeof(Hammer).Name
        //                });
        //            }
        //        }
        //        break;

        //    case 2:
        //        {
        //            if (total >= 1)
        //            {
        //                Vector3 position = largestArea[UnityEngine.Random.Range(size / 5, size / 6)].GetPosition();

        //                _traps.Add(new TrapData()
        //                {
        //                    StartPosition = position,
        //                    Name = typeof(IceBoom).Name
        //                });
        //            }

        //            if (total >= 2)
        //            {
        //                if (falloffs.Count > 1)
        //                {
        //                    Vector3 start = falloffs[0][falloffs[0].Count - 1].GetPosition();
        //                    Vector3 end = falloffs[1][0].GetPosition();

        //                    _traps.Add(new TrapData()
        //                    {
        //                        StartPosition = start,
        //                        EndPosition = end,
        //                        Name = typeof(Iceberg).Name
        //                    });
        //                }
        //            }
        //            if (total >= 3)
        //            {
        //                if (falloffs.Count > 2)
        //                {
        //                    Vector3 start = falloffs[1][falloffs[1].Count - 1].GetPosition();
        //                    Vector3 end = falloffs[2][0].GetPosition();

        //                    _traps.Add(new TrapData()
        //                    {
        //                        StartPosition = start,
        //                        EndPosition = end,
        //                        Name = typeof(Iceberg).Name
        //                    });
        //                }

        //                _traps.Add(new TrapData()
        //                {
        //                    StartPosition = Vector3.zero,
        //                    EndPosition = Vector3.zero,
        //                    Name = typeof(IceRain).Name
        //                });
        //            }
        //        }
        //        break;

        //    case 3:
        //        {
        //            if (total >= 1)
        //            {
        //                Vector3 position = largestArea[UnityEngine.Random.Range(size / 5, size / 6)].GetPosition();

        //                _traps.Add(new TrapData()
        //                {
        //                    StartPosition = position,
        //                    Name = typeof(FlameBoom).Name
        //                });
        //            }

        //            if (total >= 2)
        //            {
        //                if (falloffs.Count > 0)
        //                {
        //                    Vector3 start = falloffs[0][0].GetPosition();
        //                    Vector3 end = falloffs[0][1].GetPosition();
        //                    if (end.x > start.x)
        //                    {
        //                        end.x = start.x - 1;
        //                    }
        //                    else if (end.x < start.x)
        //                    {
        //                        end.x = start.x + 1;
        //                    }
        //                    else
        //                    {
        //                        if (end.z > start.z)
        //                        {
        //                            end.z = start.z - 1;
        //                        }
        //                        else
        //                        {
        //                            end.z = start.z + 1;
        //                        }
        //                    }

        //                    _traps.Add(new TrapData()
        //                    {
        //                        StartPosition = start,
        //                        EndPosition = end,
        //                        Name = typeof(FlameThrower).Name
        //                    });
        //                }
        //            }
        //            if (total >= 3)
        //            {
        //                if (falloffs.Count > 1)
        //                {
        //                    Vector3 start = falloffs[1][0].GetPosition();
        //                    Vector3 end = falloffs[1][1].GetPosition();
        //                    if (end.x > start.x)
        //                    {
        //                        end.x = start.x - 1;
        //                    }
        //                    else if (end.x < start.x)
        //                    {
        //                        end.x = start.x + 1;
        //                    }
        //                    else
        //                    {
        //                        if (end.z > start.z)
        //                        {
        //                            end.z = start.z - 1;
        //                        }
        //                        else
        //                        {
        //                            end.z = start.z + 1;
        //                        }
        //                    }

        //                    _traps.Add(new TrapData()
        //                    {
        //                        StartPosition = start,
        //                        EndPosition = end,
        //                        Name = typeof(FlameThrower).Name
        //                    });
        //                }

        //                var endPoint = _gatePosition;

        //                float minDistance = Vector3.Distance(falloffs[0][0].GetPosition(), endPoint);
        //                List<Cell> closetFalloff = falloffs[0];
        //                foreach (var falloff in falloffs)
        //                {
        //                    float distance = Vector3.Distance(falloff[0].GetPosition(), endPoint);
        //                    if (distance < minDistance)
        //                    {
        //                        closetFalloff = falloff;
        //                        minDistance = distance;
        //                    }
        //                }

        //                Vector3 position = (endPoint + closetFalloff[0].GetPosition()) / 2f;

        //                _traps.Add(new TrapData()
        //                {
        //                    StartPosition = position,
        //                    Name = typeof(FireCarpet).Name
        //                });
        //            }
        //        }
        //        break;
        //}
        #endregion
    }

}
