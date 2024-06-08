using System.Collections;
using UnityEngine;

public class Hammer : Trap
{
    [SerializeField] Transform _pivot;
    [SerializeField] float _rotateSpeed = 200f;
    [SerializeField] float _maxAngle = 90f;
    [SerializeField] Transform _direction;
    [SerializeField] float _repelForce = 2000f;

    private bool _attackable = true;

    private void Start()
    {
        StartCoroutine(IE_Rotate());
    }

    public override void Initialize()
    {
        base.Initialize();

    }

    private void OnCollisionEnter(Collision collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character && _attackable)
        {
            character.TakenDamage(_damage);
            character.TakenStunEffect(_stunTime);

            Vector3 direction = new Vector3(character.transform.position.x - _direction.position.x, 0,
                character.transform.position.z - _direction.position.z);
            
            character.Repel(direction * _repelForce);

            StartCoroutine(IE_Reset());
        }
    }

    IEnumerator IE_Reset()
    {
        _attackable = false;
        yield return new WaitForSeconds(_resetTime);
        _attackable = true;
    }

    IEnumerator IE_Rotate()
    {
        float direction = 1;
        while (true)
        {
            transform.RotateAround(_pivot.position, new Vector3(0, 0, direction), _rotateSpeed * Time.deltaTime);

            var angle = transform.eulerAngles.z;
            if (angle > 180)
            {
                angle -= 360;
            }

            if (angle >= _maxAngle)
            {
                direction = -1;
            }
            else if (angle <= -_maxAngle)
            {
                direction = 1;
            }
            yield return null;
        }
    }
}
