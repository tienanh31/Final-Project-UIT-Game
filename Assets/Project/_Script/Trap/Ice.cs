using System.Collections;
using UnityEngine;


public class Ice : Trap
{
    [SerializeField] ParticleSystem _iceParticle;
    [SerializeField] float _slowRatio = 0.7f;
    [SerializeField] float _slowTime = 2f;

    public static Ice Create(Transform parent = null)
    {
        Ice ice = Instantiate<Ice>(Resources.Load<Ice>("_Prefabs/Trap/Ice"), parent);
        ice.transform.position = parent.position;

        return ice;
    }

    public void Icing(Character character)
    {
        var main = _iceParticle.main;
        main.duration = _slowTime;

        _iceParticle.Play();

        StartCoroutine(IE_Slow(character));
        character.TakenSlowEffect(_slowRatio);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    IEnumerator IE_Slow(Character character)
    {
        yield return new WaitForSeconds(_slowTime);

        Destroy(gameObject);
        character.IsSlow = false;
    }

}
