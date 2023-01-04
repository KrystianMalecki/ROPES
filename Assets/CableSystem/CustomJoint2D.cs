using UnityEngine;
using UnityEngine.EventSystems;

public class CustomJoint2D : MonoBehaviour
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




    Vector2 bufferDirection;

    public virtual void NewFixPositon(CustomJoint2D from, bool forceAllFix = false)
    {
        if (cable.isUnmovable)
        {
            return;
        }

        cable.SetPosition(position, transform.position);

        if (after != from)
        {
            NewFixConnectedJoint(after, forceAllFix);
        }
        if (before != from)
        {
            NewFixConnectedJoint(before, forceAllFix);
        }
    }
    Vector3 bufferV3 = new Vector3(0, 0, 0);
    protected float originalZ = 0;
    private void NewFixConnectedJoint(CustomJoint2D joint, bool forceAllFix = false)
    {
        if (joint != null)
        {
            //  Debug.Log(joint.state.ToFormatedString());
            if (!joint.state.Has(JointState.Freezed))
            {
                if (joint.state.Has(JointState.Connected))
                {
                    joint.NewFixPositon(null);
                    return;
                }
                bufferDirection = joint.transform.position - transform.position;

                if (bufferDirection.sqrMagnitude > this.sqrDistance)
                {


                    bufferV3 = bufferDirection.normalized;
                    bufferV3 *= this.distance;
                    bufferV3 += transform.position;
                    bufferV3.z = originalZ;

                    joint.transform.position = bufferV3;
                    joint.NewFixPositon(this);


                }
                else if (forceAllFix) //even when joint hasn't updated BUT forceAllFix is true then still update down the chain
                {
                    joint.NewFixPositon(this);

                }
            }
        }
    }

    public void InitAndAdd(CustomJoint2D before, float distance, Cable cable)
    {
        this.before = before;
        this.sqrDistance = distance * distance;
        this.cable = cable;
        this.distance = distance;
        cable.AddPart(this);
        cable.SetPosition(position, transform.position);
        originalZ = transform.position.z;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!cable.isUnmovable && (cable.currentlyColliding == null) && collision.collider.transform.parent != transform.parent)
        {
            cable.currentlyColliding ??= this;

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (!cable.isUnmovable && (cable.currentlyColliding == null || cable.currentlyColliding.Equals(this)) && collision.collider.transform.parent != transform.parent)
        {
            //  cable.currentlyColliding ??= this;
            // FixPositon();
            NewFixPositon(this, true);

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (cable.currentlyColliding == this)
        {
            cable.currentlyColliding = null;
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
