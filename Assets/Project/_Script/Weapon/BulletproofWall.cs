using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class BulletproofWall : IWeapon, IDamageable
{
	#region Fields & Properties
	[SerializeField] protected float _HP = 100f;
	[SerializeField] protected float remainingTime = Mathf.Infinity;

	public float AttackPriority { get; protected set; }
	public bool IsDead { get; protected set; }
	public bool IsInPatrolScope { get; set; }
	#endregion

	#region Methods
	public static BulletproofWall Create(Vector3? size = null, float HP = 100f, float time = 3f, Vector3? position = null, Quaternion? rotation = null)
	{
		BulletproofWall wall = Instantiate(Resources.Load<BulletproofWall>("_Prefabs/Weapon/BulletProofWall"));
		if (size != null)
		{
			wall.transform.localScale = (Vector3)size;
		}
		wall.transform.rotation = (Quaternion)rotation;
		wall.transform.position =  (Vector3)position + wall.transform.forward * 2f;
		wall._HP = HP;
		wall.remainingTime = time;
		wall.IsDead = false;

		return wall;
	}

	public override void Initialize()
	{
		Type = GameConfig.WEAPON.BULLETPROOF_WALL;
		if (remainingTime != Mathf.Infinity)
		{
			StartCoroutine(IE_RemainingTime());
		}
	}

	private void Start()
	{
		IsDead = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.CompareTag(other.tag))
		{
			return;
		}

		//bounce back gernades
		if (other.gameObject.GetComponent<Gernade>())
		{
			Rigidbody rb = other.GetComponent<Rigidbody>();
			Vector3 direction = Vector3.Angle(transform.forward, rb.transform.position - this.transform.position) < 90f ?
			transform.forward : -transform.forward;
			//Debug.DrawRay(this.transform.position, direction, Color.blue, 5f);
			//Debug.DrawRay(this.transform.position, other.transform.position - this.transform.position, Color.red, 5f);
			direction = Vector3.Reflect(rb.velocity.normalized, direction);
			direction = direction.normalized * rb.velocity.magnitude;
			rb.AddForce(direction, ForceMode.Impulse);
			//Debug.DrawRay(this.transform.position, direction, Color.green, 5f);
		}
	}

	public void TakenDamage(Damage Damage)
	{
		if (_HP >= 0)
		{
			_HP -= Damage.value;
			Debug.Log($"Wall hp: {_HP}");
			if (_HP <= 0)
			{
				IsDead = true;
				StopAllCoroutines();
				Destroy(gameObject);
			}
		}
	}

	public virtual float GetHP()
	{
		return _HP;
	}

	private IEnumerator IE_RemainingTime()
	{
		yield return new WaitForSeconds(remainingTime);
		IsDead = true;
		Destroy(gameObject);
	}
	#endregion
}

