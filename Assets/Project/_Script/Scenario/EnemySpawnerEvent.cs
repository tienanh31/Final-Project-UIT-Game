using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnerEvent : GameEvent
{
    private EnemySpawner enemySpawner;

    void Awake()
    {
        enemySpawner = GetComponent<EnemySpawner>();
    }

    public override IEnumerator Invoke()
    {
        bool isDone = false;
        UnityAction action = () => isDone = true;
        onEventsStart.AddListener(action);
        Events.AddListener(action);
        onEventsComplete.AddListener(action);

        onEventsStart.Invoke();
        yield return new WaitUntil(() => isDone);

        yield return StartCoroutine(enemySpawner.SpawnWave());
        isDone = false;
        Events.Invoke();
        yield return new WaitUntil(() => isDone);

        isDone = false;
        onEventsComplete.Invoke();
        yield return new WaitUntil(() => isDone);
    }
}
