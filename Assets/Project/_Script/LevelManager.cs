using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    Sweep,
    Survival,
    SearchAndDestroy
}

public class LevelManager : MonoBehaviour
{
    #region Fields & Properties
    public GameMode currentGameMode;
    public GameMode[] availableGameMode = { GameMode.Sweep, GameMode.Survival };
    public GameObject characterSpawner;
    public AudioClip BGM;
    public AudioClip WinSFX;
    public AudioClip LoseSFX;
    public bool GamePaused = false;

    //to activate the gamelost event only once
    protected bool GameLost = false;
    protected bool GameWon = false;
    protected AudioSource audioSource;

    // [SerializeField]
    // protected PatrolScope _patrolScope;

    private static LevelManager instance;
    public static LevelManager Instance
    {
        get => instance;
        private set => instance = value;
    }

    //public CharacterSO characterInfo;
    [SerializeField] protected Character character;
    [SerializeField] public List<Enemy> enemies;
    [SerializeField] CameraController myCamera;
    public List<IDamageable> damageables;

    protected float possibleEnemyCount, enemiesLeft;
    #endregion

    #region Methods
    private void OnDrawGizmos()
    {
        if (characterSpawner == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(characterSpawner.transform.position, 0.5f);
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        damageables = new List<IDamageable>();
        audioSource = this.GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        character = GameObject.FindObjectOfType<Character>();
        if (character == null)
            switch (GameManager.Instance.SelectedCharacter)
            {
                case GameConfig.CHARACTER.CHARACTER_1:
                    character = Character1.Create(null, characterSpawner.transform.position);
                    break;

                case GameConfig.CHARACTER.CHARACTER_2:
                    character = Character2.Create(null, characterSpawner.transform.position);
                    break;

                case GameConfig.CHARACTER.CHARACTER_3:
                    character = Character3.Create(null, characterSpawner.transform.position);
                    break;

                case GameConfig.CHARACTER.CHARACTER_4:
                    character = Character4.Create(null, characterSpawner.transform.position);
                    break;

                default:
                    character = Character.Create(null, characterSpawner.transform.position);
                    break;
            }
        character.Initialize();
        myCamera = Camera.main.gameObject.GetComponent<CameraController>();
        myCamera.Initialize();
        foreach (var enemy in enemies)
        {
            enemy.Initialize();
        }

        possibleEnemyCount = enemies.Count;
        foreach (EnemySpawner es in GameObject.FindObjectsOfType<EnemySpawner>())
        {
            possibleEnemyCount += es.enemySpawnLimit;
        }
        foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
        {
            if (!enemy._initialized)
            {
                enemy.Initialize();
                possibleEnemyCount++;
            }
        }
        enemiesLeft = possibleEnemyCount;

        audioSource.clip = BGM;
        audioSource.loop = true;
        audioSource.Play();
    }

    protected virtual void Update()
    {
        foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
        {
            if (!enemy._initialized)
            {
                enemy.Initialize();
                possibleEnemyCount++;
            }
        }

        if (!character.IsDead)
        {
            character.UpdateCharacter(enemies);
            // character.IsInPatrolScope = _patrolScope.IsPointInPolygon(character.transform.position);

            // if(character.MyPet)
			// {
            //     character.MyPet.IsInPatrolScope = _patrolScope.IsPointInPolygon(character.MyPet.transform.position);
            // }
        }

        myCamera.UpdateCamera();
        foreach (var enemy in enemies)
        {
            if (!enemy.IsDead)
                enemy.UpdateEnemy();
        }
        RemoveDeathEnemy();

        if (WinCondition() && !GameWon && !GameLost)
        {
            Win();
        } 
        else if (LoseCondition() && !GameWon && !GameLost)
        {
            Lose();
        }
    }

    public void Win()
    {
        GameWon = true;
        VictoryScreen.Create();
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.PlayOneShot(WinSFX);
    }

    public void Lose()
    {
        GameLost = true;
        DefeatScreen.Create();
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.PlayOneShot(LoseSFX);
    }

    private void LateUpdate()
    {

    }

    private void RemoveDeathEnemy()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].IsDead)
            {
                if (enemies[i].deleteUponDeath)
                {
                    enemies[i].StopAllCoroutines();
                    enemies[i].OnDeath();
                }
                enemies.RemoveAt(i);
                enemiesLeft -= 1;
            }
        }
    }

    public void AddEnemy(Enemy e)
    {
        enemies.Add(e);
    }

    virtual public bool WinCondition()
    {
        switch (currentGameMode)
        {
            default:
                {
                    if (enemiesLeft == 0)
                        return true; else
                        return false;
                }
        }
    }

    virtual public bool LoseCondition()
    {
        switch (currentGameMode)
        {
            default:
                {
                    return character.IsDead;
                }
        }
    }

    public void ResumeGame()
    {
        GamePaused = false;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        GamePaused = true;
        Time.timeScale = 0;
    }
    #endregion
}
