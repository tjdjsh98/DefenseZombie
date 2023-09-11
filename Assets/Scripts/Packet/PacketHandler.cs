using System;
using UnityEngine;
using static S_PlayerList;
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
        ClientSession clientSession = session as ClientSession;
        S_AnswerEnterGame sendPacket = new S_AnswerEnterGame();
        sendPacket.permission = true;
        sendPacket.playerId = clientSession.SessionId;

        Client.Instance.Delay = Time.time;

        Server.Room.Push(() => clientSession.Send(sendPacket.Write()));
    }

    public static void S_AnswerEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_AnswerEnterGame pkt = packet as S_AnswerEnterGame;

        Client.Instance.ClientId =  pkt.playerId;

        if (pkt == null) Debug.Log($"MissingPacket {typeof(S_AnswerEnterGame)}");



        Client.Instance.AnswerRequest(pkt.permission);
        C_SuccessToEnterServer sendPkt = new C_SuccessToEnterServer();

        session.Send(sendPkt.Write());
    }

    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame pkt= packet as S_BroadcastEnterGame;

        if (pkt.playerId == Client.Instance.ClientId) return;

        Vector3 pos = new Vector3(pkt.posX, pkt.posY, pkt.posZ);
        Character character = Manager.Character.GenerateCharacter("DummySpannerCharacter", pos);

        DummyCharacter DummyCharacter = character as DummyCharacter;
        if (DummyCharacter != null)
            DummyCharacter.CharacterId = pkt.playerId;
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        throw new NotImplementedException();
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove pkt = packet as S_BroadcastMove;

        foreach(var dummy in Manager.Character.DummyList)
        {
            if (Client.Instance.ClientId != pkt.playerId)
            {
                if (dummy.CharacterId == pkt.playerId)
                {
                    dummy.SetMovePacket(pkt);
                }
            }
        }

    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList pkt = packet as S_PlayerList;

        Debug.Log(pkt.players.Count);
        foreach (var player in pkt.players)
        {
            if (!player.isSelf)
            {
                Vector3 pos = new Vector3(player.posX, player.posY, player.posZ);
                Character character = Manager.Character.GenerateCharacter("DummySpannerCharacter", pos);

                DummyCharacter dummyCharacter = character as DummyCharacter;
                if (dummyCharacter != null)
                    dummyCharacter.CharacterId = player.playerId;
            }
            else
            {
                Vector3 pos = new Vector3(player.posX, player.posY, player.posZ);
                Character character = Manager.Character.GenerateCharacter("SpannerCharacter", pos);

                PlayerCharacter playerCharacter = character as PlayerCharacter;
                if (playerCharacter != null)
                    playerCharacter.CharacterId = player.playerId;
            }
        }
    }

    public static void C_SuccessToEnterServerHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        C_SuccessToEnterServer pkt = packet as C_SuccessToEnterServer;

        if (pkt == null || clientSession.Room == null)
        {
            return;
        }

        clientSession.Room.Push(() => Server.Room.SuccessEnter(clientSession));
    }
}
