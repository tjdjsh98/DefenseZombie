﻿using System;
using UnityEngine;
using static Define;
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
    public static void S_BroadcastCharacterInfoHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastCharacterInfo pkt = packet as S_BroadcastCharacterInfo;

        Character character = Manager.Character.GetCharacter(pkt.characterId);

        character?.SetCharacterInfoPacket(pkt);
    }


    public static void S_BroadcastNewClientHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastNewClient pkt = packet as S_BroadcastNewClient;
        UI_Debug ui = Manager.UI.GetUI(UIName.Debug) as UI_Debug;
        ui.AddText($"#{pkt.ToString()}");
        Client.Instance.EnterNewOtherClinet(pkt.clientId);
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

   
    public static void S_EnterSyncInfosHandler(PacketSession session, IPacket packet)
    {
        S_EnterSyncInfos pkt = packet as S_EnterSyncInfos;
        UI_Debug ui = Manager.UI.GetUI(UIName.Debug) as UI_Debug;
        ui.AddText($"#{pkt.ToString()}");
        Manager.Character.GeneratePacketCharacter(pkt);
        Manager.Item.GenerateItemByPacket(pkt);
        Manager.Building.GenerateBuildingByPacket(pkt);
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

    public static void C_RequestGenerateCharacterHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null || clientSession.Room == null) return;

        C_RequestGenerateCharacter pkt = packet as C_RequestGenerateCharacter;

        clientSession.Room.Push(() => { clientSession.Room.GenerateCharacter(clientSession,pkt); });
    }

    public static void S_BroadcastGenerateCharacterHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastGenerateCharacter pkt = packet as S_BroadcastGenerateCharacter;


        UI_Debug ui = Manager.UI.GetUI(UIName.Debug) as UI_Debug;
        ui.AddText($"#{pkt.ToString()}");

        Manager.Character.GenerateCharacterByPacket(pkt);
    }

    public static void C_RequestRemoveCharacterHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null || clientSession.Room == null) return;

        C_RequestRemoveCharacter pkt = packet as C_RequestRemoveCharacter;
        
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

        if (Client.Instance.IsMain && character.CharacterId == Client.Instance.ClientId) return;

        character.Damage(0,new Vector2(pkt.directionX,pkt.directionY),pkt.power,pkt.stagger);
    }

    public static void C_RequestGenerateBuildingHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession == null) return;

        C_RequestGenerateBuilding pkt = packet as C_RequestGenerateBuilding;

        GameRoom room = clientSession.Room;

        room?.Push(() => { room.GenerateBuilding(pkt); });
    }
    public static void S_BroadcastGenerateBuildingHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastGenerateBuilding pkt = packet as S_BroadcastGenerateBuilding;

        if (pkt == null) return;

        Manager.Building.GenerateBuilding(pkt);
    }

    public static void C_RequestGenerateItemHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null) return;

        C_RequestGenerateItem pkt = packet as C_RequestGenerateItem;
        if (pkt == null) return;

        GameRoom room = clientSession.Room;

        room?.Push(() => { room.GenreateItem(pkt); });
    }

    public static void S_BroadcastGenerateItemHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastGenerateItem pkt = packet as S_BroadcastGenerateItem;
        if (pkt == null) return;

        Manager.Item.GenerateItemByPacket(pkt);
    }
    public static void C_RequestRemoveItemHandler(PacketSession session, IPacket packet)
    {
        C_RequestRemoveItem pkt = packet as C_RequestRemoveItem;
        ClientSession clientSession = session as ClientSession;    

        if (pkt == null) return;
        if (clientSession == null) return;

        GameRoom room = clientSession.Room;

        room?.Push(() => { room.RemoveItem(pkt); });
    }

    public static void S_BroadcastRemoveItemHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastRemoveItem pkt = packet as S_BroadcastRemoveItem;

        if (pkt == null) return;

        Manager.Item.RemoveItemByPacket(pkt);
    }
    public static void C_RequestRemoveBuildingHandler(PacketSession session, IPacket packet)
    {
        C_RequestRemoveBuilding pkt = packet as C_RequestRemoveBuilding;
        ClientSession clientSession = session as ClientSession;

        if (pkt == null) return;
        if (clientSession == null) return;

        GameRoom room = clientSession.Room;

        room?.Push(() => { room.RemoveBuilding(pkt); });
    }

    public static void S_BroadcastRemoveBuildingHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastRemoveBuilding pkt = packet as S_BroadcastRemoveBuilding;

        if (pkt == null) return;

        Manager.Building.RemoveBuildingByPacket(pkt);
    }

    public static void C_ItemInfoHandler(PacketSession session, IPacket packet)
    {
        C_ItemInfo pkt = packet as C_ItemInfo;

        if (pkt == null) return;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.SyncItemInfo(clientSession, pkt));
    }

    public static void C_BuildingInfoHandler(PacketSession session, IPacket packet)
    {
        C_BuildingInfo pkt = packet as C_BuildingInfo;

        if (pkt == null) return;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.SyncBuildingInfo(clientSession, pkt));
    }

    public static void S_BroadcastItemInfoHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastItemInfo pkt = packet as S_BroadcastItemInfo;

        if (pkt == null) return;

        Item item = Manager.Item.GetItem(pkt.itemId);
        if(item != null)
        {
            item.SyncItemInfo(pkt);
        }
    }

    public static void S_BroadcastBuildingInfoHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastBuildingInfo pkt = packet as S_BroadcastBuildingInfo;

        if (pkt == null) return;
        Building building = Manager.Building.GetBuilding(pkt.buildingId);
        if (building != null)
        {
            building.SyncBuildingInfo(pkt);
        }
    }
}
