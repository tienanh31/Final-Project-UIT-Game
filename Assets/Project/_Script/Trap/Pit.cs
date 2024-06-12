using System.Collections;
using UnityEngine;


public class Pit : Trap
{
    [SerializeField] float _fallAcceleration = 5f;
    [SerializeField] float _waitingTime = 0.5f;

    private Vector3 _originPos;

    private bool _isFalling = false;

    public override void Initialize()
    {
        //base.Initialize();

    }

    private void OnCollisionEnter(Collision collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character)
        {
            _isFalling = true;
            StartCoroutine(IE_Fall());
            StartCoroutine(IE_Reset());
        }
    }

    IEnumerator IE_Fall()
    {
        _originPos = transform.position;
        yield return new WaitForSeconds(_waitingTime);

        Vector3 velocity = Vector3.zero;
        while (_isFalling)
        {
            velocity.y -= _fallAcceleration * Time.deltaTime;

            Vector3 position = velocity * Time.deltaTime;

            transform.position += position;

            yield return null;
        }
    }

    IEnumerator IE_Reset()
    {
        yield return new WaitForSeconds(_resetTime);

        _isFalling = false;

        transform.position = _originPos;

        Debug.Log("Reset position");
    }
}
