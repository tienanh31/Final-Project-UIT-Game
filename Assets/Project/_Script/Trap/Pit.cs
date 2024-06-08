using System.Collections;
using UnityEngine;


public class Pit : Trap
{
    [SerializeField] float _fallAcceleration = 5f;
    [SerializeField] float _waitingTime = 0.5f;

    public override void Initialize()
    {
        base.Initialize();

    }

    private void OnCollisionEnter(Collision collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character)
        {
            StartCoroutine(IE_Fall());
        }
    }

    IEnumerator IE_Fall()
    {
        yield return new WaitForSeconds(_waitingTime);

        Vector3 velocity = Vector3.zero;
        while (true)
        {
            velocity.y -= _fallAcceleration * Time.deltaTime;

            Vector3 position = velocity * Time.deltaTime;

            transform.position += position;

            yield return null;
        }
    }
}
