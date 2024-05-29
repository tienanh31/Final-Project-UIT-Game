using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gun: IWeapon
{
	#region Fields & Properties
	[SerializeField] protected SO_WeaponGunStats soStats;
	[SerializeField] protected Bullet bulletPrefab;

	protected float _damage;
	protected float _attackRange;
	protected float _attackSpeed;
	protected float _reloadTime;
	protected int _magazineCapacity;
	protected float _inaccuracy;
	protected float _bulletSpeed;
	protected AudioClip _shootSFX;
	protected AudioClip _reloadSFX;
	protected AudioClip _doneReloadSFX;
	protected AudioClip _deploySFX;

	protected bool attackable = true;
	protected bool isReloading = false;
	protected float currentBulletQuantity;
	protected float delayBetweenShots;

	protected Character character;
	protected AudioSource audioSource;

	#endregion

	#region Methods
	public override void Initialize()
	{
		character = GetComponentInParent<Character>();
		
		Type = GameConfig.WEAPON.RIFLE;
		_damage = soStats.DAMAGE_DEFAULT;
		_attackRange = soStats.ATTACK_RANGE_DEFAULT;
		_attackSpeed = soStats.ATTACK_SPEED_DEFAULT;
		_bulletSpeed = soStats.BULLET_SPEED;
		_inaccuracy = soStats.INACCURACY;
		_magazineCapacity = soStats.MAGAZINE_CAPACITY;
		_reloadTime = soStats.RELOAD_TIME;

		_shootSFX = soStats.shootSFX;
		_reloadSFX = soStats.reloadSFX;
		_doneReloadSFX = soStats.doneReloadSFX;
		_deploySFX = Resources.Load<AudioClip>("Audio/m4a1_deploy");

		currentBulletQuantity = _magazineCapacity;
		delayBetweenShots = 60f / _attackSpeed; //real guns use RPM (Rounds per minute) to calculate how fast they shoot
		audioSource = GetComponent<AudioSource>();
	}

    public override void AddDamageBonus(float dame)
    {
        base.AddDamageBonus(dame);

		_damage += dame;
    }

    void Start()
	{
		BulletChange?.Invoke((int)currentBulletQuantity);
	}

    public override void AttemptAttack()
    {
        if (currentBulletQuantity > 0 && attackable && !isReloading)
		{
			StartCoroutine(Attack());
		} else if (currentBulletQuantity <= 0)
		{
			AttemptReload();
		}
    }

	public override void AttemptReload()
	{
		if (currentBulletQuantity < _magazineCapacity && !isReloading)
		{
			StartCoroutine(IE_Reload());
		}
	}

	protected virtual IEnumerator Attack()
	{
		attackable = false;
		// spawn bullet
		Vector3 direction = (transform.forward).normalized;
		Vector3 target = direction * 10f + Vector3.Cross(direction, transform.up).normalized * UnityEngine.Random.Range(-_inaccuracy, _inaccuracy);
		direction = (target).normalized;

		Bullet bullet = Instantiate(bulletPrefab, transform.position, new Quaternion());
		bullet.Initialize(_damage, _attackRange, _bulletSpeed, direction);
		bullet.tag = this.tag;
		bullet.source = this.source;

		audioSource.Stop();
		audioSource.PlayOneShot(_shootSFX);

		currentBulletQuantity -= 1;
		BulletChange?.Invoke((int)currentBulletQuantity);
		if (currentBulletQuantity == 0 && !isReloading)
		{
			AttemptReload();
		} else
			yield return new WaitForSeconds(delayBetweenShots);
		attackable = true;
	}

	protected IEnumerator IE_Reload()
	{
		isReloading = true;
		audioSource.PlayOneShot(_reloadSFX);
		if (character)
			character.SetWorldText("Reloading...", _reloadTime);

		yield return new WaitForSeconds(_reloadTime);
		currentBulletQuantity = _magazineCapacity;
		BulletChange?.Invoke((int)currentBulletQuantity);
		isReloading = false;
		audioSource.Stop();
		audioSource.PlayOneShot(_doneReloadSFX);
	}

	public override void OnSwapTo()
	{
		audioSource.Stop();
		audioSource.PlayOneShot(_deploySFX);
	}

	public override int GetCurrentBullet => (int)currentBulletQuantity;

	[ExecuteInEditMode]
	private void OnDrawGizmos()
	{
		if (soStats != null)
		{
			if (this.tag == "Player")
				Gizmos.color = Color.blue;
			else
				Gizmos.color = Color.red;

			Vector3 direction = (transform.forward).normalized;
			Vector3 target1 = direction * soStats.ATTACK_RANGE_DEFAULT + Vector3.Cross(direction, transform.up).normalized * -soStats.INACCURACY;
			Vector3 target2 = direction * soStats.ATTACK_RANGE_DEFAULT + Vector3.Cross(direction, transform.up).normalized * soStats.INACCURACY;
			Gizmos.DrawLine(this.transform.position, this.transform.position + target1);
			Gizmos.DrawLine(this.transform.position, this.transform.position + target2);
		}
	}

	#endregion
}

