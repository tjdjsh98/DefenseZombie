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
    public static void C_CharacterInfoHandler(PacketSession session, IPacket packet)
    {
        C_CharacterInfo infoPacket = packet as C_CharacterInfo;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.SyncCharacterInfo(clientSession, infoPacket));
    }

    public static void C_RequestEnterGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        S_AnswerEnterGame sendPacket = new S_AnswerEnterGame();
        sendPacket.permission = true;
        sendPacket.playerId = clientSession.SessionId;

        Server.Room.Push(() => clientSession.Send(sendPacket.Write()));
    }

    public static void S_AnswerEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_AnswerEnterGame pkt = packet as S_AnswerEnterGame;

        Client.Instance.ClientId =  pkt.playerId;

        if (pkt == null) Debug.Log($"MissingPacket {typeof(S_AnswerEnterGame)}");

        Client.Instance.AnswerRequest(pkt.permission);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;

        Manager.Character.RemoveCharacter(pkt.playerId);
    }

    public static void S_BroadcastCharacterInfoHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastCharacterInfo pkt = packet as S_BroadcastCharacterInfo;

        Character character = Manager.Character.GetCharacter(pkt.characterId);

        if(character != null && character.IsDummy)
        {
            character.DummyController.SetCharacterInfoPacket(pkt);
        }
    }

    public static void S_EnterSyncInfosHandler(PacketSession session, IPacket packet)
    {
        S_EnterSyncInfos pkt = packet as S_EnterSyncInfos;

        Manager.Character.GeneratePacketCharacter(pkt);
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

    public static void C_GenerateCharacterHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null || clientSession.Room == null) return;

        C_GenerateCharacter pkt = packet as C_GenerateCharacter;

        clientSession.Room.Push(() => { clientSession.Room.GenerateCharacter(clientSession,pkt); });
    }

    public static void S_BroadcastGenerateCharacterHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastGenerateCharacter pkt = packet as S_BroadcastGenerateCharacter;

        Manager.Character.GeneratePacketCharacter(pkt);
    }

    public static void C_RemoveCharacterHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null || clientSession.Room == null) return;

        C_RemoveCharacter pkt = packet as C_RemoveCharacter;
        
        GameRoom room = clientSession.Room as GameRoom;
        room.Push(() => { room.RemoveCharacter(pkt); });
    }

    public static void S_BroadcastRemoveCharacterHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastRemoveCharacter pkt = packet as S_BroadcastRemoveCharacter;

        Manager.Character.RemoveCharacter(pkt.characterId);
    }

    public static void C_DamageHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession == null) return;

        C_Damage pkt = packet as C_Damage;

        GameRoom room = clientSession.Room;

        room?.Push(() => { room.Damage(pkt); });
    }

    public static void S_BroadcastDamageHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastDamage pkt = packet as S_BroadcastDamage;

        Character character = Manager.Character.GetCharacter(pkt.characterId);

        if(character == null) return;

        character.Damage(0,new Vector2(pkt.directionX,pkt.directionY),pkt.power,pkt.stagger);
    }
}
