
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomJoint2DHinge : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float sqrDistance;
    public float distance;

    public CustomJoint2DHinge before;
    public CustomJoint2DHinge after;

    public Rigidbody2D ridgidbody;
    public HingeJoint2D hingeJoint2D;

    public JointState state = JointState.Normal;

    public int position;
    public Cable2Hinge cable;

    public Collider2D collider2D;


    void Start()
    {
        ridgidbody ??= GetComponent<Rigidbody2D>();
        hingeJoint2D ??= GetComponent<HingeJoint2D>();
    }


    protected float originalZ = 0;

    public void InitAndAdd(CustomJoint2DHinge before, float distance, Cable2Hinge cable)
    {
        this.before = before;
        this.sqrDistance = distance * distance;
        this.cable = cable;
        this.distance = distance;
        cable.AddPart(this);
        originalZ = transform.position.z;
        if (before != null)
        {
            hingeJoint2D.enabled = true;

            hingeJoint2D.connectedBody = before.ridgidbody;
            hingeJoint2D.connectedAnchor = before.transform.position - transform.position;
        }
        else
        {
            hingeJoint2D.enabled = false;
        }
    }





    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        state = state.Join(JointState.Dragging);
        ridgidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        ridgidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    Vector3 mousePosition;
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = originalZ;
        if (Cable2Hinge.currentlyDragging != cable)
        {
            Cable2Hinge.currentlyDragging = cable;
        }
        transform.position = mousePosition;

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        state = state.Unjoin(JointState.Dragging);

        if (Cable2Hinge.currentlyDragging == cable)
        {
            Cable2Hinge.currentlyDragging = null;
        }
        ridgidbody.constraints = RigidbodyConstraints2D.None;
        ridgidbody.bodyType = RigidbodyType2D.Dynamic;

    }


}
