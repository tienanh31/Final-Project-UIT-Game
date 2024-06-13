using System.Collections;
using UnityEngine;

public class FlameBoom : Trap
{

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void CollisionEnter(Character character)
    {
        base.CollisionEnter(character);

        character.TakenDamage(_damage);
        var flameExplosion = Flame.Create(character.transform);
        flameExplosion.Explosion(character);

        Destroy(gameObject);
    }
}
