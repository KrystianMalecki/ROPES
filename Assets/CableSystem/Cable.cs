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
    public static Cable currentlyDraggedCable;
    public CustomJoint2D currentlyColliding = null;


    public delegate void ConnectHandler(CableSocket to);
    /* public event ConnectHandler OnConnectEvent;

     public event ConnectHandler OnDisconnectEvent;*/

    public bool isUnmovable;

    public void Start()
    {

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
    public void OnConnect()
    {
        if (startSlot != null && endSlot != null)
        {
            startSlot.OnConnectEvent.Invoke(this, endSlot);
            endSlot.OnConnectEvent.Invoke(this, startSlot);
        }
    }
    public void OnDisconnect()
    {
        startSlot?.OnDisconnectEvent.Invoke();
        endSlot?.OnDisconnectEvent.Invoke();

    }

}
