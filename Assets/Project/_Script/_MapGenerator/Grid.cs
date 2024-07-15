using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;

public class Grid : MonoBehaviour
{
    [SerializeField] NavMeshSurface _navMesh;
    [SerializeField] Button _btnRandom;

    [SerializeField] GameObject[] _treePrefabs;

    [SerializeField] GameObject[] _grassPrefabs;

    [SerializeField] List<Material> _terrainMaterials;
    [SerializeField] Material _edgeMaterial;


    private List<Mesh> _meshes;
    private List<GameObject> _gameObjects;
    private List<Texture2D> _texture2Ds;

    private int _mapType = -1;

    public void SetTexture(int mapType)
    {
        if (mapType < _terrainMaterials.Count)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = _terrainMaterials[mapType];
        }
    }

    void Awake()
    {
        _meshes = new List<Mesh>();
        _gameObjects = new List<GameObject>();
        _texture2Ds = new List<Texture2D>();

        GameManager.Instance.Grid = this;
        GameManager.Instance.InitMapGenerator();
    }

    private void OnEnable()
    {
       // _btnRandom.onClick.AddListener(ReadMapData);
    }

    private void OnDisable()
    {
       // _btnRandom.onClick.RemoveListener(ReadMapData);
    }

    public void ReadMapData(MapData mapData)
    {
        ClearAllMap();


        DrawTerrainMesh(mapData.ArrayNoiseValue);
        DrawEdgeMesh(mapData.ArrayNoiseValue);
        DrawTexture(mapData.ArrayNoiseValue);
        //GenerateTrees(mapData.ArrayNoiseValue);
        //GenerateGrasses(mapData.ArrayNoiseValue);
    }

    
    void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        //List<Vector2> uvs = new List<Vector2>();

        var size = grid.GetLength(0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.Type == CellType.Ground)
                {
                    Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                    Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 d = new Vector3(x + .5f, 0, y - .5f);

                    //float height = Mathf.PerlinNoise((x - .5f) * .3f, (y + .5f) * .3f) * .9f;
                    //Vector3 a = new Vector3(x - .5f, height, y + .5f);
                    //height = Mathf.PerlinNoise((x + .5f) * .3f, (y + .5f) * .3f) * .9f;
                    //Vector3 b = new Vector3(x + .5f, height, y + .5f);
                    //height = Mathf.PerlinNoise((x - .5f) * .3f, (y - .5f) * .3f) * .9f;
                    //Vector3 c = new Vector3(x - .5f, height, y - .5f);
                    //height = Mathf.PerlinNoise((x + .5f) * .3f, (y - .5f) * .3f) * .9f;
                    //Vector3 d = new Vector3(x + .5f, height, y - .5f);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };

                    //Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                    //Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                    //Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                    //Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    //Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };

                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                        //uvs.Add(uv[k]);
                    }

                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        var collider = gameObject.AddComponent<MeshCollider>();

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (_navMesh)
        {
            _navMesh.BuildNavMesh();
        }

        _meshes.Add(mesh);
    }

    void DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        var size = grid.GetLength(0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
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
                    if (x < size - 1)
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
                    if (y < size - 1)
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
        //Texture2D texture = new Texture2D(size, size);
        //Color[] colorMap = new Color[size * size];
        //for (int y = 0; y < size; y++)
        //{
        //    for (int x = 0; x < size; x++)
        //    {
        //        Cell cell = grid[x, y];
        //        if (cell.Type == CellType.Water)
        //            colorMap[y * size + x] = Color.blue;
        //        else
        //            colorMap[y * size + x] = Color.green;
        //    }
        //}
        //texture.filterMode = FilterMode.Point;
        //texture.SetPixels(colorMap);
        //texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (GameManager.Instance != null)
        {
            meshRenderer.material = _terrainMaterials[GameManager.Instance.GetMapType()];
        }
        else
        {
            meshRenderer.material = _terrainMaterials[_mapType];
        }
        //meshRenderer.material.mainTexture = texture;

        //_texture2Ds.Add(texture);
    }

    // new version
    //void GenerateTrees(Cell[,] grid)
    //{
    //    var size = grid.GetLength(0);

    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            Cell cell = grid[x, y];
    //            if (cell.Type == CellType.Ground)
    //            {
    //                float v = Random.Range(0f, _treeNoiseScale);
    //                if (grid[x, y].NoiseValue < v 
    //                    && grid[x, y] != EndPointCell
    //                    && grid[x, y] != StartPointCell)
    //                {
    //                    GameObject prefab = _treePrefabs[Random.Range(0, _treePrefabs.Length)];
    //                    GameObject tree = Instantiate(prefab, transform);

    //                    tree.transform.position = new Vector3(x, 0, y);
    //                    tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
    //                    tree.transform.localScale = Vector3.one * Random.Range(.1f, .3f);

    //                    _gameObjects.Add(tree);

    //                    cell.IsContainTree = true;
    //                }
    //            }
    //        }
    //    }
    //}

    //void GenerateGrasses(Cell[,] grid)
    //{
    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            Cell cell = grid[x, y];
    //            if (cell.Type == CellType.Ground)
    //            {
    //                float v = Random.Range(0f, _grassNoiseScale);
    //                if (grid[x, y].NoiseValue < v)
    //                {
    //                    GameObject prefab = _grassPrefabs[Random.Range(0, _grassPrefabs.Length)];
    //                    GameObject grass = Instantiate(prefab, transform);

    //                    grass.transform.position = new Vector3(x, 0, y);
    //                    grass.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
    //                    grass.transform.localScale = Vector3.one * Random.Range(.1f, .3f);

    //                    _gameObjects.Add(grass);
    //                }
    //            }
    //        }
    //    }
    //}

    void ClearAllMap()
    {
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

    //void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying) return;
    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            Cell cell = mapData.ArrayNoiseValue[x, y];
    //            if (cell.Type == CellType.Water)
    //                Gizmos.color = Color.blue;
    //            else
    //                Gizmos.color = Color.green;
    //            Vector3 pos = new Vector3(x, 0, y);
    //            Gizmos.DrawCube(pos, Vector3.one);
    //        }
    //    }

    //    var largestArea = _grounds[_grounds.Count - 1];
    //    // start areas
    //    for (int i = 0; i < largestArea.Count / 10; i++)
    //    {
    //        Gizmos.color = Color.white;
    //        Vector3 pos = new Vector3(largestArea[i].Id.x, 0, largestArea[i].Id.y);
    //        Gizmos.DrawCube(pos, Vector3.one);
    //    }

    //    // end areas
    //    for (int i = largestArea.Count - 1; i > (largestArea.Count - largestArea.Count / 10); i--)
    //    {
    //        Gizmos.color = Color.black;
    //        Vector3 pos = new Vector3(largestArea[i].Id.x, 0, largestArea[i].Id.y);
    //        Gizmos.DrawCube(pos, Vector3.one);
    //    }
    //}
}