using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CableSocket : MonoBehaviour
{
    public static float connectionDistance = Mathf.Pow(3f, 2);
    public static float unConnectDistance = Mathf.Pow(4f, 2);

    public CableHead connectedHead;
    public bool connected => connectedHead != null;
    public Transform connectionPoint;
    private void OnMouseOver()
    {
        if (!connected)
        {
            CableHead.currentlyDragging?.NewFixPositon(CableHead.currentlyDragging);
            if (CableHead.currentlyDragging?.nearestSlot == null)
            {
                CableHead.currentlyDragging?.SetSlotToRotate(this);
            }
        }
    }
    private void OnMouseEnter()
    {
        if (!connected)
        {
            CableHead.currentlyDragging?.SetSlotToRotate(this);
        }

    }
    private void OnMouseExit()
    {
        if (!connected)
        {
            CableHead.currentlyDragging?.SetSlotToRotate(null);
        }
    }

    public void ConnectCableHead(CableHead head)
    {
        connectedHead = head;
        connectedHead.connectedSlot = this;

        connectedHead.transform.position = connectionPoint.position;
        connectedHead.state = connectedHead.state.Join(JointState.Connected);
        if (connectedHead.isEnd)
        {
            connectedHead.cable.endSlot = this;
        }
        else
        {
            connectedHead.cable.startSlot = this;
        }
        connectedHead.cable.ConnectionChanged();

    }
    public void DisconnectCableHead()
    {
        connectedHead.state = connectedHead.state.Unjoin(JointState.Connected);
        if (connectedHead.nearestSlot == this)
        {
            connectedHead.nearestSlot = null;
        }
        connectedHead.connectedSlot = null;
        if (connectedHead.isEnd)
        {
            connectedHead.cable.endSlot = null;
        }
        else
        {
            connectedHead.cable.startSlot = null;
        }
        connectedHead.cable.ConnectionChanged();
        connectedHead = null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(connectionPoint.transform.position, Mathf.Sqrt(connectionDistance));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(connectionPoint.transform.position, Mathf.Sqrt(unConnectDistance));
    }
}
public static class EnumHelper
{
    public static T Join<T>(this T enumValue, Enum value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), Convert.ToInt32(enumValue) | Convert.ToInt32(value));
    }

    public static T Unjoin<T>(this T enumValue, Enum value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), Convert.ToInt32(enumValue) & ~Convert.ToInt32(value));
    }
    public static bool Has(this Enum enumValue, Enum value)
    {
        return (Convert.ToInt32(enumValue) & Convert.ToInt32(value)) != 0;
    }
    public static bool HasAny(this Enum enumValue, params Enum[] value)
    {
        foreach (var item in value)
        {
            if (enumValue.Has(item))
            {
                return true;
            }
        }
        return false;
    }
    public static bool HasAll(this Enum enumValue, params Enum[] value)
    {
        foreach (var item in value)
        {
            if (!enumValue.Has(item))
            {
                return false;
            }
        }
        return true;
    }
    public static string ToFormatedString(this Enum enumValue)
    {
        string text = enumValue.GetType().Name + " has: ";
        foreach (var item in Enum.GetValues(enumValue.GetType()))
        {
            if (enumValue.Has((Enum)item))
            {
                text += item.ToString() + ", ";
            }
        }
        return text;
    }
}
