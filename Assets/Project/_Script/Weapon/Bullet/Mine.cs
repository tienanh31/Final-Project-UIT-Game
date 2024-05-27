using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Mine : MonoBehaviour
{
	#region Fields & Properties
	[SerializeField] float _explosionRadius = 3f, _damage = 40f, _activateTimer, _aliveTimer = 10f, _detectionRange = 1f;
	[SerializeField] bool _damageScaleWithDistance;
	bool canExplode = false;
	public GameObject source;
	#endregion

	#region Methods
	public static Mine Create(Vector3 position, string tag, float bonusDame)
	{
		Mine mine = Instantiate(Resources.Load<Mine>("_Prefabs/Bullet/Mine"), position, new Quaternion());
		mine.tag = tag;
		mine._damage += bonusDame;

		return mine;
	}

	//public void Initialize(float damage, float explosionRadius)
	//{

	//}

	void Update()
	{
		if (_activateTimer > 0)
		{
			_activateTimer -= Time.deltaTime;
		} else
		{
			canExplode = true;
		}

		if (_aliveTimer > 0)
		{
			_aliveTimer -= Time.deltaTime;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.CompareTag(collider.tag))
		{
			return;
		}

		CollisionCheck();
		if (canExplode)
		{
			Explode();
		}
	}

	protected virtual void CollisionCheck()
	{
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, _detectionRange, transform.forward, _detectionRange);
		if (hits.Length > 0)
		{
			foreach (var e in hits)
			{
				if (e.collider.gameObject.GetComponent<Enemy>() && this.tag == "Player")
				{
					Explode();
				} else
				if ((e.collider.gameObject.GetComponent<Character>() || e.collider.gameObject.GetComponent<Pet>()) && this.tag == "Enemy")
				{
					Explode();
				}
			}
		}
	}

	protected virtual void Explode()
	{
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, _explosionRadius, transform.forward, _explosionRadius);
		if (hits.Length > 0)
		{
			foreach (var e in hits)
			{
				if (this.CompareTag(e.collider.tag))
				{
					continue;
				}

				float damage = _damage;
				if (_damageScaleWithDistance)
				{
					float distance = Vector3.Distance(this.transform.position, e.point);
					damage = _damage * (distance / _explosionRadius);
				}
				if (e.collider.gameObject.GetComponent<IDamageable>() != null)
				{
					float distance = e.distance;
					float Damage = _damageScaleWithDistance ? _damage * (1 / (distance / _explosionRadius)) : _damage;
					e.collider.gameObject.GetComponent<IDamageable>().TakenDamage(new Damage(Damage, this.transform.position, DamageType.Explosive, source));
				}

			}
		}
		Debug.Log("Mine Explosion");
		Destroy(gameObject);
	}

	[ExecuteInEditMode]
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(this.transform.position, _detectionRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, _explosionRadius);
	}
	#endregion
}

