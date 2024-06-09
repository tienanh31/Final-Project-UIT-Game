using System.Collections;
using UnityEngine;

public class Hammer : Trap
{
    [SerializeField] Transform _pivot;
    [SerializeField] float _rotateSpeed = 200f;
    [SerializeField] float _maxAngle = 90f;
    [SerializeField] Transform _direction;
    [SerializeField] float _repelForce = 2000f;

    public bool _isXDirection = true;

    private bool _attackable = true;

    private void Start()
    {
        if (!_isXDirection)
        {
            Vector3 angle = transform.eulerAngles;
            angle.y = 90;
            transform.eulerAngles = angle;


        }
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
            Vector3 axis = new Vector3(0, 0, direction);
            if (!_isXDirection)
            {
                axis.z = 0;
                axis.x = direction;
            }

            transform.RotateAround(_pivot.position, axis, _rotateSpeed * Time.deltaTime);

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
