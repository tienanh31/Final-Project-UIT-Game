using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField] Grid _mapGenerator;

    public SO_CharacterDefault PlayerBonusStat;
    public SO_CharacterDefault EnemyBonusStat;

    public GameConfig.CHARACTER SelectedCharacter = GameConfig.CHARACTER.CHARACTER_DEFAULT;

    public Grid MapGenerator
    {
        get => _mapGenerator;
        set
        {
            _mapGenerator = value;
        }
    }

    public static GameManager Instance { get; protected set; }

    private int _level = 1;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextLevel();
        }
    }

    public void BeginLevel(string levelname)
    {
        SceneManager.LoadScene(levelname);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPlaySceneWithLevel(int level)
    {
        _level = level;
        if (level <= 10)
        {
            EnemyBonusStat.HP_DEFAULT += 10;
        }
        else if (level <= 20)
        {
            EnemyBonusStat.HP_DEFAULT += 10;
            EnemyBonusStat.ATTACK_BONUS += 3;
        }
        else if (level <= 30)
        {
            EnemyBonusStat.HP_DEFAULT += 12;
            EnemyBonusStat.MOVE_SPEED_DEFAULT += 0.8f;
        }
        else
        {
            EnemyBonusStat.HP_DEFAULT += 12;
            EnemyBonusStat.ATTACK_BONUS += 5;
        }

        LoadScene(SceneName.PlayScene);
    }

    public void NextLevel()
    {
        LoadPlaySceneWithLevel(_level + 1);
    }

    public void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
        Debug.Log($"Current level is {id}");
    }

    public int TotalScene() => SceneManager.sceneCountInBuildSettings;

    public bool IsInPlayScene() => SceneManager.GetActiveScene().buildIndex != 0;

    public void QuitGame() => Application.Quit();

    public Enemy SpawningEnemy(GameConfig.ENEMY enemyType, Vector3 position)
    {
        Enemy enemy = Instantiate(Resources.Load<Enemy>("_Prefabs/Enemies/" + enemyType.ToString()),
            position, new Quaternion());

        return enemy;
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