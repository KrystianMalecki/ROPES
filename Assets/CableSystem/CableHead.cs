using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CableHead : CustomJoint2D
{



    public static CableHead currentlyDragging;
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

    public void Start()
    {

    }
    /* public override void FixPositon(bool? fixBefore = null)
     {
         if (cable.isUnmovable)
         {
             return;
         }

         base.FixPositon(fixBefore);


         if (connectedToSlot)
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

         dir.originalZ = 0;

         transform.up = dir;

         if (connectedToSlot)
         {
             float sqrDistance = (transform.position - closeCablePart.transform.position).sqrMagnitude;

             if (sqrDistance > CableSocket.unConnectDistance)
             {
                 connectedSlot.DisconnectCableHead();
             }

             // closeCablePart?.FixPositon(!fixBefore);
         }
     }*/
    public override void NewFixPositon(CustomJoint2D from)
    {
        if (cable.isUnmovable)
        {
            return;
        }

        base.NewFixPositon(from);

        UpdateConnection();
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

        transform.up = dir;
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
                NewFixPositon(this);

            }

            // closeCablePart?.FixPositon(!fixBefore);
        }
    }
    public void Update()
    {
        if (connectedToSlot)
        {
            NewFixPositon(this);

            /* if (closeCablePart == after)
             {
                 Debug.Log("updating1");

                 closeCablePart?.FixPositon(true);
                 NewFixPositon(this);

             }
             else if (closeCablePart == before)
             {
                 Debug.Log("updating2");

                 closeCablePart?.FixPositon(false);
             }*/
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
        if (CableHead.currentlyDragging != this)
        {
            CableHead.currentlyDragging = this;
        }
        collider2D.enabled = false;
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
                // FixPositon();
                NewFixPositon(this);

            }

        }
        if (CableHead.currentlyDragging == this)
        {
            CableHead.currentlyDragging = null;
        }
        collider2D.enabled = true;

    }
}
