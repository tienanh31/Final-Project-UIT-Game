using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[System.Serializable]
public class Scenario : MonoBehaviour
{
    private static Scenario instance;
    public static Scenario Instance
    {
        get => instance;
        private set => instance = value;
    }

    [SerializeField]
    public List<GameEvent> gameEvents;
    public UnityEvent OnStartEvent, OnWinEvent, OnLoseEvent;

    void Awake()
    {
        Destroy(instance);
        instance = this;

        UnityAction a = () =>
        {
            LevelManager.Instance.Win();
            InvokeGameEvent("End");
        };
        GetGameEvent("Win")?.onEventsStart.AddListener(a);
        OnWinEvent?.AddListener(a);

        UnityAction b = () =>
        {
            LevelManager.Instance.Lose();
            InvokeGameEvent("End");
        };
        GetGameEvent("Lose")?.onEventsStart.AddListener(b);
        OnLoseEvent?.AddListener(b);

        // UnityAction c = () =>
        // {
        //     Debug.Log("First Event");
        // };
        // gameEvents[0].Events?.AddListener(c);
    }

    void Start()
    {
        OnStartEvent?.Invoke();
    }

    public void InvokeGameEvent(int index)
    {
        GameEvent e = gameEvents[index];
        if (e != null)
            StartCoroutine(e.Invoke());
    }

    public void InvokeGameEvent(string id)
    {
        GameEvent e = gameEvents.FirstOrDefault(x => x.ID == id);
        if (e != null)
            StartCoroutine(e.Invoke());
    }

    public GameEvent GetGameEvent(string id)
    {
        return gameEvents.FirstOrDefault(x => x.ID == id);
    }
}

[CustomEditor(typeof(Scenario)), CanEditMultipleObjects]
public class ScenarioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Scenario myTarget = (Scenario)target;

        if (GUILayout.Button("GetAllEvent"))
        {
            myTarget.gameEvents.Clear();
            myTarget.gameEvents.AddRange(GameObject.FindObjectsOfType<GameEvent>(true));
        }

        DrawDefaultInspector();
    }
}
