using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
public class CableGeneratorHinge : MonoBehaviour
{
    public GameObject cablePrefab;
    public GameObject cablePartPrefab;
    public int lengthToMake;
    public float distance;

    public GameObject MakeCable(int length, float distanceBetween)
    {
        GameObject cableObject = Instantiate(cablePrefab, transform.position, Quaternion.identity);
        Cable2Hinge cable = cableObject.GetComponent<Cable2Hinge>();

        CustomJoint2DHinge last = cable.startHead;
        cable.startHead.InitAndAdd(null, distanceBetween, cable);

        int lineLenght = 1;
        int counterToMax = 0;
        int currentPointInLine = 0;
        const int max = 2;
        Vector2 direction = Vector2.right;
        cable.lineRenderer.positionCount = length + 2;

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


            CustomJoint2DHinge currentJoint = cablePart.GetComponent<CustomJoint2DHinge>();
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

    [Button]
    public void MakeCable()
    {
        MakeCable(lengthToMake, distance);
    }


}
