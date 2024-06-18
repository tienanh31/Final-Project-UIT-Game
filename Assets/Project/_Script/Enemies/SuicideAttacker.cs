using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuicideAttacker : Enemy
{
	#region Fields & Properties

	public float _damageDefault;
	public GameObject explosionParticle;

	#endregion

	#region Methods

	public override void Initialize(PatrolScope p = null)
	{
		base.Initialize(p);
	}

	public override void UpdateEnemy()
	{
		_animator.SetInteger("State", 1);

		if (target != null)
		{
			var player = target.GetComponent<Character>();
			player.IsInPatrolScope = _patrolScope.IsPointInPolygon(player.transform.position);

			float distance = Vector3.Distance(transform.position, target.position);
			if (distance <= _attackRange)
			{
				//stop walking and start attacking.
				RotateWeapon(target.position);
				Explode();
				_animator.SetInteger("State", 2);
			}
			else if (distance <= _detectRange)
			{
				enemyAgent.SetDestination(target.position);
				RotateWeapon(target.position);
			}
			else if (target.GetComponent<IDamageable>().IsInPatrolScope)
			{
				enemyAgent.SetDestination(target.position);
				RotateWeapon(target.position);
			}
			else
			{
				target = null;
			}
		}
		else
		{
			target = DetectTarget();
		}
		if (movementBehaviour)
		{
			MovementBehaviour();
		}

		_patrolScope.Debug();
		Debug.DrawLine(transform.position, enemyAgent.destination, Color.blue);
	}

	private void Explode()
	{
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, soStats.ATTACK_RANGE_DEFAULT,
												  transform.up);

		foreach(var hit in hits)
		{
			if (CompareTag(hit.collider.tag))
			{
				continue;
			}

			IDamageable target = hit.collider.gameObject.GetComponent<IDamageable>();
			if (target != null) 
			{
				target.TakenDamage(new Damage(_damageDefault, this.transform.position, DamageType.Explosive, this.gameObject));
			}
		}
		Instantiate(explosionParticle, this.transform.position, new Quaternion());
		TakenDamage(new Damage(soStats.HP_DEFAULT, null, null, null));
	}

	#endregion
}

