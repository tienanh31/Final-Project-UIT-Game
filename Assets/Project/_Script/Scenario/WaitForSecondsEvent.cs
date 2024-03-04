using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitForSecondsEvent : GameEvent
{
    [SerializeField] private float _startDelayTime, _endDelayTime;

    public WaitForSecondsEvent(string i, float startDelayTime, float endDelayTime)
    {
        ID = i;
        _startDelayTime = startDelayTime;
        _endDelayTime = endDelayTime;
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
        yield return new WaitForSeconds(_startDelayTime);

        isDone = false;
        Events.Invoke();
        yield return new WaitUntil(() => isDone);

        isDone = false;
        onEventsComplete.Invoke();
        yield return new WaitUntil(() => isDone);
        yield return new WaitForSeconds(_endDelayTime);
    }
}
