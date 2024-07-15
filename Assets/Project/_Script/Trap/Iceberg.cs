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

    public void SetData(Vector3 start, Vector3 end)
    {
        StartPosition = start;
        EndPosition = end;

        transform.rotation = Quaternion.LookRotation((end - start).normalized);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Initialize(TrapData trapData)
    {
        base.Initialize(trapData);

        SetData(trapData.StartPosition, trapData.EndPosition);
    }

    protected override void TriggerStay(Character character)
    {
        base.TriggerStay(character);
        character.Falling();
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
