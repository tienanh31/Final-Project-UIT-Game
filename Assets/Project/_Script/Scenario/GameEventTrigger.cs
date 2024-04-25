using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    public List<GameEvent> gameEvent;
    [SerializeField] protected bool OneTimeUseOnly = false;

    protected bool Used = false;

    public void TriggerGameEvent()
    {
        if ((!Used && !OneTimeUseOnly) || (OneTimeUseOnly))
        {
            if (gameEvent != null)
            {
                Debug.Log("Triggered");
                foreach (GameEvent e in gameEvent)
                    StartCoroutine(e.Invoke());
            }
            Used = true;
        }
    }
}
