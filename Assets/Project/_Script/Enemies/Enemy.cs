using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum AlertType
{
	Nearby,
	SamePath,
	All
}

public class Enemy : MonoBehaviour, IDamageable
{
	#region Fields & Properties
	[Header("_~* 	Prefabs, Weapons & Stats")]
	[SerializeField] protected Rigidbody characterRigidbody;
	[SerializeField] protected IWeapon weapon;
	[SerializeField] protected SO_EnemyDefault soStats;
	[SerializeField] public Path path;
	[SerializeField] protected float stoppingDistance = 1f;
	[SerializeField] public bool deleteUponDeath = true;
	public bool IsDead { get; protected set; }
	public float AttackPriority { get; protected set; }

	protected NavMeshAgent enemyAgent;
	protected int currentPosition = 0;
	public Vector3 CurrentDestination { get; set; } = Vector3.zero;

	public bool IsInPatrolScope { get; set; }

	[Header("_~* 	Movement & control")]
	[SerializeField] protected Animator _animator;

	protected float _moveSpeed;
	protected float _HP;
	public bool HP { get { return HP; } }
	protected float _detectRange;
	protected float _attackRange;
	protected float _turningSpeed;

	protected bool _patrolable = true;
	public bool _initialized = false;
	//protected internal bool isAlerted = false;

	protected Transform target;
	protected Healthbar healthbar;

	[Header("_~* 	Other ")]
	//[SerializeField] protected AlertType alertType = AlertType.Nearby;
	protected bool movementBehaviour = true;

	[Header("_~* 	Events ")]
	public UnityEvent<Enemy> OnDeathEvent;

	protected PatrolScope _patrolScope;

	#endregion

	#region Methods
	public virtual void Initialize(PatrolScope patrolScope = null)
	{
		_initialized = true;
		this.tag = GameConfig.COLLIDABLE_OBJECT.ENEMY.ToString();
		enemyAgent = GetComponent<NavMeshAgent>();
		enemyAgent.Warp(transform.position);

		characterRigidbody = GetComponent<Rigidbody>();
		OnDeathEvent = new UnityEvent<Enemy>();
		healthbar = GetComponentInChildren<Healthbar>();

		//SO_EnemyDefault stats = (SO_EnemyDefault)LevelManager.Instance.GetStats(GameConfig.SO_TYPE.ENEMY, (int)GameConfig.ENEMY.ENEMY_DEFAULT);
		//SO_EnemyDefault stats = LevelManager.Instance.GetStats(this);
		_moveSpeed = soStats.MOVE_SPEED_DEFAULT;
		_HP = soStats.HP_DEFAULT;
		_detectRange = soStats.DETECT_RANGE;
		_attackRange = soStats.ATTACK_RANGE_DEFAULT;
		_turningSpeed = soStats.TURNING_SPEED;

		enemyAgent.speed = _moveSpeed;
		enemyAgent.angularSpeed = _turningSpeed;
		enemyAgent.acceleration = _moveSpeed;
		enemyAgent.stoppingDistance = 1f;

		if (patrolScope != null)
		{
			_patrolScope = patrolScope;
		}

		weapon = GetComponentInChildren<IWeapon>();
		if (weapon)
		{
			weapon.Initialize();
			weapon.tag = this.tag;
			weapon.source = gameObject;
		}

		healthbar.Start();
		LevelManager.Instance.damageables.Add(this);
		LevelManager.Instance.AddEnemy(this);
		if (movementBehaviour)
		{
			//CurrentDestination = path.GetNodePosition(currentPosition);
			//enemyAgent.SetDestination(CurrentDestination);
		}

		var statBonus = GameManager.Instance.EnemiesBonusStat;
		_HP += statBonus.HP;
		_moveSpeed += statBonus.MOVE_SPEED;
		weapon.AddDamageBonus(statBonus.ATTACK_BONUS);

	}

