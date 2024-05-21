using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField] Grid _mapGenerator;

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

    public void BeginLevel(string levelname)
    {
        SceneManager.LoadScene(levelname);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

    #endregion
}

public class SceneName
{
    public const string MainMenu = "Main Menu";
    public const string PlayScene = "PlayScene";
}