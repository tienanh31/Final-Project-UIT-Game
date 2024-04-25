using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class NighttimeEnemy : Enemy
{
    [Header("_~* 	NightTime Enemy Stats")]
    [SerializeField] protected float DetectAngle;
    [SerializeField] protected Light flashlight;

    public override void Initialize(Path p = null)
    {
        base.Initialize(p);
        flashlight.spotAngle = DetectAngle;
        flashlight.range = _detectRange;
    }

    public override void AlertNearbyEnemies(GameObject gameObject)
    {
        AlertAllEnemies(gameObject);
    }

    public override void Alert(GameObject gameObject)
    {
        base.Alert(gameObject);
        _turningSpeed *= 360;
    }

    protected override Transform DetectTarget()
    {
        Transform target = null;
        float maxPriority = -9999;

        foreach (Collider c in Physics.OverlapSphere(this.transform.position, _detectRange, LayerMask.GetMask("Damageables")))
        {

            if (c.gameObject.CompareTag(this.tag)) continue;
            IDamageable damageable = c.gameObject.GetComponent<IDamageable>();
            if (damageable == null) continue;
            if (damageable.IsDead) continue;

            Transform damagableTransform = c.gameObject.transform;
            if (Vector3.Angle(this.transform.forward, damagableTransform.position - this.transform.position) > DetectAngle / 2)
                continue;

            RaycastHit[] info = Physics.RaycastAll(this.transform.position, damagableTransform.position - this.transform.position, Vector3.Distance(this.transform.position, damagableTransform.position));
            bool blocked = false;
            foreach (RaycastHit hit in info)
            {
                //theres an object blocking
                if (hit.collider.gameObject.GetComponent<IDamageable>() == null)
                {
                    blocked = true;
                    break;
                }
            }
            if (blocked) continue;

            if (Vector3.Distance(damagableTransform.position, this.transform.position) <= _detectRange)
            {
                if (damageable.AttackPriority > maxPriority)
                {
                    target = damagableTransform;
                    maxPriority = damageable.AttackPriority;
                }
            }
        }
        return target;
    }
}
