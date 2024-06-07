using System.Collections;
using UnityEngine;


public class Trap : MonoBehaviour
{
    [SerializeField] protected float _damage = 20f;
    [SerializeField] protected float _resetTime = 0f;
    [SerializeField] protected float _stunTime = 0f;
    [SerializeField] protected bool _isSlow = false;

    public virtual void Initialize()
    {

    }

    protected virtual void TriggerEnter(Character character)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponent<Character>();
        if (character)
        {
            TriggerEnter(character);
        }
    }

    protected virtual void TriggerStay(Character character)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        var character = other.GetComponent<Character>();
        if (character)
        {
            TriggerStay(character);
        }
    }

    protected virtual void TriggerExit(Character character)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        var character = other.GetComponent<Character>();
        if (character)
        {
            TriggerExit(character);
        }
    }
}
