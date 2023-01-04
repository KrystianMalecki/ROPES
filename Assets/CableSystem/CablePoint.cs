
using UnityEngine;
using UnityEngine.EventSystems;

public class CablePoint : CustomJoint2D, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static CablePoint currentlyDraggedPoint;

    Vector3 mousePosition;
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        state = state.Join(JointState.Dragging);
        if (currentlyDraggedPoint != this)
        {
            currentlyDraggedPoint = this;
        }
        collider2D.enabled = false;

    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (cable.isUnmovable)
        {
            return;
        }
        mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = originalZ;
        if (Cable.currentlyDraggedCable != cable)
        {
            Cable.currentlyDraggedCable = cable;
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

        if (currentlyDraggedPoint == this)
        {
            currentlyDraggedPoint = null;
        }
    }
}
