using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

public class Triangle
{
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Vertex3;
}

public class PatrolScope : PathNode
{
	#region Fields & Properties
	[SerializeField] public List<Transform> _corners = new List<Transform>();
    public List<Triangle> _triangles;

	#endregion

	#region Methods
	private void Awake()
	{
        if (_corners.Count >= 2)
        {
            _triangles = new List<Triangle>();
            for (int i = 2; i < _corners.Count; i++)
            {
                _triangles.Add(new Triangle()
                {
                    Vertex1 = _corners[i - 2].position,
                    Vertex2 = _corners[i - 1].position,
                    Vertex3 = _corners[i].position
                });
            }
        }
	}

    public override Vector3 GetNodePostion()
    {
        return GetRandomDestination(this.transform.position);
    }

    public Vector3 GetRandomDestination(Vector3 transformPosition)
	{
        Vector3 GenVector = RandomWithinTriangle(_triangles[Random.Range(0, _triangles.Count - 1)]);

		while (Vector3.Distance(transformPosition, GenVector) < 2f)
		{
			GenVector = RandomWithinTriangle(_triangles[Random.Range(0, _triangles.Count - 1)]);
		}
        GenVector.y = transformPosition.y;
		return GenVector;
    }

    public bool IsPointInPolygon(Vector3 point)
    {
        bool inside = false;

        int polygonLength = _corners.Count;
        int i = 0;
        // x, y for tested point.
        float pointX = point.x, pointY = point.z;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        endX = _corners[polygonLength - 1].position.x;
        endY = _corners[polygonLength - 1].position.z;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            
            endX = _corners[i++].position.x;
            endY = _corners[i++].position.z;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }

    private Vector3 RandomWithinTriangle(Triangle triangle)
    {
        var r1 = Mathf.Sqrt(Random.Range(0f, 1f));
        var r2 = Random.Range(0f, 1f);
        var m1 = 1 - r1;
        var m2 = r1 * (1 - r2);
        var m3 = r2 * r1;

        Vector2 p1;
        p1.x = triangle.Vertex1.x;
        p1.y = triangle.Vertex1.z;

        Vector2 p2;
        p2.x = triangle.Vertex2.x;
        p2.y = triangle.Vertex2.z;

        Vector2 p3;
        p3.x = triangle.Vertex3.x;
        p3.y = triangle.Vertex3.z;

        Vector2 result = (m1 * p1) + (m2 * p2) + (m3 * p3);
        return new Vector3(result.x, 1f, result.y);
    }

    [ExecuteInEditMode]
    public void OnDrawGizmos()
    {
        if (_corners == null || _corners.Count < 2)
		{
            return;
		}

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        for (int i = 2; i < _corners.Count; i++)
        {
            Gizmos.DrawLine(_corners[i - 2].position, _corners[i - 1].position);
            Gizmos.DrawLine(_corners[i - 2].position, _corners[i].position);
            Gizmos.DrawLine(_corners[i - 1].position, _corners[i].position);
        }
    }
    #endregion
}

[CustomEditor(typeof(PatrolScope)), CanEditMultipleObjects]
public class PatrolScopeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PatrolScope myTarget = (PatrolScope)target;

        if (GUILayout.Button("GetAllNodes"))
        {
            myTarget._corners.Clear();
            myTarget._corners.AddRange(myTarget.GetComponentsInChildren<Transform>());
            myTarget._corners.Remove(myTarget.transform);

            myTarget._triangles = new List<Triangle>();
            for (int i = 2; i < myTarget._corners.Count; i++)
            {
                myTarget._triangles.Add(new Triangle()
                {
                    Vertex1 = myTarget._corners[i - 2].position,
                    Vertex2 = myTarget._corners[i - 1].position,
                    Vertex3 = myTarget._corners[i].position
                });
            }
        }

        if (GUILayout.Button("Get Random Point"))
        {
            Debug.Log($"Postion: '{myTarget.GetNodePostion()}, Triangle Count: '{myTarget._triangles.Count}");
        }

        DrawDefaultInspector();
    }
}

