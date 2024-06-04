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
        if (other.CompareTag("Player"))
        {
            var character = other.GetComponent<Character>();
            TriggerEnter(character);
        }
    }

    protected virtual void TriggerStay()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerStay();
        }
    }

    protected virtual void TriggerExit(Character character)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var character = other.GetComponent<Character>();
            TriggerExit(character);
        }
    }
}
