using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CableSlot2Hinge : MonoBehaviour
{
    public static float connectionDistance = Mathf.Pow(3f, 2);
    public static float unConnectDistance = Mathf.Pow(4f, 2);

    public CableHead2Hinge connectedHead;
    public bool connected => connectedHead != null;
    public Transform connectionPoint;
    private void OnMouseOver()
    {
        // CableHead.currentlyDragging?.SetSlotToRotate(transform);
        if (!connected)
        {
            // CableHead.currentlyDragging?.FixPositon();
            if (CableHead2Hinge.currentlyDragging?.nearestSlot == null)
            {
                CableHead2Hinge.currentlyDragging?.SetSlotToRotate(this);
            }
        }
    }
    private void OnMouseEnter()
    {
        if (!connected)
        {
            CableHead2Hinge.currentlyDragging?.SetSlotToRotate(this);
        }

    }
    private void OnMouseExit()
    {
        if (!connected)
        {
            CableHead2Hinge.currentlyDragging?.SetSlotToRotate(null);
        }
    }

    public void ConnectCableHead(CableHead2Hinge head)
    {
        connectedHead = head;
        connectedHead.connectedSlot = this;

        connectedHead.transform.position = connectionPoint.position;
        connectedHead.state = connectedHead.state.Join(JointState.Connected);
        if (connectedHead.isEnd)
        {
            connectedHead.cable.endSlot = this;
        }
        else
        {
            connectedHead.cable.startSlot = this;
        }
        connectedHead.cable.ConnectionChanged();

    }
    public void DisconnectCableHead()
    {
        connectedHead.state = connectedHead.state.Unjoin(JointState.Connected);
        if (connectedHead.nearestSlot == this)
        {
            connectedHead.nearestSlot = null;
        }
        connectedHead.connectedSlot = null;
        if (connectedHead.isEnd)
        {
            connectedHead.cable.endSlot = null;
        }
        else
        {
            connectedHead.cable.startSlot = null;
        }
        connectedHead.cable.ConnectionChanged();
        connectedHead = null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(connectionPoint.transform.position, Mathf.Sqrt(connectionDistance));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(connectionPoint.transform.position, Mathf.Sqrt(unConnectDistance));
    }
}