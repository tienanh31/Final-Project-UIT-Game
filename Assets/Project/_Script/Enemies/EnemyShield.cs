using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShield : Enemy
{
    [Header("_~* 	Shield Enemy Unique stuff")]
    [SerializeField] Vector3 _offSet;
    [SerializeField] float _skillCooldown;
    [SerializeField] float _skillDuration;

    [SerializeField] float wallHP;
    [SerializeField] Vector3 wallDimension;

    protected BulletproofWall Shield;
    protected bool canUseSkill = true;
    protected bool shieldBroken = false;

    public override void UpdateEnemy()
    {
        _patrolScope.Debug();
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

                if (canUseSkill && !Shield)
                {
                    StartCoroutine(Skill());
                }
                weapon.AttemptAttack();
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

        if (!Shield && shieldBroken)
        {
            shieldBroken = true;
            StartCoroutine(SkillCooldown());
        }

        Debug.DrawLine(transform.position, enemyAgent.destination, Color.blue);
    }

    public override void TakenDamage(Damage damage)
    {
        if (canUseSkill)
        {
            StartCoroutine(Skill());
        }
        base.TakenDamage(damage);
    }

    protected virtual IEnumerator SkillCooldown()
    {
        canUseSkill = false;
        yield return new WaitForSeconds(_skillCooldown);
        canUseSkill = true;
    }

    protected override IEnumerator Skill()
    {
        canUseSkill = false;
        Shield = BulletproofWall.Create(wallDimension, wallHP, _skillDuration, characterRigidbody.position, this.transform.rotation);
        Shield.transform.tag = this.tag;
        Shield.transform.SetParent(this.transform);
        Shield.transform.localPosition = Vector3.zero + _offSet;
        shieldBroken = false;

        yield return null;
    }
}
