using System.Collections;
using UnityEngine;


public class Iceberg : Trap
{
    [SerializeField] float _speed = 10f;

    public Vector3 StartPosition = new Vector3(0, .5f, 0);
    public Vector3 EndPosition = new Vector3(10, .5f, 0);

    private void Start()
    {
        StartCoroutine(IE_Move());
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    IEnumerator IE_Move()
    {
        Vector3 direction;
        while (true)
        {
            direction = (EndPosition - StartPosition).normalized;

            transform.position += direction * _speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, StartPosition) >=
                Vector3.Distance(EndPosition, StartPosition))
            {
                (StartPosition, EndPosition) = (EndPosition, StartPosition);
            }

            yield return null;
        }
    }
}
