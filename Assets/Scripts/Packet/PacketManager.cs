using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
     #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance{get {return _instance; }}
    #endregion

    PacketManager() 
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>,IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort,Action<PacketSession,IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {  
              _makeFunc.Add((ushort)PacketID.C_RequestEnterGame, MakePacket<C_RequestEnterGame>);
        _handler.Add((ushort)PacketID.C_RequestEnterGame, PacketHandler.C_RequestEnterGameHandler);
      _makeFunc.Add((ushort)PacketID.S_AnswerEnterGame, MakePacket<S_AnswerEnterGame>);
        _handler.Add((ushort)PacketID.S_AnswerEnterGame, PacketHandler.S_AnswerEnterGameHandler);
      _makeFunc.Add((ushort)PacketID.C_SuccessToEnterServer, MakePacket<C_SuccessToEnterServer>);
        _handler.Add((ushort)PacketID.C_SuccessToEnterServer, PacketHandler.C_SuccessToEnterServerHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
        _handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);
      _makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        _handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        _handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
      _makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
        _handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);
      _makeFunc.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);
        _handler.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);
      _makeFunc.Add((ushort)PacketID.C_Attack, MakePacket<C_Attack>);
        _handler.Add((ushort)PacketID.C_Attack, PacketHandler.C_AttackHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastAttack, MakePacket<S_BroadcastAttack>);
        _handler.Add((ushort)PacketID.S_BroadcastAttack, PacketHandler.S_BroadcastAttackHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastMove, MakePacket<S_BroadcastMove>);
        _handler.Add((ushort)PacketID.S_BroadcastMove, PacketHandler.S_BroadcastMoveHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestGenerateCharacter, MakePacket<C_RequestGenerateCharacter>);
        _handler.Add((ushort)PacketID.C_RequestGenerateCharacter, PacketHandler.C_RequestGenerateCharacterHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastGenerateCharacter, MakePacket<S_BroadcastGenerateCharacter>);
        _handler.Add((ushort)PacketID.S_BroadcastGenerateCharacter, PacketHandler.S_BroadcastGenerateCharacterHandler);
      _makeFunc.Add((ushort)PacketID.S_CharacterList, MakePacket<S_CharacterList>);
        _handler.Add((ushort)PacketID.S_CharacterList, PacketHandler.S_CharacterListHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null) 
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func < PacketSession, ArraySegment<byte>,IPacket > func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet =  func.Invoke(session,buffer);
            if(onRecvCallback != null)
                onRecvCallback.Invoke(session,packet);
            else
                HandlePacket(session,packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        return pkt;
    }
    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if(_handler.TryGetValue(packet.Protocol,out action))
        {
            action.Invoke(session,packet);
        }
    }
}
