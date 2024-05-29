using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : MonoBehaviour
{
	[SerializeField] protected float _speed, _rotationSpeed, _minDistance = 1f, _minPlayerDistance = 1f, _damage, _interval;
	[SerializeField] protected float _detectRange;
	[SerializeField] protected float aliveTimer;
	[SerializeField] Enemy target;
	[SerializeField] GameObject followTarget;

	Vector3 targetPosition, lookPosition;
	bool attackable = true;

	public static Drone Create(Vector3? position = null, Quaternion? rotation = null)
	{
		Drone drone = Instantiate(Resources.Load<Drone>("_Prefabs/Pet/Drone"));
		drone.transform.rotation = (Quaternion)rotation;
		drone.transform.position = (Vector3)position;
		return drone;
	}

	void Start()
	{
		this.tag = "Player";
		StartCoroutine(ignoreCollision());
	}

	IEnumerator ignoreCollision()
	{
		Physics.IgnoreCollision(this.GetComponent<Collider>(), Character.Instance.GetComponent<Collider>(), true);
		yield return new WaitForSeconds(0.5f);
		Physics.IgnoreCollision(this.GetComponent<Collider>(), Character.Instance.GetComponent<Collider>(), false);
	}

	void Update()
	{
		if (aliveTimer > 0)
			aliveTimer -= Time.deltaTime;
		else
		{
			StopAllCoroutines();
			Destroy(this.gameObject);
			return;
		}

		
		if (target == null)
		{
			target = FindTarget();
		}
		if (target != null)
		{
			followTarget = target.gameObject;
			targetPosition = followTarget.transform.position;
			lookPosition = targetPosition;
			lookPosition.y = 0;
		} else
		{
			followTarget = Character.Instance.gameObject;
			targetPosition = followTarget.transform.position;
			lookPosition = targetPosition;
			lookPosition.y = 0;
		}

		var q = Quaternion.LookRotation(lookPosition - transform.position);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, q, _rotationSpeed * Time.deltaTime);

		if (followTarget == Character.Instance.gameObject)
		{
			if (Vector3.Distance(targetPosition, this.transform.position) > _minPlayerDistance)
			{
				this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, _speed * Time.deltaTime);
			}
		} else
		{
			if (Vector3.Distance(targetPosition, this.transform.position) > _minDistance)
			{
				this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, _speed * Time.deltaTime);
			}
			else if (target != null && attackable)
			{
				StartCoroutine(Attack());
			}
		}
	}

	Enemy FindTarget()
	{
		RaycastHit[] info = Physics.SphereCastAll(this.transform.position, _detectRange, Vector3.up, _detectRange);
		float minDistance = 9999f;
		RaycastHit? final = null;
		foreach (RaycastHit hit in info)
		{
			if (hit.collider.gameObject.GetComponent<Enemy>())
			{
				if (Vector3.Distance(this.transform.position, hit.point) < minDistance)
				{
					final = hit;
					minDistance = Vector3.Distance(this.transform.position, hit.point);
				}
			}
		}
		if (final != null)
		{
			RaycastHit a = (RaycastHit)(final);
			target = a.collider.gameObject.GetComponent<Enemy>();
			return target;
		} else return null;
	}

	IEnumerator Attack()
	{
		attackable = false;
		target.GetComponent<Enemy>().TakenDamage(new Damage(_damage, this.transform.position, DamageType.Default, this.gameObject));
		yield return new WaitForSeconds(_interval);
		attackable = true;
	}

	public void SetTarget(Enemy Target)
	{
		target = Target;
	}

	[ExecuteInEditMode]
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(this.transform.position, _detectRange);
		Gizmos.DrawWireSphere(this.transform.position, _minDistance);
	}
}

