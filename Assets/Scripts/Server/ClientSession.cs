using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCore;
using System.Net;
using System;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public GameRoom Room { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }


    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected: {endPoint}");

        Server.Room.Push(() => Server.Room.Enter(this));
    }
    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);

        if (Room != null)
        {
            GameRoom room = Room;
            Server.Room.Push(() =>
            {
                room.Leave(this);
            });
            Room = null;
        }

        Console.WriteLine($"OnDisconnected: {endPoint}");
    }


    public override void OnSend(int numOfByte)
    {
        //Console.WriteLine($"Trasferred bytes: {numOfByte}");
    }
}
