using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideBomb : IWeapon
{
    [SerializeField] protected float _explosionTimer, _explosionRadius, _damage, _selfDamage;
    [SerializeField] protected bool _damageScaleWithDistance, _damageAllies;
    [SerializeField] protected GameObject explosionParticle;
    [SerializeField] protected AudioClip explosionSFX;
    protected AudioSource audioSource;

    public override void Initialize()
    {
        this.tag = transform.parent.tag;
        audioSource = this.GetComponent<AudioSource>();
    }

    public override void AddDamageBonus(float dame)
    {
        base.AddDamageBonus(dame);

        _damage += dame;
    }

    public override void AttemptAttack()
    {
        StartCoroutine(Attack());
    }

    public override void AttemptReload() { }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(_explosionTimer);
        Explode();
    }

    protected virtual void Explode()
    {
        int layermask = LayerMask.GetMask("Damageables");
        RaycastHit[] info = Physics.SphereCastAll(this.transform.position, _explosionRadius, Vector3.up, 0, layermask);
        foreach (RaycastHit hit in info)
        {
            if ((!_damageAllies && this.tag == hit.transform.tag) || (transform.parent.gameObject == hit.collider.gameObject))
            {
                continue;
            }

            //check if theres a wall between
            bool c = false;
            Vector3 hitlocation = (hit.point == Vector3.zero) ? hit.transform.position : hit.point;
            RaycastHit[] info2 = Physics.RaycastAll(this.transform.position, hitlocation, Vector3.Distance(this.transform.position, hit.transform.position));
            foreach (RaycastHit hit2 in info2)
            {
                //theres an object blocking
                if (hit2.collider.gameObject.GetComponent<BulletproofWall>() || (hit2.collider.gameObject.GetComponent<IDamageable>() == null))
                {
                    c = true;
                }
            }
            if (c) continue;

            if (hit.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                float distance = hit.distance;
                float Damage = _damageScaleWithDistance ? _damage * (1 / (distance / _explosionRadius)) : _damage;
                hit.collider.gameObject.GetComponent<IDamageable>().TakenDamage(new Damage(Damage, this.transform.position, DamageType.Explosive, source));
            }
            Debug.DrawLine(this.transform.position, hitlocation, Color.green, 5f);
        }
        StopAllCoroutines();
        Instantiate(explosionParticle, this.transform.position, new Quaternion());
        audioSource.Stop();
        audioSource.PlayOneShot(explosionSFX);
        if (gameObject.GetComponentInParent<IDamageable>() != null)
        {
            gameObject.GetComponentInParent<IDamageable>().TakenDamage(new Damage(_selfDamage, null, null, null));
        }
    }

    [ExecuteInEditMode]
    protected virtual void OnDrawGizmos()
    {
        if (this.tag == "Player")
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(this.transform.position, _explosionRadius);
    }
}
