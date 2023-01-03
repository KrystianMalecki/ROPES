
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
public class Cable2Hinge : MonoBehaviour
{
    public List<Transform> points;

    public CableHead2Hinge startHead;
    public CableHead2Hinge endHead;

    public CableSlot2Hinge startSlot;
    public CableSlot2Hinge endSlot;

    public LineRenderer lineRenderer;
    public float distanceBetweenParts = 1;
    public static Cable2Hinge currentlyDragging;
    public CustomJoint2DHinge currentlyColliding = null;
    public delegate void ChangeConnectionSlots();
    public event ChangeConnectionSlots OnChangeConnectionSlots;

    public bool isUnmovable;

    public void Start()
    {
        /* OnChangeConnectionSlots += () =>
         {
             if (startSlot != null && endSlot != null)
             {
                 Debug.Log($"Connection between {startSlot} and {endSlot}!");
             }
             else
             {
                 Debug.Log($"Connection broken!");
             }
         };*/
    }
    //todo implement better
    public void ConnectionChanged()
    {
        OnChangeConnectionSlots.Invoke();
    }
    public void SetPosition(int at, Vector3 position)
    {
        lineRenderer.SetPosition(at, position);
    }
    public void AddPart(CustomJoint2DHinge joint)
    {
        points.Add(joint.transform);
        joint.position = points.Count - 1;
    }
    public void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);

        }
    }
}
