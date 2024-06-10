using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlameThrower : Trap
{
    [SerializeField] List<ParticleSystem> _flameParticles;
    [SerializeField] float _flameDuration = 6f;
    [SerializeField] float _chargingTime = 2f;

    private bool _damable = false;
    private bool _isTargetOut = true;

    private void Start()
    {
        foreach(var flameParticle in _flameParticles)
        {
            var main = flameParticle.main;
            main.duration = _flameDuration;
            main.loop = false;
        }

        StartCoroutine(IE_Flaming());
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);
        _isTargetOut = false;
        StartCoroutine(IE_Damage(character));
    }

    protected override void TriggerStay(Character character)
    {
        base.TriggerStay(character);
    }

    protected override void TriggerExit(Character character)
    {
        base.TriggerExit(character);
        _isTargetOut = true;
    }

    private IEnumerator IE_Damage(Character character)
    {
        while (!_isTargetOut)
        {
            if (_damable)
            {
                character.TakenDamage(_damage);
            }
            yield return new WaitForSeconds(_resetTime);
        }
    }

    private IEnumerator IE_Flaming()
    {
        while (true)
        {
            Debug.Log("Flaming");

            foreach (var flameParticle in _flameParticles)
            {
                flameParticle.Play();
            }

            _damable = true;
            yield return new WaitForSeconds(_flameDuration);

            _damable = false;
            yield return new WaitForSeconds(_chargingTime);
        }
    }
}
