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
      _makeFunc.Add((ushort)PacketID.S_BroadcastNewClient, MakePacket<S_BroadcastNewClient>);
        _handler.Add((ushort)PacketID.S_BroadcastNewClient, PacketHandler.S_BroadcastNewClientHandler);
      _makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        _handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        _handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
      _makeFunc.Add((ushort)PacketID.S_EnterSyncInfos, MakePacket<S_EnterSyncInfos>);
        _handler.Add((ushort)PacketID.S_EnterSyncInfos, PacketHandler.S_EnterSyncInfosHandler);
      _makeFunc.Add((ushort)PacketID.C_CharacterInfo, MakePacket<C_CharacterInfo>);
        _handler.Add((ushort)PacketID.C_CharacterInfo, PacketHandler.C_CharacterInfoHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastCharacterInfo, MakePacket<S_BroadcastCharacterInfo>);
        _handler.Add((ushort)PacketID.S_BroadcastCharacterInfo, PacketHandler.S_BroadcastCharacterInfoHandler);
      _makeFunc.Add((ushort)PacketID.C_ItemInfo, MakePacket<C_ItemInfo>);
        _handler.Add((ushort)PacketID.C_ItemInfo, PacketHandler.C_ItemInfoHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastItemInfo, MakePacket<S_BroadcastItemInfo>);
        _handler.Add((ushort)PacketID.S_BroadcastItemInfo, PacketHandler.S_BroadcastItemInfoHandler);
      _makeFunc.Add((ushort)PacketID.C_BuildingInfo, MakePacket<C_BuildingInfo>);
        _handler.Add((ushort)PacketID.C_BuildingInfo, PacketHandler.C_BuildingInfoHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastBuildingInfo, MakePacket<S_BroadcastBuildingInfo>);
        _handler.Add((ushort)PacketID.S_BroadcastBuildingInfo, PacketHandler.S_BroadcastBuildingInfoHandler);
      _makeFunc.Add((ushort)PacketID.C_Damage, MakePacket<C_Damage>);
        _handler.Add((ushort)PacketID.C_Damage, PacketHandler.C_DamageHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastDamage, MakePacket<S_BroadcastDamage>);
        _handler.Add((ushort)PacketID.S_BroadcastDamage, PacketHandler.S_BroadcastDamageHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestGenerateCharacter, MakePacket<C_RequestGenerateCharacter>);
        _handler.Add((ushort)PacketID.C_RequestGenerateCharacter, PacketHandler.C_RequestGenerateCharacterHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastGenerateCharacter, MakePacket<S_BroadcastGenerateCharacter>);
        _handler.Add((ushort)PacketID.S_BroadcastGenerateCharacter, PacketHandler.S_BroadcastGenerateCharacterHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestRemoveCharacter, MakePacket<C_RequestRemoveCharacter>);
        _handler.Add((ushort)PacketID.C_RequestRemoveCharacter, PacketHandler.C_RequestRemoveCharacterHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastRemoveCharacter, MakePacket<S_BroadcastRemoveCharacter>);
        _handler.Add((ushort)PacketID.S_BroadcastRemoveCharacter, PacketHandler.S_BroadcastRemoveCharacterHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestGenerateItem, MakePacket<C_RequestGenerateItem>);
        _handler.Add((ushort)PacketID.C_RequestGenerateItem, PacketHandler.C_RequestGenerateItemHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastGenerateItem, MakePacket<S_BroadcastGenerateItem>);
        _handler.Add((ushort)PacketID.S_BroadcastGenerateItem, PacketHandler.S_BroadcastGenerateItemHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestRemoveItem, MakePacket<C_RequestRemoveItem>);
        _handler.Add((ushort)PacketID.C_RequestRemoveItem, PacketHandler.C_RequestRemoveItemHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastRemoveItem, MakePacket<S_BroadcastRemoveItem>);
        _handler.Add((ushort)PacketID.S_BroadcastRemoveItem, PacketHandler.S_BroadcastRemoveItemHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestGenerateBuilding, MakePacket<C_RequestGenerateBuilding>);
        _handler.Add((ushort)PacketID.C_RequestGenerateBuilding, PacketHandler.C_RequestGenerateBuildingHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastGenerateBuilding, MakePacket<S_BroadcastGenerateBuilding>);
        _handler.Add((ushort)PacketID.S_BroadcastGenerateBuilding, PacketHandler.S_BroadcastGenerateBuildingHandler);
      _makeFunc.Add((ushort)PacketID.C_RequestRemoveBuilding, MakePacket<C_RequestRemoveBuilding>);
        _handler.Add((ushort)PacketID.C_RequestRemoveBuilding, PacketHandler.C_RequestRemoveBuildingHandler);
      _makeFunc.Add((ushort)PacketID.S_BroadcastRemoveBuilding, MakePacket<S_BroadcastRemoveBuilding>);
        _handler.Add((ushort)PacketID.S_BroadcastRemoveBuilding, PacketHandler.S_BroadcastRemoveBuildingHandler);

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
