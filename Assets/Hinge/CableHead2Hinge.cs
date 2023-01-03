
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CableHead2Hinge : CustomJoint2DHinge
{

    public static CableHead2Hinge currentlyDragging;
    public bool isEnd;
    CustomJoint2DHinge _closeCablePart;

    CustomJoint2DHinge closeCablePart
    {
        get
        {
            if (_closeCablePart == null)
            {
                if (before != null)
                {
                    _closeCablePart = before;
                }
                else if (after != null)
                {
                    _closeCablePart = after;
                }
            }
            return _closeCablePart;
        }
    }

    public CableSlot2Hinge nearestSlot = null;
    public bool nearSlot => nearestSlot != null;

    Vector3 dir;
    public CableSlot2Hinge connectedSlot = null;
    public bool connectedToSlot => connectedSlot != null;

    public void Start()
    {

    }


    public void UpdateRotation()
    {
        if (connectedToSlot && !state.Has(JointState.Dragging))
        {
            dir = -connectedSlot.connectionPoint.transform.up;
        }
        else if (nearSlot)
        {
            dir = (nearestSlot.connectionPoint.transform.position - transform.position);
        }
        else
        {
            dir = -(closeCablePart.transform.position - transform.position);

        }

        //dir.z = originalZ;

       // transform.up = dir;
    }
    public void UpdateConnection()
    {
        if (connectedToSlot)
        {
            float sqrDistanceFromCable = (transform.position - closeCablePart.transform.position).sqrMagnitude;
            bool disconnect = false;
            if (sqrDistanceFromCable > CableSocket.unConnectDistance)
            {
                disconnect = true;
            }
            else
            {
                float sqrDistanceFromSlot = (transform.position - connectedSlot.transform.position).sqrMagnitude;
                if (sqrDistanceFromSlot > CableSocket.unConnectDistance)
                {
                    disconnect = true;
                }
            }
            if (disconnect)
            {
                Debug.Log("Disconnect");
                connectedSlot.DisconnectCableHead();

            }

        }
    }
    public void SetSlotToRotate(CableSlot2Hinge target)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        nearestSlot = target;

    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        collider2D.enabled = false;

    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        base.OnDrag(eventData);
        UpdateConnection();
        UpdateRotation();
        if (CableHead2Hinge.currentlyDragging != this)
        {
            CableHead2Hinge.currentlyDragging = this;
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        base.OnEndDrag(eventData);
        if (nearSlot && !connectedToSlot)
        {
            //calc sqe distance to nearestSlot tranform
            float sqrDistance = (nearestSlot.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance <= CableSocket.connectionDistance)
            {
                nearestSlot.ConnectCableHead(this);

            }

        }
        if (CableHead.currentlyDragging == this)
        {
            CableHead.currentlyDragging = null;
        }
        collider2D.enabled = true;

    }
}
