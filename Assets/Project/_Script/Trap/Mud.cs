using System.Collections;
using UnityEngine;


public class Mud : Trap
{
    [SerializeField] float _slowRatio = 0.5f;
    private bool _isTargetOut = false;

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void TriggerEnter(Character character)
    {
        base.TriggerEnter(character);

        _isTargetOut = false;
        StartCoroutine(IE_Damage(character));
        character.TakenSlowEffect(_slowRatio);
        Debug.LogWarning("In");
    }

    protected override void TriggerStay()
    {
        base.TriggerStay();
    }

    protected override void TriggerExit(Character character)
    {
        base.TriggerExit(character);
        _isTargetOut = true;
        character.IsSlow = false;
        Debug.LogWarning("Out");
    }

    IEnumerator IE_Damage(Character character)
    {
        while (!_isTargetOut)
        {
            character.TakenDamage(_damage);
            yield return new WaitForSeconds(_resetTime);
        }
    }

}
