using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyTurret : Enemy, IDamageable
{
    #region Fields & Properties
    [Header("_~* 	Turret Stats")]
    [SerializeField] protected float DetectSweepAngle;
    [SerializeField] protected float DetectAngle;
    [SerializeField] protected float _sweepSpeed;

    [SerializeField] protected LineRenderer Line1;
    [SerializeField] protected LineRenderer Line2;
    [SerializeField] protected Light flashlight;
    protected int _sweepDirection = 1;

    protected Quaternion originalRotation;
    #endregion

    #region Methods
    public override void Initialize(Path p = null)
    {
        _initialized = true;
        originalRotation = this.transform.rotation;
        this.tag = GameConfig.COLLIDABLE_OBJECT.ENEMY.ToString();
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

        weapon = GetComponentInChildren<IWeapon>();
        if (weapon)
        {
            weapon.Initialize();
            weapon.tag = this.tag;
        }

        healthbar.Start();
        flashlight.spotAngle = DetectAngle;
        flashlight.range = _detectRange;

        LevelManager.Instance.damageables.Add(this);
        LevelManager.Instance.AddEnemy(this);
    }

    public override void UpdateEnemy()
    {
        if (!isAlerted)
        {
            target = DetectTarget();
            if (target != null)
            {
                switch (alertType)
                {
                    case AlertType.SamePath:
                        AlertSamePathEnemies(target.gameObject);
                        break;
                    case AlertType.All:
                        AlertAllEnemies(target.gameObject);
                        break;
                    default:
                        AlertNearbyEnemies(target.gameObject);
                        break;
                }
                isAlerted = true;
            }
        }
        if (target != null)
        {
            Line1.gameObject.SetActive(false);
            Line2.gameObject.SetActive(false);
            if (Vector3.Distance(transform.position, target.position) <= _attackRange)
            {
                //stop walking and start attacking.
                RotateWeapon(target.position);
                weapon.AttemptAttack();
            }
            else
            {
                RotateWeapon(target.position);
            }
        }
        else
        {
            //due to inconsistent nature of Update(), I have to add _sweepDirection in to the formula
            //so when the sweep direction change, it will only change again when reach the other side 
            this.transform.Rotate(new Vector3(0, _sweepSpeed * Time.deltaTime * _sweepDirection));
            if (Quaternion.Angle(originalRotation, this.transform.rotation) > (DetectSweepAngle / 2))
            {
                _sweepDirection = -_sweepDirection;
                this.transform.Rotate(new Vector3(0, _sweepSpeed * Time.deltaTime * _sweepDirection));
            }

            Line1.gameObject.SetActive(true);
            Line2.gameObject.SetActive(true);

            Vector3 currentRotation = Quaternion.Euler(0, DetectAngle / 2, 0) *this.transform.forward;
            Line1.SetPosition(0, this.transform.position);
            Ray ray = new Ray(this.transform.position, currentRotation);
            Line1.SetPosition(1, ray.GetPoint(_detectRange));

            Line2.SetPosition(0, this.transform.position);
            currentRotation = Quaternion.Euler(0, -DetectAngle / 2, 0) * this.transform.forward;
            ray = new Ray(this.transform.position, currentRotation);
            Line2.SetPosition(1, ray.GetPoint(_detectRange));
        }
    }

    protected override Transform DetectTarget()
    {
        Transform target = null;
        float maxPriority = -9999;

        foreach (Collider c in Physics.OverlapSphere(this.transform.position, _detectRange, LayerMask.GetMask("Damageables")))
        {

            if (c.gameObject.CompareTag(this.tag)) continue;
            IDamageable damageable = c.gameObject.GetComponent<IDamageable>();
            if (damageable == null) continue;
            if (damageable.IsDead) continue;

            Transform damagableTransform = c.gameObject.transform;
            if (Vector3.Angle(this.transform.forward, damagableTransform.position - this.transform.position) > DetectAngle / 2)
                continue;

            RaycastHit[] info = Physics.RaycastAll(this.transform.position, damagableTransform.position - this.transform.position, Vector3.Distance(this.transform.position, damagableTransform.position));
            bool blocked = false;
            foreach (RaycastHit hit in info)
            {
                //theres an object blocking
                if (hit.collider.gameObject.GetComponent<IDamageable>() == null)
                {
                    blocked = true;
                    break;
                }
            }
            if (blocked) continue;

            if (Vector3.Distance(damagableTransform.position, this.transform.position) <= _detectRange)
            {
                if (damageable.AttackPriority > maxPriority)
                {
                    target = damagableTransform;
                    maxPriority = damageable.AttackPriority;
                }
            }
        }
        return target;
    }

    protected override void MovementBehaviour()
    {
        return;
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
            Gizmos.color = Color.blue;
            if (target != null)
                Gizmos.DrawLine(target.position, this.transform.position); else
                Gizmos.DrawRay(this.transform.position, this.transform.forward.normalized * soStats.DETECT_RANGE);
        }
    }
    #endregion
}
