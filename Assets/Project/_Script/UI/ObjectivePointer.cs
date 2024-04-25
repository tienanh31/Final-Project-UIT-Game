using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectivePointer : Pointer
{
    // Update is called once per frame
    protected override void Update()
    {
        float distance = Mathf.Infinity;
        GameObject[] objectives = GameObject.FindGameObjectsWithTag("OBJECTIVE");
        foreach (GameObject objective in objectives)
        {
            if (objective.activeSelf && Vector3.Distance(this.gameObject.transform.position, objective.transform.position) < distance)
            {
                distance = Vector3.Distance(this.gameObject.transform.position, objective.transform.position);
                target = objective.transform;
            }
        }

        base.Update();
    }
}
