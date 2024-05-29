using System.Collections;
using UnityEngine;

public class Bullet: MonoBehaviour
{
	#region Fields & Properties
	public float Damage { get; protected set; }
	float range;
	float speed;

	Vector3 direction;
	Vector3 previousPosition;
	public GameObject source;
	#endregion

	#region Methods
	public void Initialize(float dame, float range, float speed, Vector3 direction)
	{
		this.Damage = dame;
		this.range = range;
		this.speed = speed;
		this.direction = direction;

		previousPosition = transform.position;
		StartCoroutine(RangeRemaining());
	}

	IEnumerator RangeRemaining()
	{
		while(range > 0)
		{
			transform.position += direction * speed * Time.deltaTime;

			range -= Vector3.Distance(transform.position, previousPosition);
			//Debug.Log(range);
			previousPosition = transform.position;
			yield return null;
		}

		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.CompareTag(collider.tag))
		{
			return;
		}

		IDamageable damagable = collider.GetComponent<IDamageable>();
		if (damagable != null && !this.CompareTag(collider.tag))
		{
			damagable.TakenDamage(new Damage(Damage, this.transform.position, DamageType.Bullet, source));
		}
		Destroy(gameObject);
	}
	#endregion
}

