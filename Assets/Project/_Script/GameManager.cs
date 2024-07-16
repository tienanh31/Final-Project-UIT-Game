using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField] Grid _grid;
    [SerializeField] NavMeshSurface _navMesh;

    public NavMeshSurface NavMesh => _navMesh;
    public VictoryScreen.BuffStat PlayerBonusStat;
    public VictoryScreen.BuffStat EnemiesBonusStat;

    public GameConfig.CHARACTER SelectedCharacter = GameConfig.CHARACTER.CHARACTER_DEFAULT;

    public Grid Grid
    {
        get => _grid;
        set
        {
            _grid = value;
        }
    }

    public static GameManager Instance { get; protected set; }

    private int _level = 40;
    private List<VictoryScreen.BuffStat> _buffStats;
    private MyXMLReader _myXmlData;

    private MapGenerator _mapGenerator;
    #endregion

    #region Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
        	Destroy(gameObject);
        }
        DontDestroyOnLoad(Instance);

        PlayerBonusStat = new VictoryScreen.BuffStat()
        {
            HP = 0,
            MOVE_SPEED = 0,
            ATTACK_BONUS = 0
        };

        EnemiesBonusStat = new VictoryScreen.BuffStat()
        {
            HP = 0,
            MOVE_SPEED = 0,
            ATTACK_BONUS = 0,
        };

        _buffStats = new List<VictoryScreen.BuffStat>();
        _buffStats.Add(
            new VictoryScreen.BuffStat()
            {
                HP = 8,
                MOVE_SPEED = 0.9f,
                ATTACK_BONUS = 5f
            });
        _buffStats.Add(
            new VictoryScreen.BuffStat()
            {
                HP = 10,
                MOVE_SPEED = 1f,
                ATTACK_BONUS = 5.5f
            });
        _buffStats.Add(
            new VictoryScreen.BuffStat()
            {
                HP = 12,
                MOVE_SPEED = 1.2f,
                ATTACK_BONUS = 6f
            });
        _buffStats.Add(
            new VictoryScreen.BuffStat()
            {
                HP = 14,
                MOVE_SPEED = 1.5f,
                ATTACK_BONUS = 6.5f
            });

        _myXmlData = new MyXMLReader();
        _myXmlData.ReadFile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_level < 41)
            {
                LevelManager.Instance.Win();
                //NextLevel();
            }
        }
    }

    public void InitMapGenerator()
    {
        _mapGenerator = new MapGenerator();
        var mapData = _mapGenerator.GenerateMap(100, GetMapType(), GetEnemiesCurrentLevel());

        _grid.ReadMapData(mapData);

        LevelManager.Instance.Initialize(mapData.PlayerPosition, mapData.GatePosition);
        LevelManager.Instance.SpawningEnemies(mapData.EnemieDatas);
        LevelManager.Instance.SpawningTraps(mapData.TrapDatas, mapData.LargestArea);

        Debug.Log("Level: " + _level);
    }

    public void BeginLevel(string levelname)
    {
        SceneManager.LoadScene(levelname);
    }

    public void ContinueGame()
    {
        DataPersistenceManager.Instance.LoadData();
        GameData data = DataPersistenceManager.Instance.GameData;

        _level = data.Level;
        PlayerBonusStat = data.PlayerBuffStat;
        EnemiesBonusStat = data.EnemiesBuffStat;

        LoadCurrentLevel();
    }

    public void AddBuffStat(int type)
    {
        switch(type)
        {
            case 0:
                PlayerBonusStat.HP += _buffStats[GetMapType()].HP;
                break;
            case 1:
                PlayerBonusStat.MOVE_SPEED += _buffStats[GetMapType()].MOVE_SPEED;
                break;
            case 2:
                PlayerBonusStat.ATTACK_BONUS += _buffStats[GetMapType()].ATTACK_BONUS;
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void LoadCurrentLevel()
    {
        LoadScene(SceneName.PlayScene);
        UIManager.Instance.ResumeGame();
    }

    public void LoadPlaySceneWithLevel(int level)
    {
        _level = level;
        if (level <= 10)
        {
            EnemiesBonusStat.HP += 10;
        }
        else if (level <= 20)
        {
            EnemiesBonusStat.HP += 10;
            EnemiesBonusStat.ATTACK_BONUS += 3;
        }
        else if (level <= 30)
        {
            EnemiesBonusStat.HP += 12;
            EnemiesBonusStat.MOVE_SPEED += 0.8f;
        }
        else
        {
            EnemiesBonusStat.HP += 12;
            EnemiesBonusStat.ATTACK_BONUS += 5;
        }

        LoadScene(SceneName.PlayScene);
    }

    public void NextLevel()
    {
        if (_level == 40)
        {
            EndingScreen.Create();
            return;
        }

        LoadPlaySceneWithLevel(_level + 1);
    }

    public void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
        Debug.Log($"Current level is {id}");
    }

    public int TotalScene() => SceneManager.sceneCountInBuildSettings;

    public bool IsInPlayScene() => SceneManager.GetActiveScene().buildIndex != 0;

    public void SaveData()
    {
        DataPersistenceManager.Instance.GameData.SetData(_level, PlayerBonusStat, EnemiesBonusStat);
        DataPersistenceManager.Instance.SaveData();
    }

    public void QuitGame()
    {
        SaveData();

        Application.Quit();
    }

    public Enemy SpawningEnemy(GameConfig.ENEMY enemyType, Vector3 position)
    {
        Enemy enemy = Instantiate(Resources.Load<Enemy>("_Prefabs/Enemies/" + enemyType.ToString()),
            position, new Quaternion());

        return enemy;
    }

    public Trap SpawingTrap(string type, Vector3 position)
    {
        Trap trap = Instantiate(Resources.Load<Trap>("_Prefabs/Trap/" + type),
            position, new Quaternion());
        return trap;
    }

    public Dictionary<GameConfig.ENEMY, int> GetEnemiesCurrentLevel()
    {
        return _myXmlData.GetEnemiesAtLevel(_level);
    }

    public int GetMapType()
    {
        if (_level <= 10)
        {
            return 0;
        }

        if (_level <= 20)
        {
            return 1;
        }

        if (_level <= 30)
        {
            return 2;
        }

        return 3;
    }

    #endregion
}

public class SceneName
{
    public const string MainMenu = "Main Menu";
    public const string PlayScene = "PlayScene";
}