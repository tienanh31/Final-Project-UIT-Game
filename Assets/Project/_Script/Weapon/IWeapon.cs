using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IWeapon : MonoBehaviour
{
    public GameConfig.WEAPON Type { get; protected set; }
    public virtual void Initialize() {}
    public virtual void AttemptAttack() {}
    public virtual void AttemptReload() { }
    public virtual int GetCurrentBullet => 0;
    public Action<int> BulletChange;
    public GameObject source;

    public virtual void OnSwapTo() {}

    public virtual void AddDamageBonus(float dame) { }

    public void SetType(GameConfig.WEAPON type) => Type = type;

    protected void OnDestroy()
    {
        StopAllCoroutines();
    }
}
