using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableOld : MonoBehaviour
{
    public List<Transform> points;
    //  public int maxCableLenght = 10;

    public CableHeadOld startHead;
    public CableHeadOld endHead;

    public LineRenderer lineRenderer;
    public float distanceBetweenParts = 1;
    public static CableOld currentlyDragging;
    /* private void OnValidate()
     {
         if (distanceBetweenParts == -1)
         {
             distanceBetweenParts = 1;
         }
     }*/
    private Vector3[] _array = null;
    public Vector3[] pointsArray
    {
        get
        {
            if (_array == null || _array.Length != points.Count)
            {
                _array = new Vector3[points.Count];
            }
            for (int i = 0; i < points.Count; i++)
            {
                _array[i] = points[i].position;
            }
            return _array;
        }
    }
    private void Start()
    {
        lineRenderer.positionCount = points.Count;
    }
    void Update()
    {
        //print pointsArray line by line
        for (int i = 0; i < pointsArray.Length; i++)
        {
        }


        lineRenderer.SetPositions(pointsArray);

    }
}
