using System.Collections;
using UnityEngine;

public class IceRain : Trap
{
    [SerializeField] float _fallSpeed = 20f;

    [HideInInspector]
    public float Limit = -1;

    public Vector3 Direction = Vector3.down;

    private void Start()
    {
        StartCoroutine(IE_Fall());
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);

        character.TakenDamage(_damage);
        character.TakenStunEffect(_stunTime);

        Destroy(gameObject);
    }

    IEnumerator IE_Fall()
    {
        while (true)
        {
            transform.position += Direction * _fallSpeed * Time.deltaTime;

            if (transform.position.y < Limit)
            {
                Destroy(gameObject);
            }
            yield return null;
        }
    }
}
