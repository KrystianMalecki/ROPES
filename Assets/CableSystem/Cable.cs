using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
public class Cable : MonoBehaviour
{
    public List<Transform> points;

    public CableHead startHead;
    public CableHead endHead;

    public CableSocket startSlot;
    public CableSocket endSlot;

    public LineRenderer cableRenderer;
    public float distanceBetweenParts = 1;
    public static Cable currentlyDragging;
    public CustomJoint2D currentlyColliding = null;
    public delegate void ChangeConnectionSlots();
    public event ChangeConnectionSlots OnChangeConnectionSlots;

    public bool isUnmovable;

    public void Start()
    {
        OnChangeConnectionSlots += () =>
        {
            if (startSlot != null && endSlot != null)
            {
                Debug.Log($"Connection between {startSlot} and {endSlot}!");
            }
            else
            {
                Debug.Log($"Connection broken!");
            }
        };
    }
    //todo implement better
    public void ConnectionChanged()
    {
        OnChangeConnectionSlots.Invoke();
    }
    public void SetPosition(int at, Vector3 position)
    {
        cableRenderer.SetPosition(at, position);
    }
    public void AddPart(CustomJoint2D joint)
    {
        points.Add(joint.transform);
        joint.position = points.Count - 1;
    }
}
