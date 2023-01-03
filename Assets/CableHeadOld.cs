using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CableHeadOld : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // [SerializeReference]
    // public Transform cableEnd;
    public CableOld cable;

    // public bool colliding;

    Vector3 mousePosition;
    public Joint2D realJoint;
    public Joint2D dragJoint;
    public static CableHeadOld currentlyDragging;
    private void Start()
    {
        startMass = dragJoint.attachedRigidbody.mass;

    }

    /* void Update()
     {


         mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         mousePosition.originalZ = 0;
         if (Vector3.Distance(mousePosition, points[0].position) > maxCableLenght)
         {
             Vector3 bufferDirection = (mousePosition - points[0].position).normalized;
             mousePosition = points[0].position + bufferDirection * maxCableLenght;
         }
    
         transform.position = mousePosition;
     }*/
    float startMass;
    public void OnMouseDrag()
    {
       
    }
    private void OnMouseUp()
    {
       


    }
    public void OnMouseExit()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePosition.z = 0;
        //    
        /* if (Vector3.Distance(mousePosition, cableEnd.position) > cable.distanceBetweenParts)
         {
             Vector3 bufferDirection = (mousePosition - cableEnd.position).normalized;
             mousePosition = cableEnd.position + bufferDirection * cable.distanceBetweenParts;
         }*/
        if (CableHeadOld.currentlyDragging != cable)
        {
            CableHeadOld.currentlyDragging = this;
            CableOld.currentlyDragging = cable;
            dragJoint.attachedRigidbody.mass = 100000;

        }
        transform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("b");

        if (CableHeadOld.currentlyDragging == cable)
        {
            CableHeadOld.currentlyDragging = null;
            CableOld.currentlyDragging = null;
            dragJoint.attachedRigidbody.mass = startMass;
        }
        dragJoint.attachedRigidbody.mass = startMass;
    }
}
