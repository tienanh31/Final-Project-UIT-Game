using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sword : IWeapon
{
	#region Fields & Properties
	[SerializeField] protected SO_WeaponGunStats soStats;

	protected float _damage;
	protected float _attackSpeed;
	protected float _attackRange;
	protected float delayBetweenSwings;

	private bool isAttackable;
	#endregion

	#region Methods
	public override void Initialize()
	{
		Type = GameConfig.WEAPON.SWORD;
		isAttackable = true;

		_damage = soStats.DAMAGE_DEFAULT;
		_attackSpeed = soStats.ATTACK_SPEED_DEFAULT;
		_attackRange = soStats.ATTACK_RANGE_DEFAULT;
		delayBetweenSwings = 1f / _attackSpeed; //How many times you can swing each second.
	}

	public override void AddDamageBonus(float dame)
	{
		base.AddDamageBonus(dame);

		_damage += dame;
	}

	public override void AttemptAttack()
	{
		if (isAttackable)
			StartCoroutine(Attack());
	}

	protected IEnumerator Attack()
	{
		isAttackable = true;
		yield return new WaitForSeconds(delayBetweenSwings);
		isAttackable = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isAttackable)
		{
			return;
		}

		if (CompareTag(other.tag))
		{
			return;
		}

		IDamageable target = other.GetComponent<IDamageable>();
		if (target != null)
		{
			target.TakenDamage(new Damage(_damage, this.transform.position, DamageType.Melee, source));
			isAttackable = false;
		}
	}

	[ExecuteInEditMode]
	private void OnDrawGizmos()
	{
		if (soStats != null)
		{
			if (this.tag == "Player")
				Gizmos.color = Color.blue;
			else
				Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(this.transform.position, _attackRange);
		}
	}

	#endregion
}

