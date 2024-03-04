using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvent : MonoBehaviour
{
    [SerializeField] public string ID;
    public UnityEvent Events;
    public UnityEvent onEventsStart;
    public UnityEvent onEventsComplete;

    public virtual IEnumerator Invoke()
    {
        bool isDone = false;
        UnityAction action = () => isDone = true;
        onEventsStart.AddListener(action);
        Events.AddListener(action);
        onEventsComplete.AddListener(action);

        onEventsStart.Invoke();
        yield return new WaitUntil(() => isDone);

        isDone = false;
        Events.Invoke();
        yield return new WaitUntil(() => isDone);

        isDone = false;
        onEventsComplete.Invoke();
        yield return new WaitUntil(() => isDone);
    }
}