using System.Collections;
using UnityEngine;


public class Flame : Trap
{
    [SerializeField] ParticleSystem _flameParticle;

    public static Flame Create(Transform parent = null)
    {
        Flame flame = Instantiate<Flame>(Resources.Load<Flame>("_Prefabs/Trap/Flame"), parent);
        flame.transform.position = parent.position;

        return flame;
    }

    public void Explosion(Character character)
    {
        Debug.Log("Flame");
        var main = _flameParticle.main;
        main.duration = _resetTime * 3f;

        _flameParticle.Play();

        StartCoroutine(IE_Damage(character));
        Destroy(gameObject, _resetTime * 3);
    }

    public override void Initialize()
    {
        base.Initialize();

    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);
    }

    protected override void TriggerStay(Character character)
    {
        base.TriggerStay(character);
    }

    protected override void TriggerExit(Character character)
    {
        base.TriggerExit(character);
    }

    private IEnumerator IE_Damage(Character character)
    {
        while (true)
        {
            character.TakenDamage(_damage);
            yield return new WaitForSeconds(_resetTime);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Flame destroy");
    }
}
