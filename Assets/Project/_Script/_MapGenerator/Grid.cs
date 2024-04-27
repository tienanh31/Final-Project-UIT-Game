using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [SerializeField] Button btnRandom;

    [SerializeField] GameObject[] _enemyPrefabs;
    [SerializeField] float _enemyNoiseScale = .005f;
    [SerializeField] float _enemyDensity = .5f;

    [SerializeField] GameObject[] _treePrefabs;
    [SerializeField] float _treeNoiseScale = .005f;
    [SerializeField] float _treeDensity = .5f;

    [SerializeField] GameObject[] _grassPrefabs;
    [SerializeField] float _grassNoiseScale = .01f;
    [SerializeField] float _grassDensity = .5f;

    [SerializeField] Material _terrainMaterial;
    [SerializeField] Material _edgeMaterial;
    [SerializeField] float _waterLevel = .4f;
    [SerializeField] float _scale = .1f;
    [SerializeField] int _size = 100;

    [SerializeField] GameObject _startPoint;
    [SerializeField] GameObject _endPoint;

    Cell[,] _grid;
    List<List<Cell>> _grounds;

    Cell _startPointCell;
    Cell _endPointCell;
    List<Cell> _enemiesPosition;

    private List<Mesh> _meshes;
    private List<GameObject> _gameObjects;
    private List<Texture2D> _texture2Ds;

    void Start()
    {
        _meshes = new List<Mesh>();
        _gameObjects = new List<GameObject>();
        _texture2Ds = new List<Texture2D>();

        RandomMap();
    }

    private void OnEnable()
    {
        btnRandom.onClick.AddListener(RandomMap);
    }

    private void OnDisable()
    {
        btnRandom.onClick.RemoveListener(RandomMap);
    }

    void RandomMap()
    {
        ClearAllMap();

        float[,] noiseMap = new float[_size, _size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * _scale + xOffset, y * _scale + yOffset);
                noiseMap[x, y] = noiseValue;
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
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
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
                CellType type = CellType.Ground;
                if (isWater)
                {
                    type = CellType.Water;
                }

                Cell cell = new Cell(type, noiseValue, new Vector2Int(x, y));
                _grid[x, y] = cell;
            }
        }

        CalculateStartAndEndPoint();

        DrawTerrainMesh(_grid);
        DrawEdgeMesh(_grid);
        DrawTexture(_grid);
        GenerateTrees(_grid);
        GenerateGrasses(_grid);
        GenerateEnemies(_grid);
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

            Debug.Log(debug);
        }

        int size = _grounds.Count;
        if (size > 0)
        {
            var largestArea = _grounds[size - 1];
            var n = largestArea.Count;

            _startPointCell = largestArea[Random.Range(0, n / 10)];
            while (_startPointCell.IsContainTree)
            {
                _startPointCell = largestArea[Random.Range(0, n / 10)];
            }

            _endPointCell = largestArea[Random.Range(n - n / 10, n)];
            while (_endPointCell.IsContainTree)
            {
                _endPointCell = largestArea[Random.Range(n - n / 10, n)];
            }

            _startPoint.transform.position = new Vector3(_startPointCell.Id.x, 0, _startPointCell.Id.y);
            _endPoint.transform.position = new Vector3(_endPointCell.Id.x, 0, _endPointCell.Id.y);
        }
    }

    void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.Type == CellType.Ground)
                {
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                    Vector2 uvA = new Vector2(x / (float)_size, y / (float)_size);
                    Vector2 uvB = new Vector2((x + 1) / (float)_size, y / (float)_size);
                    Vector2 uvC = new Vector2(x / (float)_size, (y + 1) / (float)_size);
                    Vector2 uvD = new Vector2((x + 1) / (float)_size, (y + 1) / (float)_size);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[k]);
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        _meshes.Add(mesh);
    }

    void DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.Type == CellType.Ground)
                {
                    if (x > 0)
                    {
                        Cell left = grid[x - 1, y];
                        if (left.Type == CellType.Water)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (x < _size - 1)
                    {
                        Cell right = grid[x + 1, y];
                        if (right.Type == CellType.Water)
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (y > 0)
                    {
                        Cell down = grid[x, y - 1];
                        if (down.Type == CellType.Water)
                        {
                            Vector3 a = new Vector3(x - .5f, 0, y - .5f);
                            Vector3 b = new Vector3(x + .5f, 0, y - .5f);
                            Vector3 c = new Vector3(x - .5f, -1, y - .5f);
                            Vector3 d = new Vector3(x + .5f, -1, y - .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                    if (y < _size - 1)
                    {
                        Cell up = grid[x, y + 1];
                        if (up.Type == CellType.Water)
                        {
                            Vector3 a = new Vector3(x + .5f, 0, y + .5f);
                            Vector3 b = new Vector3(x - .5f, 0, y + .5f);
                            Vector3 c = new Vector3(x + .5f, -1, y + .5f);
                            Vector3 d = new Vector3(x - .5f, -1, y + .5f);
                            Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                            for (int k = 0; k < 6; k++)
                            {
                                vertices.Add(v[k]);
                                triangles.Add(triangles.Count);
                            }
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = _edgeMaterial;

        _gameObjects.Add(edgeObj);
        _meshes.Add(mesh);
    }

    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(_size, _size);
        Color[] colorMap = new Color[_size * _size];
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.Type == CellType.Water)
                    colorMap[y * _size + x] = Color.blue;
                else
                    colorMap[y * _size + x] = Color.green;
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = _terrainMaterial;
        meshRenderer.material.mainTexture = texture;

        _texture2Ds.Add(texture);
    }

    void GenerateEnemies(Cell[,] grid)
    {
        int numEnemies = 5;
        _enemiesPosition = new List<Cell>();

        //// Get enemy at Endpoint
        //int endX = Mathf.RoundToInt(_endPoint.transform.position.x);
        //int endY = Mathf.RoundToInt(_endPoint.transform.position.z);
        //// Create enemy at Endpoint
        GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, transform);
        enemy.transform.position = new Vector3(_endPointCell.Id.x, 0f, _endPointCell.Id.y);
        _gameObjects.Add(enemy);

        //random enemies'pos in map
        _enemiesPosition.Add(_endPointCell);

        var largestArea = _grounds[_grounds.Count - 1];
        int startRandom = largestArea.Count / 10;
        int endRandom = largestArea.Count - largestArea.Count / 10;

        while (_enemiesPosition.Count < numEnemies)
        {
            var randomCell = largestArea[Random.Range(startRandom, endRandom)];

            if (_enemiesPosition.FindIndex(e => e.Equals(randomCell)) == -1)
            {
                if (!randomCell.IsContainTree)
                {
                    _enemiesPosition.Add(randomCell);
                }
            }
        }

        // Create random enemies in map 
        foreach (var cell in _enemiesPosition)
        {
            GameObject randomEnemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            GameObject randomEnemy = Instantiate(randomEnemyPrefab, transform);
            randomEnemy.transform.position = new Vector3(cell.Id.x, 0f, cell.Id.y);

            _gameObjects.Add(randomEnemy);

            Debug.LogWarning(cell.Id);
        }
    }

    void GenerateTrees(Cell[,] grid)
    {
        //float[,] noiseMap = new float[_size, _size];
        //(float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        //for (int y = 0; y < _size; y++)
        //{
        //    for (int x = 0; x < _size; x++)
        //    {
        //        float noiseValue = Mathf.PerlinNoise(x * _treeNoiseScale + xOffset, y * _treeNoiseScale + yOffset);
        //        noiseMap[x, y] = noiseValue;
        //    }
        //}

        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.Type == CellType.Ground)
                {
                    float v = Random.Range(0f, _treeDensity);
                    if (grid[x, y].NoiseValue < v)
                    {
                        GameObject prefab = _treePrefabs[Random.Range(0, _treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);

                        tree.transform.position = new Vector3(x, 0, y);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(.1f, .3f);

                        _gameObjects.Add(tree);

                        cell.IsContainTree = true;
                    }
                }
            }
        }
    }

    void GenerateGrasses(Cell[,] grid)
    {
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.Type == CellType.Ground)
                {
                    float v = Random.Range(0f, _grassDensity);
                    if (grid[x, y].NoiseValue < v)
                    {
                        GameObject prefab = _grassPrefabs[Random.Range(0, _grassPrefabs.Length)];
                        GameObject grass = Instantiate(prefab, transform);

                        grass.transform.position = new Vector3(x, 0, y);
                        grass.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        grass.transform.localScale = Vector3.one * Random.Range(.1f, .3f);

                        _gameObjects.Add(grass);
                    }
                }
            }
        }
    }

    void ClearAllMap()
    {
        if (_enemiesPosition != null)
        {
            _enemiesPosition.Clear();
        }    

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            DestroyImmediate(meshFilter);
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null)
        {
            DestroyImmediate(meshRenderer);
        }

        for (int i = _meshes.Count - 1; i >= 0; i--)
        {
            if(_meshes[i] != null)
            {
                DestroyImmediate(_meshes[i]);
            }
            _meshes.RemoveAt(i);
        }

        for (int i = _gameObjects.Count - 1; i >= 0; i--)
        {
            if (_gameObjects[i] != null)
            {
                DestroyImmediate(_gameObjects[i].gameObject);
            }
            _gameObjects.RemoveAt(i);
        }

        for (int i = _texture2Ds.Count - 1; i >= 0; i--)
        {
            if (_texture2Ds[i] != null)
            {
                DestroyImmediate(_texture2Ds[i]);
            }
            _texture2Ds.RemoveAt(i);
        }

    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                Cell cell = _grid[x, y];
                if (cell.Type == CellType.Water)
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.green;
                Vector3 pos = new Vector3(x, 0, y);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }

        var largestArea = _grounds[_grounds.Count - 1];
        // start areas
        for (int i = 0; i < largestArea.Count / 10; i++)
        {
            Gizmos.color = Color.white;
            Vector3 pos = new Vector3(largestArea[i].Id.x, 0, largestArea[i].Id.y);
            Gizmos.DrawCube(pos, Vector3.one);
        }

        // end areas
        for (int i = largestArea.Count - 1; i > (largestArea.Count - largestArea.Count / 10); i--)
        {
            Gizmos.color = Color.black;
            Vector3 pos = new Vector3(largestArea[i].Id.x, 0, largestArea[i].Id.y);
            Gizmos.DrawCube(pos, Vector3.one);
        }
    }
}