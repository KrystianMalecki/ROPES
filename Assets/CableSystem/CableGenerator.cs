using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class CableGenerator : MonoBehaviour
{
    public GameObject cablePrefab;
    public GameObject cablePartPrefab;
    public int lengthToMake;
    public float distance;

    public GameObject MakeCable(int length, float distanceBetween)
    {
        GameObject cableObject = Instantiate(cablePrefab, transform.position, Quaternion.identity);
        Cable cable = cableObject.GetComponent<Cable>();

        CustomJoint2D last = cable.startHead;
        cable.startHead.InitAndAdd(null, distanceBetween, cable);

        int lineLenght = 1;
        int counterToMax = 0;
        int currentPointInLine = 0;
        const int max = 2;
        Vector2 direction = Vector2.right;
        cable.cableRenderer.positionCount = length + 2;

        for (int i = 0; i < length; i++)
        {
            GameObject cablePart = Instantiate(cablePartPrefab, transform.position, Quaternion.identity);
            cablePart.transform.parent = cableObject.transform;
            cablePart.transform.position = last.transform.position + (Vector3)direction * distanceBetween;
            currentPointInLine++;

            if (currentPointInLine >= lineLenght)
            {
                currentPointInLine = 0;
                counterToMax++;
                direction = Quaternion.Euler(0, 0, 90) * direction;
            }
            if (counterToMax >= max)
            {
                counterToMax = 0;
                lineLenght++;
            }
            cablePart.name = "part " + i;


            CustomJoint2D currentJoint = cablePart.GetComponent<CustomJoint2D>();
            currentJoint.InitAndAdd(last, distanceBetween, cable);

            last.after = currentJoint;
            last = currentJoint;
            cable.SetPosition(currentJoint.position, currentJoint.transform.position);

        }
        cable.endHead.InitAndAdd(last, distanceBetween, cable);
        cable.endHead.transform.position = last.transform.position + (Vector3)direction * distanceBetween;

        last.after = cable.endHead;


        return cableObject;
    }
    Vector3 pos = Vector3.zero;
    bool s = false;
    CustomJoint2D joint = null;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {

            Debug.Log("loop");

                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                var hits = Physics2D.CircleCastAll(pos, 1f, Vector2.zero);
                //iterate
                string hitss = "";
                float distance = 1000000;
                GameObject closest = null;
                foreach (var hit in hits)
                {
                    hitss += hit.collider.gameObject.name + " ";
                    if (hit.collider.CompareTag("CableHeadOld"))
                    {
                        closest = null;
                        break;
                    }
                    if (hit.collider.CompareTag("CablePart"))
                    {
                        if (distance > hit.distance)
                        {
                            distance = hit.distance;
                            closest = hit.collider.gameObject;
                        }
                        /* if (hit.distance < 0.1f)
                         {
                             closest = null;
                             break;
                         }*/
                    }
                }
                if (closest != null)
                {
                    joint = closest.GetComponent<CustomJoint2D>();
                    pos.z = joint.transform.position.z;
                    joint.transform.position = pos;



                    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                    pointerEventData.position = Input.mousePosition;




                    ExecuteEvents.Execute(joint.gameObject, pointerEventData, ExecuteEvents.beginDragHandler);
                    s = true;
                }
                Debug.Log(hitss);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            s = false;
        }
        if (s)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            ExecuteEvents.Execute(joint.gameObject, pointerEventData, ExecuteEvents.dragHandler);
        }
    }
    [Button]
    public void MakeCable()
    {
        MakeCable(lengthToMake, distance);
    }


}
