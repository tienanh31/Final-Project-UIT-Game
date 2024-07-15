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
    //public GameObject characterSpawner;
    //public AudioClip BGM;
    //public AudioClip WinSFX;
    //public AudioClip LoseSFX;
    [SerializeField] GameObject _endingGate;

    public bool GamePaused = false;

    //to activate the gamelost event only once
    protected bool GameLost = false;
    protected bool GameWon = false;
    //protected AudioSource audioSource;

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

    public Character MyCharacter => character;

    protected float possibleEnemyCount, enemiesLeft;
    #endregion

    #region Methods
    //private void OnDrawGizmos()
    //{
    //    if (characterSpawner == null)
    //        return;

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere(characterSpawner.transform.position, 0.5f);
    //}

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        damageables = new List<IDamageable>();
        //audioSource = this.GetComponent<AudioSource>();
    }

    public virtual void Initialize(Vector3 playerPosition, Vector3 gatePosition)
    {
        _endingGate.SetActive(false);

        var characterSpawner = playerPosition;

        character = GameObject.FindObjectOfType<Character>();
        if (character == null)
        {
            switch (GameManager.Instance.SelectedCharacter)
            {
                case GameConfig.CHARACTER.CHARACTER_1:
                    character = Character1.Create(null, characterSpawner);
                    break;

                case GameConfig.CHARACTER.CHARACTER_2:
                    character = Character2.Create(null, characterSpawner);
                    break;

                case GameConfig.CHARACTER.CHARACTER_3:
                    character = Character3.Create(null, characterSpawner);
                    break;

                case GameConfig.CHARACTER.CHARACTER_4:
                    character = Character4.Create(null, characterSpawner);
                    break;

                default:
                    character = Character.Create(null, characterSpawner);
                    break;
            }
        }    
        character.Initialize();
        character.CollideEndingGate = Win;

        if (myCamera == null)
        {
            myCamera = Camera.main.gameObject.GetComponent<CameraController>();
        }
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

        //audioSource.clip = BGM;
        //audioSource.loop = true;
        //audioSource.Play();

        Vector3 position = gatePosition;
        position.y += _endingGate.transform.lossyScale.y / 2f;

        _endingGate.transform.position = position;
        _endingGate.transform.forward = Vector3.Normalize(character.transform.position - _endingGate.transform.position);
    
    }

    //public void SpawningEnemies(List<Cell> positions)
    //{
    //    var enemyTypes = GameManager.Instance.GetEnemiesCurrentLevel();

    //    for (int i = 0; i < positions.Count; i++)
    //    {
    //        var enemy = GameManager.Instance.SpawningEnemy((GameConfig.ENEMY)UnityEngine.Random.Range(0, 5),
    //            positions[i].GetPosition());
    //        enemy.Initialize();

    //        enemies.Add(enemy);
    //    }

    //    possibleEnemyCount = enemies.Count;
    //    enemiesLeft = possibleEnemyCount;
    //}

    //public void SpawningEnemies(List<PatrolScope> patrolScopes)
    //{
    //    var enemyTypes = GameManager.Instance.GetEnemiesCurrentLevel();
        
    //    int total = enemyTypes.Sum(e => e.Value);
    //    //Debug.Log("Total enemy: " + total);

    //    foreach (var enemyType in enemyTypes)
    //    {
    //        if (enemyType.Key == GameConfig.ENEMY.TRAP)
    //        {
    //            SpawningTrap(enemyType.Value);
    //            continue;
    //        }

    //        int value = enemyType.Value;
    //        int size = patrolScopes.Count - 1;
    //        while (value > 0)
    //        {
    //            var enemy = GameManager.Instance.SpawningEnemy(enemyType.Key, patrolScopes[size].Corners[0]);
    //            enemy.Initialize(patrolScopes[size]);

    //            enemies.Add(enemy);

    //            value--;
    //            size--;
    //            if (size < 0)
    //            {
    //                size = patrolScopes.Count - 1;
    //            }
    //        }
    //    }

    //    //for (int i = patrolScopes.Count - 1; i >= 0; i--)
    //    //{
    //    //    var enemy = GameManager.Instance.SpawningEnemy((GameConfig.ENEMY)UnityEngine.Random.Range(0, 5),
    //    //        patrolScopes[i].Corners[0]);
    //    //    enemy.Initialize(patrolScopes[i]);

    //    //    enemies.Add(enemy);

    //    //    Debug.Log(patrolScopes[i].Corners.Count);
    //    //}


    //    possibleEnemyCount = enemies.Count;
    //    enemiesLeft = possibleEnemyCount;
    //}

    public void SpawningEnemies(List<EnemyData> enemyDatas)
    {
        foreach(var enemyData in enemyDatas)
        {
            var enemy = GameManager.Instance.SpawningEnemy((GameConfig.ENEMY)enemyData.Type,
                enemyData.Position);
            enemy.Initialize(enemyData.PatrolScope);

            enemies.Add(enemy);
        }

        possibleEnemyCount = enemyDatas.Count;
        enemiesLeft = possibleEnemyCount;
    }

    public void SpawningTraps(List<TrapData> trapDatas, List<Cell> largestArea)
    {
        foreach(var trapData in trapDatas)
        {
            Trap trap = GameManager.Instance.SpawingTrap(trapData.Name, trapData.StartPosition);
            trap.Initialize();

            IceRain icerain = trap as IceRain;
            if (icerain)
            {
                StartCoroutine(IE_IceRain(1.5f, largestArea));
            }


        }
    }

    //private void SpawningTrap(int total)
    //{
    //    var falloffs = GameManager.Instance.MapGenerator.Falloffs;
    //    var largestArea = GameManager.Instance.MapGenerator.LargestArea;

    //    int mapType = GameManager.Instance.GetMapType();

    //    int size = largestArea.Count;
    //    switch(mapType)
    //    {
    //        case 0:
    //        case 1:
    //            {
    //                if (total >= 1)
    //                {
    //                    Vector3 position = Vector3.zero;
    //                    position = largestArea[UnityEngine.Random.Range(size / 5, size / 7)].GetPosition();

    //                    Mud mud = GameManager.Instance.SpawingTrap(typeof(Mud).Name, position) as Mud;
    //                    mud.Initialize();
    //                }

    //                int random = UnityEngine.Random.Range(0, falloffs.Count);
    //                if (total >= 2)
    //                {
    //                    var falloff = falloffs[random];
    //                    foreach (var cell in falloff)
    //                    {
    //                        Pit pit = GameManager.Instance.SpawingTrap(typeof(Pit).Name, cell.GetPosition()) as Pit;
    //                        pit.Initialize();
    //                    }

    //                }
    //                if (total >= 3)
    //                {
    //                    if (falloffs.Count > 1)
    //                    {
    //                        int newRandom = UnityEngine.Random.Range(0, falloffs.Count);
    //                        while (newRandom == random)
    //                        {
    //                            newRandom = UnityEngine.Random.Range(0, falloffs.Count);
    //                        }

    //                        var falloff = falloffs[newRandom];
    //                        foreach (var cell in falloff)
    //                        {
    //                            Pit pit = GameManager.Instance.SpawingTrap(typeof(Pit).Name, cell.GetPosition()) as Pit;
    //                            pit.Initialize();
    //                        }
    //                    }

    //                    Vector3 position = Vector3.zero;


    //                    position = largestArea[UnityEngine.Random.Range(size / 4, size / 5)].GetPosition();
    //                    Hammer hammer = GameManager.Instance.SpawingTrap(typeof(Hammer).Name, position) as Hammer;
    //                    hammer.Initialize();
    //                }
    //            }
    //            break;

    //        case 2:
    //            {
    //                if (total >= 1)
    //                {
    //                    Vector3 position = Vector3.zero;
    //                    position = largestArea[UnityEngine.Random.Range(size / 5, size / 6)].GetPosition();

    //                    IceBoom iceBoom = GameManager.Instance.SpawingTrap(typeof(IceBoom).Name, position) as IceBoom;
    //                    iceBoom.Initialize();
    //                }

    //                if (total >= 2)
    //                {
    //                    if (falloffs.Count > 1)
    //                    {
    //                        Vector3 start = falloffs[0][falloffs[0].Count - 1].GetPosition();
    //                        Vector3 end = falloffs[1][0].GetPosition();

    //                        Iceberg iceberg = GameManager.Instance.SpawingTrap(typeof(Iceberg).Name, start) as Iceberg;
    //                        iceberg.Initialize();
    //                        iceberg.SetData(start, end);

    //                    }
    //                }
    //                if (total >= 3)
    //                {
    //                    if (falloffs.Count > 2)
    //                    {
    //                        Vector3 start = falloffs[1][falloffs[1].Count - 1].GetPosition();
    //                        Vector3 end = falloffs[2][0].GetPosition();

    //                        Iceberg iceberg = GameManager.Instance.SpawingTrap(typeof(Iceberg).Name, start) as Iceberg;
    //                        iceberg.Initialize();
    //                        iceberg.SetData(start, end);
    //                    }

    //                    StartCoroutine(IE_IceRain(1, largestArea));
    //                }
    //            }
    //            break;

    //        case 3:
    //            {
    //                if (total >= 1)
    //                {
    //                    Vector3 position = Vector3.zero;
    //                    position = largestArea[UnityEngine.Random.Range(size / 5, size / 6)].GetPosition();

    //                    FlameBoom flameBoom = GameManager.Instance.SpawingTrap(typeof(FlameBoom).Name, position) as FlameBoom;
    //                    flameBoom.Initialize();
    //                }

    //                if (total >= 2)
    //                {
    //                    if (falloffs.Count > 0)
    //                    {
    //                        Vector3 start = falloffs[0][0].GetPosition();
    //                        Vector3 end = falloffs[0][1].GetPosition();
    //                        if (end.x > start.x)
    //                        {
    //                            end.x = start.x - 1;
    //                        }
    //                        else if (end.x < start.x)
    //                        {
    //                            end.x = start.x + 1;
    //                        }
    //                        else
    //                        {
    //                            if (end.z > start.z)
    //                            {
    //                                end.z = start.z - 1;
    //                            }
    //                            else
    //                            {
    //                                end.z = start.z + 1;
    //                            }
    //                        }
                            


    //                        FlameThrower flameThrower = GameManager.Instance.SpawingTrap(typeof(FlameThrower).Name, start) as FlameThrower;
    //                        flameThrower.Initialize();
    //                        flameThrower.SetData((end - start).normalized);
    //                    }
    //                }
    //                if (total >= 3)
    //                {
    //                    if (falloffs.Count > 1)
    //                    {
    //                        Vector3 start = falloffs[1][0].GetPosition();
    //                        Vector3 end = falloffs[1][1].GetPosition();
    //                        if (end.x > start.x)
    //                        {
    //                            end.x = start.x - 1;
    //                        }
    //                        else if (end.x < start.x)
    //                        {
    //                            end.x = start.x + 1;
    //                        }
    //                        else
    //                        {
    //                            if (end.z > start.z)
    //                            {
    //                                end.z = start.z - 1;
    //                            }
    //                            else
    //                            {
    //                                end.z = start.z + 1;
    //                            }
    //                        }

    //                        FlameThrower flameThrower = GameManager.Instance.SpawingTrap(typeof(FlameThrower).Name, start) as FlameThrower;
    //                        flameThrower.Initialize();
    //                        flameThrower.SetData((end - start).normalized);
    //                    }

    //                    var endPoint = GameManager.Instance.MapGenerator.EndPointCell.GetPosition();

    //                    float minDistance = Vector3.Distance(falloffs[0][0].GetPosition(), endPoint);
    //                    List<Cell> closetFalloff = falloffs[0];
    //                    foreach (var falloff in falloffs)
    //                    {
    //                        float distance = Vector3.Distance(falloff[0].GetPosition(), endPoint);
    //                        if (distance < minDistance)
    //                        {
    //                            closetFalloff = falloff;
    //                            minDistance = distance;
    //                        }
    //                    }

    //                    Vector3 position = (endPoint + closetFalloff[0].GetPosition()) / 2f;

    //                    FireCarpet fireCarpet = GameManager.Instance.SpawingTrap(typeof(FireCarpet).Name, position) as FireCarpet;
    //                    fireCarpet.Initialize();
    //                }
    //            }
    //            break;
    //    }
    //}

    protected IEnumerator IE_IceRain(float time = 1f, List<Cell> largestArea = null)
    {
        while (true)
        {
            Vector3 startPosition = largestArea[UnityEngine.Random.Range(0, largestArea.Count)].GetPosition();
            startPosition.y = UnityEngine.Random.Range(5, 10);
            Vector3 endPosition = largestArea[UnityEngine.Random.Range(0, largestArea.Count)].GetPosition();

            IceRain iceRain = GameManager.Instance.SpawingTrap(typeof(IceRain).Name, startPosition) as IceRain;
            iceRain.Initialize();
            iceRain.Direction = (endPosition - startPosition).normalized;

            yield return new WaitForSeconds(time);
        }
    }

    protected virtual void Update()
    {

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
        //audioSource.loop = false;
        //audioSource.Stop();
        //audioSource.PlayOneShot(WinSFX);
    }

    public void Lose()
    {
        GameLost = true;
        DefeatScreen.Create();
        //audioSource.loop = false;
        //audioSource.Stop();
        //audioSource.PlayOneShot(LoseSFX);
    }

    private void LateUpdate()
    {
        if (enemiesLeft == 0)
        {
            _endingGate.SetActive(true);
        }
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
        return false;
    }

    virtual public bool LoseCondition()
    {
        switch (currentGameMode)
        {
            default:
                {
                    return character.IsDead 
                        || character.transform.position.y < GameManager.Instance.Grid.transform.position.y - 1f;
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
