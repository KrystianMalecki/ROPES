using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
[System.Serializable]
public class ConnectEvent : UnityEvent<Cable, CableSocket> { }

[System.Serializable]
public class DisconnectEvent : UnityEvent { }
public class CableSocket : MonoBehaviour
{
    [SerializeField]
    float connectDistance = 3f;
    [SerializeField]
    float disconnectDistance = 4f;
    float _connectSqrDistance = 0;
    float _disconnectSqrDistance = 0;

    public float connectSqrDistance
    {
        get
        {
            if (_connectSqrDistance == 0)
            {
                _connectSqrDistance = connectDistance * connectDistance;
            }
            return _connectSqrDistance;
        }
    }
    public float disconnectSqrDistance
    {
        get
        {
            if (_disconnectSqrDistance == 0)
            {
                _disconnectSqrDistance = Mathf.Pow(disconnectDistance, 2);
            }
            return _disconnectSqrDistance;
        }
    }

    public CableHead connectedHead;
    public bool connected => connectedHead != null;
    public Transform connectionPoint;
    public ConnectEvent OnConnectEvent;
    public DisconnectEvent OnDisconnectEvent;

    private void OnMouseOver()
    {
        if (!connected)
        {
            CableHead.currentlyDraggedHead?.NewFixPositon(CableHead.currentlyDraggedHead);
            if (CableHead.currentlyDraggedHead?.nearestSlot == null)
            {
                CableHead.currentlyDraggedHead?.SetSlotToRotate(this);
            }
        }
    }
    private void OnMouseEnter()
    {
        if (!connected)
        {
            CableHead.currentlyDraggedHead?.SetSlotToRotate(this);
        }

    }
    private void OnMouseExit()
    {
        if (!connected)
        {
            CableHead.currentlyDraggedHead?.SetSlotToRotate(null);
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
        connectedHead.cable.OnConnect();

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
        connectedHead.cable.OnDisconnect();

        connectedHead = null;
    }

    private void OnDrawGizmos()
    {
        if (connectionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(connectionPoint.transform.position, connectDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(connectionPoint.transform.position, disconnectDistance);
        }
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
