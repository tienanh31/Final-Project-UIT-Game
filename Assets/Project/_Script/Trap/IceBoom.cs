using System.Collections;
using UnityEngine;

public class IceBoom : Trap
{
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);

        character.TakenDamage(_damage);
        var ice = Ice.Create(character.transform);
        ice.Icing(character);

        Destroy(gameObject);
    }

}
