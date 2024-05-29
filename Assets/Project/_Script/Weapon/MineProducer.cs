using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MineProducer : IWeapon
{
	#region Fields & Properties
	[SerializeField] float _maxBulletCount, _cooldown, _attackRange, _attackSpeed, _maxRange, _fuseTime;

	private float currentBulletQuantity, cooldownTimer, delayBetweenThrow;
	private bool canPlaceMine = true;

	private float _bonusDame = 0;
	#endregion

	#region Methods
	public override void Initialize()
	{
		Type = GameConfig.WEAPON.MINE_PRODUCER;
		currentBulletQuantity = 3;
		delayBetweenThrow = 60f / _attackSpeed;
		BulletChange?.Invoke((int)currentBulletQuantity);
	}

	public override void AddDamageBonus(float dame)
	{
		base.AddDamageBonus(dame);
		_bonusDame += dame;
	}

	void Update()
	{
		if (cooldownTimer > 0)
		{
			cooldownTimer -= Time.deltaTime;
		}
		else
		if (currentBulletQuantity < _maxBulletCount)
		{
			currentBulletQuantity++;
			BulletChange?.Invoke((int)currentBulletQuantity);

			cooldownTimer = _cooldown;
		}
	}

	public override int GetCurrentBullet => (int)currentBulletQuantity;

	public override void AttemptAttack()
	{
		if (currentBulletQuantity > 0 && canPlaceMine)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Vector3 l = hit.point;
				if (Vector3.Distance(hit.point, this.transform.position) < _attackRange)
				{
					l = (Vector3)(hit.point - this.transform.position).normalized * _attackRange;
				}
				l.y += 0.1f;
				StartCoroutine(Attack(l));
			}
				
		}
	}

	protected IEnumerator Attack(Vector3 location)
	{
		// spawn mine
		canPlaceMine = false;
		
		Mine mine = Mine.Create(location, this.tag, _bonusDame);
		mine.source = this.source;
		yield return new WaitForSeconds(delayBetweenThrow);

		currentBulletQuantity -= 1;
		BulletChange?.Invoke((int)currentBulletQuantity);

		canPlaceMine = true;
	}

	#endregion
}

