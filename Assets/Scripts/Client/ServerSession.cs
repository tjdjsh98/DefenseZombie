using ServerCore;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Diagnostics;

class ServerSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected: {endPoint}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer, (s,p) => PacketQueue.Instance.Push(p));
    }

    public override void OnSend(int numOfByte)
    {
        //Console.WriteLine($"Trasferred bytes: {numOfByte}");
    }
}
