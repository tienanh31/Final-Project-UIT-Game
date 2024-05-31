using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Vertex3;
}

public class PatrolScope
{
	#region Fields & Properties
	public List<Vector3> Corners = new List<Vector3>();
    public List<Triangle> Triangles;

    #endregion

    #region Methods

	public void Initialize()
	{
        if (Corners.Count >= 2)
        {
            Triangles = new List<Triangle>();
            for (int i = 2; i < Corners.Count; i++)
            {
                Triangles.Add(new Triangle()
                {
                    Vertex1 = Corners[i - 2],
                    Vertex2 = Corners[i - 1],
                    Vertex3 = Corners[i]
                });
            }
        }
	}

    //public override Vector3 GetNodePostion()
    //{
    //    return GetRandomDestination(this.Vector3.position);
    //}

    public Vector3 GetRandomDestination(Vector3 Vector3Position)
	{
        Vector3 GenVector = RandomWithinTriangle(Triangles[Random.Range(0, Triangles.Count - 1)]);

		while (Vector3.Distance(Vector3Position, GenVector) < 2f)
		{
			GenVector = RandomWithinTriangle(Triangles[Random.Range(0, Triangles.Count - 1)]);
		}
        GenVector.y = Vector3Position.y;
		return GenVector;
    }

    public bool IsPointInPolygon(Vector3 p)
    {
        for (int i = 0; i < Triangles.Count; i++)
        {
            Vector3 A = Triangles[i].Vertex1;
            Vector3 B = Triangles[i].Vertex2;
            Vector3 C = Triangles[i].Vertex3;

            double s1 = C.z - A.z;
            double s2 = C.x - A.x;
            double s3 = B.z - A.z;
            double s4 = p.z - A.z;

            double w1 = (A.x * s1 + s4 * s2 - p.x * s1) / (s3 * s2 - (B.x - A.x) * s1);
            double w2 = (s4 - w1 * s3) / s1;

            bool result = w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;

            if (result)
            {
                return result;
            }
        }

        return false;
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
        return new Vector3(result.x, Corners[0].y, result.y);
    }

    public void Debug()
    {
        if (Corners == null || Corners.Count < 2)
        {
            return;
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        for (int i = 2; i < Corners.Count; i++)
        {
            UnityEngine.Debug.DrawLine(Corners[i - 2], Corners[i - 1]);
            UnityEngine.Debug.DrawLine(Corners[i - 2], Corners[i]);
            UnityEngine.Debug.DrawLine(Corners[i - 1], Corners[i]);
        }
    }

    [ExecuteInEditMode]
    public void OnDrawGizmos()
    {
        if (Corners == null || Corners.Count < 2)
		{
            return;
		}

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        for (int i = 2; i < Corners.Count; i++)
        {
            Gizmos.DrawLine(Corners[i - 2], Corners[i - 1]);
            Gizmos.DrawLine(Corners[i - 2], Corners[i]);
            Gizmos.DrawLine(Corners[i - 1], Corners[i]);
        }
    }
    #endregion
}

//[CustomEditor(typeof(PatrolScope)), CanEditMultipleObjects]
//public class PatrolScopeEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        PatrolScope myTarget = (PatrolScope)target;

//        if (GUILayout.Button("GetAllNodes"))
//        {
//            myTarget._corners.Clear();
//            myTarget._corners.AddRange(myTarget.GetComponentsInChildren<Vector3>());
//            myTarget._corners.Remove(myTarget.Vector3);

//            myTarget._triangles = new List<Triangle>();
//            for (int i = 2; i < myTarget._corners.Count; i++)
//            {
//                myTarget._triangles.Add(new Triangle()
//                {
//                    Vertex1 = myTarget._corners[i - 2].position,
//                    Vertex2 = myTarget._corners[i - 1].position,
//                    Vertex3 = myTarget._corners[i].position
//                });
//            }
//        }

//        if (GUILayout.Button("Get Random Point"))
//        {
//            Debug.Log($"Postion: '{myTarget.GetNodePostion()}, Triangle Count: '{myTarget._triangles.Count}");
//        }

//        DrawDefaultInspector();
//    }
//}

