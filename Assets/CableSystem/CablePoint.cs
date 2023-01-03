using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePoint : CustomJoint2D
{




    public JointState state = JointState.Normal;

    public int position;
    public Cable cable;





    public virtual void NewFixPositon(CustomJoint2D from)
    {
        if (cable.isUnmovable)
        {
            return;
        }

        cable.SetPosition(position, transform.position);

        if (after != from)
        {
            NewFixConnectedJoint(after);
        }
        if (before != from)
        {
            NewFixConnectedJoint(before);
        }
    }
    Vector3 bufferV3 = new Vector3(0, 0, 0);
    protected float originalZ = 0;
    private void NewFixConnectedJoint(CustomJoint2D joint)
    {
        if (joint != null)
        {
            //  Debug.Log(joint.state.ToFormatedString());
            if (!joint.state.HasAny(JointState.Freezed, JointState.Connected))
            {
                bufferDirection = joint.transform.position - transform.position;

                if (bufferDirection.sqrMagnitude > this.sqrDistance)
                {


                    bufferV3 = bufferDirection.normalized;
                    bufferV3 *= this.distance;
                    bufferV3 += transform.position;
                    bufferV3.z = originalZ;

                    joint.transform.position = bufferV3;

                }
            }
            joint.NewFixPositon(this);

        }
    }

    public void InitAndAdd(CustomJoint2D before, float distance, Cable cable)
    {
        this.before = before;
        this.sqrDistance = distance * distance;
        this.cable = cable;
        this.distance = distance;
        cable.AddPart(this);
        originalZ = transform.position.z;
    }

    void OnCollisionStay2D(Collision2D collision)
    {

        if (!cable.isUnmovable && (cable.currentlyColliding == null || cable.currentlyColliding.Equals(this)) && collision.collider.transform.parent != transform.parent)
        {
            cable.currentlyColliding ??= this;
            // FixPositon();
            NewFixPositon(this);

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (cable.currentlyColliding == this)
        {
            cable.currentlyColliding = null;
        }

    }


    Vector3 mousePosition;
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        state = state.Join(JointState.Dragging);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = originalZ;
        if (Cable.currentlyDragging != cable)
        {
            Cable.currentlyDragging = cable;
        }
        transform.position = mousePosition;
        NewFixPositon(this);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        state = state.Unjoin(JointState.Dragging);

        if (Cable.currentlyDragging == cable)
        {
            Cable.currentlyDragging = null;
        }
    }


}
