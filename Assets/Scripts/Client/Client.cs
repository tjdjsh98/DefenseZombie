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

    public int ClientId = -1;

    bool _sendRequest = false;
    bool _recvAnswer = false;
    bool _isEnableEnterGame = false;
    bool _successEnterGame = false;

    [field:SerializeField]public float Delay = 0;
    public void Send(ArraySegment<byte> segment)
    {
        _session.Send(segment);
    }

    public void Init(string ipAddress)
    {
        _clinet = this;
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
            S_BroadcastMove pkt = packet as S_BroadcastMove;
            PacketManager.Instance.HandlePacket(_session, packet);
        }
        if (IsEnterStart && !_successEnterGame)
        {
            if(_recvAnswer)
            {
                if(_isEnableEnterGame)
                {
                    SceneManager.LoadScene("InGame");
                    _successEnterGame = true;
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void AnswerRequest(bool enable)
    {
        _recvAnswer = true;
        _isEnableEnterGame = enable;
        Delay = Time.time - Delay;
    }

    public void EnterGame()
    {
        IsEnterStart = true;    
    }
}
