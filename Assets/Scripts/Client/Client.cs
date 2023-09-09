using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    static Client _clinet;
    public static Client Instance { get { return _clinet; } }

    public bool IsEnterStart { get; private set; }

    static ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> segment)
    {
        _session.Send(segment);
    }

    public void Init(string ipAddress)
    {
        // DNS
        //string host = Dns.GetHostName();
        //IPHostEntry entry = Dns.GetHostEntry(host);
        IPAddress ipAddr = IPAddress.Parse(ipAddress);

        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connecter = new Connector();

        connecter.Connect(endPoint, () => { return _session; },EnterGame);

    }

    void Update()
    {
        List<IPacket> packets = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in packets)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }

        if (IsEnterStart)
        {
            IsEnterStart = false;
            SceneManager.LoadScene("InGame");
        }
    }

    public void EnterGame()
    {
        IsEnterStart = true;    
    }
}
