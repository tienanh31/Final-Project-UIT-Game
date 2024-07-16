using System.Collections;
using UnityEngine;

public class IceBoom : Trap
{
    public override void Initialize()
    {
        base.Initialize();

        StartCoroutine(IE_ChangePhysic());
    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);

        character.TakenDamage(_damage);
        var ice = Ice.Create(character.transform);
        ice.Icing(character);

        Destroy(gameObject);
    }

    protected IEnumerator IE_ChangePhysic()
    {
        while (true)
        {
            Vector3 oldPos = transform.position;

            yield return new WaitForSeconds(0.1f);

            if (Vector3.Distance(oldPos, transform.position) < 0.1f)
            {
                BoxCollider body = GetComponent<BoxCollider>();
                if (body)
                {
                    body.isTrigger = true;
                }

                Rigidbody rig = GetComponent<Rigidbody>();
                if (rig)
                {
                    rig.useGravity = false;
                }

                break;
            }
        }
    }
}
