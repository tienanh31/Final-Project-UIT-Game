using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path : MonoBehaviour
{
    [SerializeField]
    protected List<PathNode> pathNodes;

    public Vector3 GetNodePosition(int index)
    {
        if (index >= pathNodes.Count || index < 0)
        {
            return Vector3.zero;
        }
        else
        {
            return pathNodes[index].GetNodePostion();
        }
    }

    public PathNode GetNode(int index)
    {
        if (index >= pathNodes.Count || index < 0)
        {
            return null;
        }
        else
        {
            return pathNodes[index];
        }
    }

    public int NodeCount()
    {
        return pathNodes.Count;
    }

    public void Clear()
    {
        pathNodes.Clear();
    }   

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (pathNodes == null)
            return;

        Gizmos.color = Color.red;
        foreach (var node in pathNodes)
        {
            if (node != null || !(bool)(this.GetType() == typeof(PatrolScope)))
                Gizmos.DrawSphere(node.transform.position, 0.25f);
        }
        for (int i = 1; i < pathNodes.Count; i++)
        {
            if (pathNodes[i] != null && pathNodes[i-1] != null)
                Gizmos.DrawLine(pathNodes[i].transform.position, pathNodes[i - 1].transform.position);
        }
    }
}

