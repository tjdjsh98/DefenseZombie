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
        Character character = Manager.Character.GenerateCharacter("SpannerCharacter", pos,true);

        character.CharacterId = pkt.playerId;
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;

        Manager.Character.RemoveCharacter(pkt.playerId);
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove pkt = packet as S_BroadcastMove;

        foreach (var player in Manager.Character.PlayerList)
        {
            if(!player.IsDummy) continue;

            if (Client.Instance.ClientId != pkt.playerId)
            {
                if (player.CharacterId == pkt.playerId)
                {
                    player.GetComponent<DummyController>().SetMovePacket(pkt);
                    return;
                }
            }
        }

        foreach (var enemy in Manager.Character.EnemyList)
        {
            if (!enemy.IsDummy) continue;

            if (enemy.CharacterId == pkt.playerId)
            {
                enemy.GetComponent<DummyController>().SetMovePacket(pkt);
            }
        }

    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList pkt = packet as S_PlayerList;

        Debug.Log(pkt.players.Count);
        foreach (var player in pkt.players)
        {
            Vector3 pos = new Vector3(player.posX, player.posY, player.posZ);
            Character character = Manager.Character.GenerateCharacter("SpannerCharacter", pos, !player.isSelf);
            Debug.Log(character);
            character.CharacterId = player.playerId;
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

    public static void C_AttackHandler(PacketSession session, IPacket pakcet)
    {
    }

    public static void S_BroadcastAttackHandler(PacketSession arg1, IPacket arg2)
    {
    }

    public static void C_RequestGenerateCharacterHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null || clientSession.Room == null) return;

        C_RequestGenerateCharacter pkt = packet as C_RequestGenerateCharacter;

        clientSession.Room.Push(() => { clientSession.Room.GenerateEnemy(pkt); });
    }

    public static void S_BroadcastGenerateCharacterHandler(PacketSession session, IPacket packet)
    {
        // 메인 클라이언트면 생략
        if (Client.Instance.ClientId == 1) return;

        S_BroadcastGenerateCharacter pkt = packet as S_BroadcastGenerateCharacter;

        Manager.Character.GenerateDummyCharacter(pkt);
    }

    public static void S_CharacterListHandler(PacketSession arg1, IPacket arg2)
    {
        throw new NotImplementedException();
    }
}
