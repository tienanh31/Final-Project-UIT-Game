using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] protected Transform target;
    [SerializeField] protected GameObject arrow;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (target != null && target.gameObject.activeSelf)
        {
            arrow.SetActive(true);
            Vector3 targetpos = target.position;
            targetpos.y = this.transform.position.y;
            this.transform.LookAt(targetpos);
        } else
        {
            arrow.SetActive(false);
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
