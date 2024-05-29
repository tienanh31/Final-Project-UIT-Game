using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Does this sequence happens parallel with the previous sequence or after?
public enum WaitBetweenSequenceType
{
    Continous,
    Parallel,
    PostSequence
}

[System.Serializable]
public class SpawnInfo
{
    [SerializeField] public Enemy enemy;
    [SerializeField] public uint quantity;
    [SerializeField] public float delayBetweenSpawn; // delay between spawning each enemy
}

[System.Serializable]
public class SpawnSequence
{
    [SerializeField] public Path path;
    [SerializeField] public SpawnInfo[] enemySpawnInfos;
    //Does this sequence happens parallel with the previous sequence or after?
    [SerializeField] public WaitBetweenSequenceType WaitType;
    [SerializeField] public float delayPostSequence;
}

[System.Serializable]
public class Wave
{
    //Time delay before spawning wave
    [SerializeField] public float delayBeforeSpawn;
    [SerializeField] public List<SpawnSequence> spawnSequences;
}