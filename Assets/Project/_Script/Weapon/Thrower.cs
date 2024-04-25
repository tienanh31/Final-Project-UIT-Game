using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Thrower : IWeapon
{
    #region Fields & Properties
    [SerializeField] SO_WeaponThrowerStats soStats;
    [SerializeField] Gernade gernadePrefab;
    public float throwAngleOffset = 30; //in degrees

    protected float _damage;
    protected float _attackRange;
    protected float _delayBetweenAttack;
    protected float _cooldown;
    protected float _maxRange;
    protected int _maxGernadeCount;
    protected AudioClip _throwSFX;
    protected AudioClip _deploySFX;

    protected float cooldownTimer = 0f;
    protected bool attackable = true;
    protected int currentGernadeCount;
    protected AudioSource audioSource;

    #endregion

    #region Methods
    public override void Initialize()
    {
        audioSource = GetComponent<AudioSource>();
        Type = GameConfig.WEAPON.GERNADETHROWER;
        _damage = soStats.DAMAGE_DEFAULT;
        _delayBetweenAttack = soStats.DELAY_BETWEEN_THROW;
        _cooldown = soStats.COOLDOWN;
        _maxRange = soStats.MAX_RANGE;
        _maxGernadeCount = soStats.MAX_GERNADE_COUNT;

        _throwSFX = soStats.throwSFX;
        _deploySFX = Resources.Load<AudioClip>("Audio/m4a1_deploy");

        currentGernadeCount = _maxGernadeCount;
        BulletChange?.Invoke((int)currentGernadeCount);
    }

    public override int GetCurrentBullet => (int)currentGernadeCount;

    public override void AttemptAttack()
    {
        if (currentGernadeCount > 0 && attackable)
            StartCoroutine(Attack());
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        } else
        if (currentGernadeCount < _maxGernadeCount)
        {
            currentGernadeCount++;
            BulletChange?.Invoke((int)currentGernadeCount);

            cooldownTimer = _cooldown;
        }
    }

    public override void OnSwapTo()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(_deploySFX);
    }

    protected IEnumerator Attack()
    {
        attackable = false;
        currentGernadeCount--;
        BulletChange?.Invoke((int)currentGernadeCount);

        GameObject gernade = Instantiate(gernadePrefab.gameObject, this.transform.position, new Quaternion());
        gernade.tag = this.tag;
        gernade.GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(IgnoreCollision(gernade.gameObject));

        Vector3 targetPosition;
        if (this.tag == "Player") targetPosition = Character.Instance.GetWorldMousePosition();
        else targetPosition = Character.Instance.transform.position;
        if (Vector3.Distance(this.transform.position, targetPosition) > _maxRange)
        {
            targetPosition = this.transform.position + (targetPosition - this.transform.position).normalized * _maxRange;
        }
        Throw(targetPosition, gernade.GetComponent<Gernade>());
        audioSource.Stop();
        audioSource.PlayOneShot(_throwSFX);

        yield return new WaitForSeconds(_delayBetweenAttack);
        attackable = true;
    }

    void Throw(Vector3 target, Gernade gernade)
    {
        gernade.source = this.source;
        Vector3 direction = target - gernade.transform.position;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = throwAngleOffset * Mathf.Deg2Rad;
        if (Mathf.Tan(a) == 0)
        {
            gernade.GetComponent<Rigidbody>().AddForce(_maxRange * direction.normalized, ForceMode.Impulse);
        }
        else
        {
            direction.y = distance * Mathf.Tan(a);
            distance += h / Mathf.Tan(a);

            float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            gernade.GetComponent<Rigidbody>().AddForce(velocity * direction.normalized, ForceMode.Impulse);
        }

    }

    IEnumerator IgnoreCollision(GameObject a)
    {
        Physics.IgnoreCollision(a.GetComponent<Collider>(), transform.parent.gameObject.GetComponent<Collider>(), true);
        yield return new WaitForSeconds(0.1f);
        if (a != null)
        Physics.IgnoreCollision(a.GetComponent<Collider>(), transform.parent.gameObject.GetComponent<Collider>(), false);
    }

    #endregion
}
