using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public virtual Vector3 GetNodePostion()
    {
        return this.transform.position;
    }
}
