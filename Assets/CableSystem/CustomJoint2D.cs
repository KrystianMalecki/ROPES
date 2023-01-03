using UnityEngine;
using UnityEngine.EventSystems;

public class CustomJoint2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float sqrDistance;
    public float distance;

    public CustomJoint2D before;
    public CustomJoint2D after;

    public Rigidbody2D ridgidbody;
    public static bool useForce = false;
    public JointState state = JointState.Normal;

    public int position;
    public Cable cable;

    public Collider2D collider2D;


    void Start()
    {
        ridgidbody ??= GetComponent<Rigidbody2D>();
    }

    protected Vector2 bufferDirection;
    /* public virtual void FixPositon(bool? goToBefore = null)
     {
         if (cable.isUnmovable)
         {
             return;
         }
         cable.SetPosition(position, transform.position);

         if (goToBefore == null || goToBefore == false)
         {
             FixConnectedJoint(after, false);
         }
         if (goToBefore == null || goToBefore == true)
         {
             FixConnectedJoint(before, true);
         }

     }*/
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
    protected Vector3 bufferV3 = new Vector3(0, 0, 0);
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
    /* private void FixConnectedJoint(CustomJoint2D joint, bool goToBefore)
     {
         if (joint != null)
         {
             Debug.Log(joint.state.ToFormatedString());
             if (!joint.state.HasAny(JointState.Freezed, JointState.Connected))
             {
                 bufferDirection = joint.transform.position - transform.position;

                 if (bufferDirection.sqrMagnitude > this.sqrDistance)
                 {

                     if (useForce)
                     {
                         joint.ridgidbody.AddForce(bufferDirection.normalized * (bufferDirection.sqrMagnitude - this.sqrDistance));
                     }
                     else
                     {
                         Vector3 v3 = (bufferDirection.normalized * this.sqrDistance);
                         joint.transform.position = transform.position + v3;
                     }
                 }
             }
             joint.FixPositon(goToBefore);

         }
     }*/
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
[System.Flags]
public enum JointState
{
    Normal = 1 << 1,
    Dragging = 1 << 2,
    Connected = 1 << 3,
    Freezed = 1 << 4,
}
