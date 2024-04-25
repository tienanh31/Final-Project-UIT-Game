using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Gernade : MonoBehaviour
{
    [SerializeField] protected float explosionTimer, _explosionRadius, _damage;
    [SerializeField] protected bool _damageScaleWithDistance;
    [SerializeField] protected GameObject explosionParticle;
    [HideInInspector] public GameObject source;

    [SerializeField] protected AudioClip explodeSFX;
    [SerializeField] protected AudioClip bounceSFX;

    protected AudioSource audioSource;
    protected bool exploded = false;

    protected virtual void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public virtual void Update()
    {
        if (explosionTimer > 0)
        {
            explosionTimer -= Time.deltaTime;
        } else
        {
            if (!exploded)
                Explode();
        }
    }

    protected virtual void Explode()
    {
        exploded = true;
        int layermask = LayerMask.GetMask("Damageables");
        RaycastHit[] info = Physics.SphereCastAll(this.transform.position, _explosionRadius, Vector3.up, 0, layermask);
        foreach (RaycastHit hit in info)
        {
            if (this.tag == hit.transform.tag)
            {
                continue;
            }
            
            //check if theres a wall between
            bool c = false;
            Vector3 hitlocation = (hit.point == Vector3.zero) ? hit.transform.position : hit.point;
            RaycastHit[] info2 = Physics.RaycastAll(this.transform.position, hitlocation - this.transform.position, Vector3.Distance(this.transform.position, hit.transform.position));
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
            //Debug.DrawLine(this.transform.position, hitlocation, Color.green, 5f);
        }

        StopAllCoroutines();
        Instantiate(explosionParticle, this.transform.position, new Quaternion());
        audioSource.Stop();
        audioSource.PlayOneShot(explodeSFX);
        this.GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, explodeSFX.length);
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (!other.collider.isTrigger)
        {
            audioSource.PlayOneShot(bounceSFX);
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

