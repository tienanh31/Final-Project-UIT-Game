using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : Pet
{
	#region Fields & Properties
	[SerializeField] private Bullet bulletPrefab;
	[SerializeField] protected SO_Turret soStats;

	[SerializeField] protected float _turningSpeed;
	#endregion

	#region Methods
	public static Turret Create(Transform parent = null)
	{
		Turret turret = Instantiate(Resources.Load<Turret>("_Prefabs/Pet/Turret"), parent);

		return turret;
	}

	public override void Initialize(string tag)
	{
		Type = GameConfig.PET.TURRET;
		base.Initialize(tag);

		Stats.Add(GameConfig.STAT_TYPE.HP, soStats.HP_DEFAULT);
		Stats.Add(GameConfig.STAT_TYPE.ATTACK_SPEED, soStats.ATTACK_SPEED_DEFAULT);
		Stats.Add(GameConfig.STAT_TYPE.DAMAGE, soStats.DAMAGE_DEFAULT);
		Stats.Add(GameConfig.STAT_TYPE.BULLET_SPEED, soStats.BULLET_SPEED);
		Stats.Add(GameConfig.STAT_TYPE.ATTACK_RANGE, soStats.ATTACK_RANGE_DEFAULT);
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

	public override void UpdatePet(List<Enemy> enemies = null)
	{
		base.UpdatePet(enemies);
	}

	protected override void Attack()
	{
		base.Attack();

		if (!target)
		{
			return;
		}
		var q = Quaternion.LookRotation(target.transform.position - transform.position);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, q, _turningSpeed * Time.deltaTime);

		if (attackable)
		{
			Bullet bullet = Instantiate(bulletPrefab, transform.position, new Quaternion());
			bullet.Initialize(Stats[GameConfig.STAT_TYPE.DAMAGE],
							  Stats[GameConfig.STAT_TYPE.ATTACK_RANGE],
							  Stats[GameConfig.STAT_TYPE.BULLET_SPEED],
							 (target.position - transform.position).normalized);
			bullet.tag = this.tag;

			attackable = false;
			StartCoroutine(IE_Reload());
		}
	}

	public override void TakenDamage(Damage damage)
	{
		base.TakenDamage(damage);
		if(Stats[GameConfig.STAT_TYPE.HP] > 0)
		{
			Stats[GameConfig.STAT_TYPE.HP] -= damage.value;
			if(Stats[GameConfig.STAT_TYPE.HP] <= 0)
			{
				IsDead = true;
			}
		}

		//Debug.Log($"Pet hp: {Stats[GameConfig.STAT_TYPE.HP]}");
	}

	[ExecuteInEditMode]
	protected virtual void OnDrawGizmos()
	{
		if (soStats != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(this.transform.position, soStats.ATTACK_RANGE_DEFAULT);
		}
	}

	#endregion
}
