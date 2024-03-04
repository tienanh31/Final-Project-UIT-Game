using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Default,
    Bullet,
    Explosive,
    Melee
}

public class Damage
{
    public float value;
    public Vector3? Origin;
    public DamageType? damageType;
    public GameObject? damageSource;

    public Damage(float v, Vector3? o, DamageType? t, GameObject? d)
    {
        value = v;
        Origin = o;
        damageType = t;
        damageSource = d;
    }
}

public interface IDamageable
{
    public bool IsDead { get; }
    public float AttackPriority { get; }
    public virtual void TakenDamage(Damage damage) { }
    public float GetHP();
}
