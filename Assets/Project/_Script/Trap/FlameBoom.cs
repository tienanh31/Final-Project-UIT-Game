using System.Collections;
using UnityEngine;

public class FlameBoom : Trap
{

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);

        character.TakenDamage(_damage);
        var flameExplosion = Flame.Create(character.transform);
        flameExplosion.Explosion(character);

        Destroy(gameObject);
    }

    protected override void TriggerStay(Character character)
    {
        base.TriggerStay(character);
    }

    protected override void TriggerExit(Character character)
    {
        base.TriggerExit(character);
    }

}
