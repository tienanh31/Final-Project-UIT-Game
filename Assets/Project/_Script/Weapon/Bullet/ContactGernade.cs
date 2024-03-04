using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContactGernade : Gernade
{
    protected override void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