	public virtual void UpdateEnemy()
	{
		if (IsDead)
        {
			return;
        }

		_patrolScope.Debug();

		_animator.SetInteger("State", 1);

		if (target != null)
		{
			var player = target.GetComponent<Character>();
			player.IsInPatrolScope = _patrolScope.IsPointInPolygon(player.transform.position);

			float distance = Vector3.Distance(transform.position, target.position);
			if (distance <= _attackRange)
			{
				//stop walking and start attacking.
				enemyAgent.SetDestination(transform.position);
				RotateWeapon(target.position);
				weapon.AttemptAttack();
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

		if (target != null)
        {
			//Debug.Log($"Distance: {Vector3.Distance(transform.position, target.position)}");
			Debug.DrawLine(transform.position, enemyAgent.destination, Color.blue);
		}
	}

	public virtual void TakenDamage(Damage damage)
	{
		if (IsDead)
		{
			return;
		}

		if (damage != null && damage.damageSource != null)
		{
			target = damage.damageSource?.transform;
		}			

		Debug.LogWarning($"Source damage: {target}");

		if (_HP > 0)
		{
			_HP -= damage.value;
			healthbar.HealthUpdate();
			//Debug.Log($"Enemy hp: {_HP}");
			if (_HP <= 0)
			{
				IsDead = true;
				if (!deleteUponDeath)
				{
					if (damage.Origin != null)
					{
						Vector3 damageDirection = (Vector3)(damage.Origin - this.transform.position);

						enemyAgent.enabled = false;
						characterRigidbody.isKinematic = false;
						characterRigidbody.freezeRotation = false;
						characterRigidbody.AddForce(damageDirection.normalized * 5f, ForceMode.Impulse);
					}
				}
			}
		}
	}

	public virtual void OnDeath(Vector3? DamageDirection = null, float punch = 0.0f)
	{
		//Debug.Log("Enemy die");
		//AlertNearbyEnemies(null);
		LevelManager.Instance.damageables.Remove(this);
		OnDeathEvent?.Invoke(this);
		OnDeathEvent?.RemoveAllListeners();
		if (deleteUponDeath)
		{
			float ratio = UnityEngine.Random.Range(0f, 1f);
			if (ratio < GameConfig.RATIO_DROP_BUFF)
			{
				Item.CreateBuff(transform.position, GameConfig.BUFF.HP, 10f);
			}
			else if (ratio < GameConfig.RATIO_DROP_ITEM)
			{
				Item.CreateItem(transform.position, weapon);
			}

			_animator.SetInteger("State", 3);

			if (this.GetType() == typeof(SuicideAttacker))
            {
				Destroy(gameObject);
				return;
            }
			Destroy(gameObject, 1f);
		}
	}

	protected virtual Transform DetectTarget()
	{
		Transform target = null;

		var player = LevelManager.Instance.MyCharacter;
		if (Vector3.Distance(transform.position, player.transform.position) <= _detectRange)
        {
			target = player.transform;
			player.IsInPatrolScope = _patrolScope.IsPointInPolygon(player.transform.position);
			Debug.Log("Detect target");
        }
		//float maxPriority = -9999;

		//foreach (Collider c in Physics.OverlapSphere(this.transform.position, _detectRange, LayerMask.GetMask("Damageables")))
		//{

		//	if (c.gameObject.CompareTag(this.tag)) continue;
		//	IDamageable damageable = c.gameObject.GetComponent<IDamageable>();
		//	if (damageable == null) continue;
		//	if (damageable.IsDead) continue;

		//	Transform damagableTransform = c.gameObject.transform;
		//	RaycastHit[] info = Physics.RaycastAll(this.transform.position, damagableTransform.position - this.transform.position, Vector3.Distance(this.transform.position, damagableTransform.position));
		//	bool blocked = false;
		//	foreach (RaycastHit hit in info)
		//	{
		//		//theres an object blocking
		//		if (hit.collider.gameObject.GetComponent<IDamageable>() == null)
		//		{
		//			blocked = true;
		//			break;
		//		}
		//	}
		//	if (blocked) continue;

		//	if (Vector3.Distance(damagableTransform.position, this.transform.position) <= _detectRange)
		//	{
		//		if (damageable.AttackPriority > maxPriority)
		//		{
		//			target = damagableTransform;
		//			maxPriority = damageable.AttackPriority;
		//		}
		//	}
		//}
		return target;
	}

	protected void RotateWeapon(Vector3 location)
	{
		var q = Quaternion.LookRotation(location - transform.position);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, q, _turningSpeed * Time.deltaTime);
	}

	protected virtual void MovementBehaviour()
	{
		if (enemyAgent == null)
		{
			return;
		}
		if (!_patrolable)
		{
			return;
		}

		if (target != null)
        {
			return;
        }

		if (CurrentDestination == Vector3.zero)
		{
			CurrentDestination = _patrolScope.GetRandomDestination(transform.position);
		}

		enemyAgent.SetDestination(CurrentDestination);

		if (enemyAgent.remainingDistance < 1f)
		{
			StartCoroutine(IE_StopAwhile());
			CurrentDestination = _patrolScope.GetRandomDestination(enemyAgent.destination);
		}
	}

	protected IEnumerator IE_StopAwhile()
	{
		_patrolable = false;
		_animator.SetInteger("State", 0);
		yield return new WaitForSeconds(GameConfig.TIME_STOP_AFTER_PATROLLING);
		_patrolable = true;
	}

	//protected virtual void MovementBehaviour()
	//{
	//	if (enemyAgent.remainingDistance <= stoppingDistance)
	//	{
	//		if (path.GetNode(currentPosition).GetType() == typeof(PatrolScope))
	//		{
	//			if (_patrolable)
	//			{
	//				//Debug.Log("Destination reached, starting patrol at " + CurrentDestination.ToString());
	//				StartCoroutine(IE_Patrol());
	//			}
	//		} 
	//		else
	//		{
	//			currentPosition++;
	//			if (currentPosition >= path.NodeCount())
	//				currentPosition = 0;
	//			CurrentDestination = path.GetNodePosition(currentPosition);
	//			enemyAgent.SetDestination(CurrentDestination);
	//		}
	//	}
	//}


	protected virtual IEnumerator Skill()
	{
		yield return null;
	}

	public virtual float GetHP()
	{
		return _HP;
	}

    private void OnDestroy()
    {
		
    }

    [ExecuteInEditMode]
	private void OnDrawGizmos()
	{
		if (soStats != null && Selection.Contains(gameObject))
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, soStats.DETECT_RANGE);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, soStats.ATTACK_RANGE_DEFAULT);
		}
	}
	#endregion
}

