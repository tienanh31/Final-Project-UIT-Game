using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IEnumeratorEvent : UnityEvent<IEnumerator> { }

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Wave wave;
    public uint enemySpawnLimit { get; protected set; } //how much enemy will spawn
    [SerializeField] protected List<Enemy> spawnedEnemies;

    public IEnumeratorEvent SpawnEvent;
    public UnityEvent OnWaveDoneSpawning;
    public UnityEvent OnAllEnemyKilled;

    public bool _allEnemyKilled { get; protected set; }
    public bool _WaveDoneSpawning { get; protected set; }

    protected bool Activated = false;

    void Awake()
    {
        _allEnemyKilled = true;
        _WaveDoneSpawning = true;
        enemySpawnLimit = 0;
        foreach (SpawnSequence ss in wave.spawnSequences)
        {
            foreach (SpawnInfo si in ss.enemySpawnInfos)
                enemySpawnLimit += si.quantity;
        }
        spawnedEnemies = new List<Enemy>();
    }

    // Start is called before the first frame update
    void Start()    
    {
        // UnityAction<int> a = (int wave) => 
        // {
        //     wave = currentWave;
        //     Debug.Log("Wave " + wave +": All enemies killed");
        // };
        // OnAllCurrentWaveEnemyKilled.AddListener(a);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedEnemies.Count != 0)
        {
            _allEnemyKilled = false;
        }
        if (!_allEnemyKilled && _WaveDoneSpawning && spawnedEnemies.Count == 0)
        {
            _allEnemyKilled = true;
            OnAllEnemyKilled?.Invoke();
        }
    }

    public void SpawnWaveNonCoroutine()
    {
        if (!Activated)
        {
            Activated = true;
            StartCoroutine(SpawnWave());
        }
    }

    public IEnumerator SpawnWave()
    {
        _WaveDoneSpawning = false;
        Activated = true;
        if (wave.spawnSequences.Count > 0)
        {
            int index = 0;

            if (wave.spawnSequences[index].WaitType == WaitBetweenSequenceType.Parallel)
            {
                StartCoroutine(SpawnSpawnSequence(wave.spawnSequences[index], index));
            }
            else
            {
                yield return StartCoroutine(SpawnSpawnSequence(wave.spawnSequences[index], index));
            }
            index++;

            while (index < wave.spawnSequences.Count)
            {
                //yield return new WaitForSeconds(wave.spawnSequences[index].delayPostSequence);
                while (index < wave.spawnSequences.Count &&
                    wave.spawnSequences[index].WaitType == WaitBetweenSequenceType.Parallel)
                {
                    yield return new WaitForSeconds(wave.spawnSequences[index].delayPostSequence);
                    StartCoroutine(SpawnSpawnSequence(wave.spawnSequences[index], index));
                    index++;
                }

                if (spawnedEnemies.Count == 0 && index < wave.spawnSequences.Count)
                {
                    yield return new WaitForSeconds(wave.spawnSequences[index].delayPostSequence);
                    yield return StartCoroutine(SpawnSpawnSequence(wave.spawnSequences[index], index));
                    index++;
                }
            }
            OnWaveDoneSpawning?.Invoke();
            _WaveDoneSpawning = true;
        }
    }

    public IEnumerator SpawnSpawnSequence(SpawnSequence spawnSequence, int i)
    {
        foreach (SpawnInfo enemySpawnInfo in spawnSequence.enemySpawnInfos)
        {
            yield return StartCoroutine(Spawn(enemySpawnInfo, spawnSequence.path));
        }
        yield return null;
    }

    public IEnumerator Spawn(SpawnInfo enemySpawnInfo, Path path)
    {
        for (int i = 0; i < enemySpawnInfo.quantity; i++)
        {
            Enemy e = Instantiate(enemySpawnInfo.enemy, transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            spawnedEnemies.Add(e);
            LevelManager.Instance.AddEnemy(e);
            e.Initialize(path);
            e.OnDeathEvent.AddListener(new UnityAction<Enemy>(OnEnemyDeath));

            yield return new WaitForSeconds(enemySpawnInfo.delayBetweenSpawn);
        }
    }

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(this.transform.position, 0.5f);
        Gizmos.color = Color.red;
        foreach (SpawnSequence ss in wave.spawnSequences)
        {
            if (ss.path != null)
            {
                Gizmos.DrawLine(this.transform.position, ss.path.GetNode(0).transform.position);
            }
        }
    }

    private void OnEnemyDeath(Enemy e)
    {
        spawnedEnemies.Remove(e);
    }
}
