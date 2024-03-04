using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TestEnemy : Enemy, IDamageable
{
    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        UpdateEnemy();
    }
}

