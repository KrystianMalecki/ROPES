using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CableHead : CablePoint
{



    public static CableHead currentlyDraggedHead;
    public bool isEnd;
    CustomJoint2D _closeCablePart;

    CustomJoint2D closeCablePart
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

    public CableSocket nearestSlot = null;
    public bool nearSlot => nearestSlot != null;

    Vector3 dir;
    public CableSocket connectedSlot = null;
    public bool connectedToSlot => connectedSlot != null;


    public override void NewFixPositon(CustomJoint2D from, bool forceAllFix = false)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        UpdateConnection();
        base.NewFixPositon(from, forceAllFix);


        UpdateRotation();


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

        dir.z = originalZ;

        //   transform.up = dir;
        //2d look at
        transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90);
    }
    public void UpdateConnection()
    {
        if (connectedToSlot)
        {
            float sqrDistanceFromCable = Vector2.SqrMagnitude(transform.position - closeCablePart.transform.position);
            bool disconnect = false;
            if (sqrDistanceFromCable > nearestSlot.disconnectSqrDistance)
            {
                disconnect = true;
            }
            else
            {
                float sqrDistanceFromSlot = Vector2.SqrMagnitude(transform.position - closeCablePart.transform.position);

                if (sqrDistanceFromSlot > nearestSlot.disconnectSqrDistance)
                {
                    disconnect = true;
                }
            }
            if (disconnect)
            {
                connectedSlot.DisconnectCableHead();
                NewFixPositon(this);

            }

        }
    }
    public void Update()
    {
        if (connectedToSlot)
        {
          //  NewFixPositon(this);
        }
    }
    public void SetSlotToRotate(CableSocket target)
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
        if (currentlyDraggedHead != this)
        {
            currentlyDraggedHead = this;
        }
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

    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        base.OnEndDrag(eventData);
        TryToConnect();
        if (currentlyDraggedHead == this)
        {
            currentlyDraggedHead = null;
        }
        collider2D.enabled = true;
    }
    public void TryToConnect()
    {
        if (nearSlot && !connectedToSlot)
        {
            float sqrDistance = (nearestSlot.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance <= nearestSlot.connectSqrDistance)
            {
                nearestSlot.ConnectCableHead(this);
                NewFixPositon(this);
            }

        }
    }
}
