using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item2 : MonoBehaviour
{
    public CableSocket socket;
    public void OnConnect(Cable cable, CableSocket to)
    {
        Debug.Log("My socke has made connection to " + to.name + " with cable " + cable.name);
    }
    public void OnDisconnect()
    {
        Debug.Log("My socket connection has been removed");
    }
    public void Start()
    {
        socket.OnConnectEvent.AddListener(OnConnect);
        socket.OnDisconnectEvent.AddListener(OnDisconnect);
    }
    public void OnDestroy()
    {
        socket.OnConnectEvent.RemoveListener(OnConnect);
        socket.OnDisconnectEvent.RemoveListener(OnDisconnect);
    }
}
