using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;

using UnityEngine;

using Debug = UnityEngine.Debug;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }
    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;


        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession,movePacket));
    }

    public static void C_RequestEnterGameHandler(PacketSession session, IPacket packet)
    {
        throw new NotImplementedException();
    }

    public static void S_AnswerEnterGameHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("Rec");
    }

    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        throw new NotImplementedException();
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        throw new NotImplementedException();
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        throw new NotImplementedException();
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
    }
}
