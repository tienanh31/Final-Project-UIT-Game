using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        target = DetectTarget();

        if (target != null)
        {
            Transform targetTransform = target;
            if (Vector3.Distance(transform.position, targetTransform.position)
            <= _attackRange)
            {
                //stop walking and start attacking.
                enemyAgent.SetDestination(transform.position);
                RotateWeapon(targetTransform.position);
                if (canUseSkill && !Shield)
                {
                    StartCoroutine(Skill());
                }
                weapon.AttemptAttack();
            }
            else
            {
                enemyAgent.SetDestination(targetTransform.position);
                RotateWeapon(targetTransform.position);
            }
        }
        else
        {
            //MovementBehaviour();
        }

        if (!Shield && shieldBroken)
        {
            shieldBroken = true;
            StartCoroutine(SkillCooldown());
        }
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
