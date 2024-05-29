using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicalTrigger : MonoBehaviour
{
    public GameEventTrigger e;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Character>())
        {
            e.TriggerGameEvent();
        }
    }
}
