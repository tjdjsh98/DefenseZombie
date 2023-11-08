using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using CharacterInfo = S_EnterSyncInfos.CharacterInfo;
using BuildingInfo = S_EnterSyncInfos.BuildingInfo;
using ItemInfo = S_EnterSyncInfos.ItemInfo;
using static S_EnterSyncInfos;

public class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    List<ClientSession> _sessionsInGame = new List<ClientSession>();

    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    List<ArraySegment<byte>> _pendingInGameList = new List<ArraySegment<byte>>();

    Dictionary<int, CharacterInfo> _characterDictionary = new Dictionary<int, CharacterInfo>();
    Dictionary<int, BuildingInfo> _buildingDictionary = new Dictionary<int, BuildingInfo>();
    Dictionary<int, Dictionary<int, BuildingInfo>> _buildingCoordiate = new Dictionary<int, Dictionary<int, BuildingInfo>>();
    Dictionary<int, ItemInfo> _itemDictionary = new Dictionary<int, ItemInfo>();
    Dictionary<int, ProjectileInfo> _projectileDictionary = new Dictionary<int, ProjectileInfo>();
    static int _characterId = 1000;
    static int _buildingId = 0;
    static int _itemId = 0;
    static int _projectileId = 0;

    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Flush()
    {
        foreach (var s in _sessions)
            s.Send(_pendingList);

        foreach (var s in _sessionsInGame)
            s.Send(_pendingInGameList);

        // Console.WriteLine($"Flushed {_pendingList.Count} itmes");
        _pendingList.Clear();
        _pendingInGameList.Clear();
    }

    public void Broadcast(ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }

    public void BroadcastInGame(ArraySegment<byte> segment)
    {
        _pendingInGameList.Add(segment);
    }

    

    public void Enter(ClientSession session)
    {
        // 플레이어 추가
        _sessions.Add(session);
        session.Room = this;
    }

    public void SuccessEnter(ClientSession session)
    {
        // 새로운 플레이어를 게임 방에 넣음
        _sessionsInGame.Add(session);

        // 새로운 플레이어에게 모든 정보를 보냄
        S_EnterSyncInfos packet = new S_EnterSyncInfos();

        foreach (var c in _characterDictionary.Values)
        {
            packet.characterInfos.Add(new CharacterInfo()
            {
                characterId = c.characterId,
                characterName = c.characterName,
                hp = c.hp,
                posX = c.posX,
                posY = c.posY,
                data1 = c.data1,
                data2 = c.data2
            });

            if (string.IsNullOrEmpty(packet.characterInfos[packet.characterInfos.Count - 1].data1))
            {
                packet.characterInfos[packet.characterInfos.Count - 1].data1 = string.Empty;
            }
            if (string.IsNullOrEmpty(packet.characterInfos[packet.characterInfos.Count - 1].data2))
            {
                packet.characterInfos[packet.characterInfos.Count - 1].data2 = string.Empty;
            }
        }

        foreach(var i in _itemDictionary.Values)
        {
            packet.itemInfos.Add(new ItemInfo()
            {
                itemId = i.itemId,
                itemName = i.itemName,
                posX = i.posX,
                posY = i.posY,
                data = i.data
            });
            if (string.IsNullOrEmpty(packet.itemInfos[packet.itemInfos.Count - 1].data))
            {
                packet.itemInfos[packet.itemInfos.Count - 1].data = string.Empty;
            }
        }

        foreach(var b in _buildingDictionary.Values)
        {
            packet.buildingInfos.Add(new BuildingInfo()
            {
                buildingId = b.buildingId,
                buildingName = b.buildingName,
                cellPosX= b.cellPosX,
                cellPosY= b.cellPosY,
                hp= b.hp,
                posX = b.posX,
                posY = b.posY,
                data= b.data,
            });
            if (string.IsNullOrEmpty(packet.buildingInfos[packet.buildingInfos.Count - 1].data))
            {
                packet.buildingInfos[packet.buildingInfos.Count - 1].data = string.Empty;
            }
        }


        session.Send(packet.Write());

        // 다른 플레이어들에게 새로운 플레이어의 입장을 알립니다.
        S_BroadcastNewClient broadcastPkt = new S_BroadcastNewClient();
        broadcastPkt.clientId = session.SessionId;

        BroadcastInGame(broadcastPkt.Write());

        // 모두에게 새로운 클라이언트의 캐릭터 생성을 알립니다.
        if (!_characterDictionary.ContainsKey(session.SessionId))
        {
            S_BroadcastGenerateCharacter gen = new S_BroadcastGenerateCharacter();

            Define.CharacterName characterName = (Define.CharacterName.CustomCharacter);

            gen.characterId = session.SessionId;
            gen.isSuccess = true;
            gen.requestNumber = 0;
            gen.characterName = (int)characterName;
            gen.posX = 0;
            gen.posY = -2;

            CharacterInfo info = new CharacterInfo()
            {
                characterId = gen.characterId,
                characterName = gen.characterName,
                posX = gen.posX,
                posY = gen.posY,
                data1 = string.Empty,
                data2 = string.Empty
            };
            _characterDictionary.Add(info.characterId, info);

            BroadcastInGame(gen.Write());
        }
    }

    public void Leave(ClientSession session)
    {
        //플레이어 제거
         _sessions.Remove(session);

        // 모두에게 알린다
        S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
        leave.playerId = session.SessionId;
        BroadcastInGame(leave.Write());
    }

    // data1 = 일반 정보
    // data2 = 컨트롤 정보
    public void SyncCharacterInfo(ClientSession session, C_CharacterInfo packet)
    {
        // 캐릭터의 정보를 갱신해줍니다.
        CharacterInfo info = null;

        _characterDictionary.TryGetValue(packet.characterId, out info);

        if(info != null)
        {
            info.characterId = packet.characterId;
            info.hp = packet.hp;
            info.data1 = packet.data;

            // 모두에게 알린다
            S_BroadcastCharacterInfo pkt = new S_BroadcastCharacterInfo();
            pkt.characterId = packet.characterId;
            pkt.hp = packet.hp;
            pkt.data = packet.data;

            BroadcastInGame(pkt.Write());
        }
    }
    public void SyncCharacterControlInfo(ClientSession session, C_CharacterControlInfo packet)
    {
        // 캐릭터의 정보를 갱신해줍니다.
        CharacterInfo info = null;

        _characterDictionary.TryGetValue(packet.characterId, out info);

        if (info != null)
        {
            info.characterId = packet.characterId;
            info.posX = packet.posX;
            info.posY = packet.posY;
            info.data2 = packet.data;

            // 모두에게 알린다
            S_BroadcastCharacterControlInfo pkt = new S_BroadcastCharacterControlInfo();
            pkt.characterId = packet.characterId;
            pkt.posX = packet.posX;
            pkt.posY = packet.posY;
            pkt.data = packet.data;

            BroadcastInGame(pkt.Write());
        }
    }
    public void SyncItemInfo(ClientSession session, C_ItemInfo packet)
    {
        // 아이템의 정보를 갱신해줍니다.
        ItemInfo info = null;

        _itemDictionary.TryGetValue(packet.itemId, out info);

        if (info != null)
        {
            info.posX = packet.posX;
            info.posY = packet.posY;
            info.data = packet.data;

            // 모두에게 알린다
            S_BroadcastItemInfo pkt = new S_BroadcastItemInfo();
            pkt.itemId = packet.itemId;
            pkt.posX = packet.posX;
            pkt.posY = packet.posY;
            pkt.data = packet.data;

            BroadcastInGame(pkt.Write());
        }
    }
    public void SyncBuildingInfo(ClientSession session, C_BuildingInfo packet)
    {
        // 건물의 정보를 갱신해줍니다.
        BuildingInfo info = null;

        _buildingDictionary.TryGetValue(packet.buildingId, out info);

        if (info != null)
        {
            info.hp= packet.hp;
            info.posX = packet.posX;
            info.posY = packet.posY;
            info.cellPosX= packet.cellPosX;
            info.cellPosY = packet.cellPosY;
            info.data = packet.data;

            // 모두에게 알린다
            S_BroadcastBuildingInfo pkt = new S_BroadcastBuildingInfo();
            pkt.buildingId = packet.buildingId;
            pkt.hp= packet.hp;
            pkt.posX = packet.posX;
            pkt.posY = packet.posY;
            pkt.cellPosX = packet.cellPosX;
            pkt.cellPosY = packet.cellPosY;
            pkt.data = packet.data;

            BroadcastInGame(pkt.Write());
        }
    }


    public void GenerateCharacter(ClientSession clientSession, C_RequestGenerateCharacter packet)
    {
        S_BroadcastGenerateCharacter sendPacket = new S_BroadcastGenerateCharacter();

        Define.CharacterName characterName = (Define.CharacterName)packet.characterName;
        
        sendPacket.isSuccess = true;

        if (packet.isPlayerableChracter && _characterDictionary.ContainsKey(clientSession.SessionId))
            sendPacket.isSuccess = false;
        
        if (packet.isPlayerableChracter) sendPacket.characterId = clientSession.SessionId;
        else sendPacket.characterId = ++_characterId;

        sendPacket.requestNumber = packet.requestNumber;
        sendPacket.characterName = packet.characterName;
        sendPacket.posX = packet.posX;
        sendPacket.posY = packet.posY;

        CharacterInfo info = new CharacterInfo()
        {
            characterId = sendPacket.characterId,
            characterName = sendPacket.characterName,
            posX = packet.posX,
            posY = packet.posY,
            data1 = string.Empty,
            data2 = string.Empty
        };
        _characterDictionary.Add(info.characterId,info);

        BroadcastInGame(sendPacket.Write());
    }

    public void RemoveCharacter(C_RequestRemoveCharacter packet)
    {
        CharacterInfo info = null;
        _characterDictionary.TryGetValue(packet.characterId, out info);

        if (info == null) return;

        _characterDictionary.Remove(packet.characterId);

        S_BroadcastRemoveCharacter sendPacket = new S_BroadcastRemoveCharacter();
        sendPacket.characterId = packet.characterId;
        sendPacket.requestNumber = packet.requestNumber;

        BroadcastInGame(sendPacket.Write());
    }

   
    public void GenerateBuilding(C_RequestGenerateBuilding packet)
    {
        bool isSucess = true;

        Building buildingOrigin = Manager.Data.GetBuilding((Define.BuildingName)packet.buildingName);

        Vector2Int cellPos = new Vector2Int(packet.posX, packet.posY);
        BuildingInfo info = null;
        if (buildingOrigin == null)
        {
            isSucess = false;
        }
        else
        {
            // 빌딩이 들어갈 크기가 되는지 검사
            for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
            {
                if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

                Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width, i / buildingOrigin.BuildingSize.width);

                if (_buildingCoordiate.ContainsKey(pos.x) && _buildingCoordiate[pos.x].ContainsKey(pos.y) && _buildingCoordiate[pos.x][pos.y] != null)
                    isSucess = false;

                if (!isSucess)
                    break;
            }

            if (isSucess)
            {
                info = new BuildingInfo()
                {
                    buildingId = ++_buildingId,
                    hp = buildingOrigin.Hp,
                    buildingName = packet.buildingName,
                    cellPosX = packet.posX,
                    cellPosY = packet.posY,
                    data = string.Empty,
                };

                _buildingDictionary.Add(info.buildingId, info);
                for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
                {
                    if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

                    Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width, i / buildingOrigin.BuildingSize.width);


                    if (!_buildingCoordiate.ContainsKey(pos.x))
                    {
                        _buildingCoordiate.Add(pos.x, new Dictionary<int, BuildingInfo>());
                    }
                    if (!_buildingCoordiate[pos.x].ContainsKey(pos.y))
                    {
                        _buildingCoordiate[pos.x].Add(pos.y, info);
                    }
                    else
                    {
                        _buildingCoordiate[pos.x][pos.y] = info;
                    }
                }
            }
        }
        S_BroadcastGenerateBuilding sendPacket = new S_BroadcastGenerateBuilding();

        sendPacket.requestNumber = packet.requestNumber;
        if (isSucess)
            sendPacket.buildingId = info.buildingId;
        sendPacket.buildingName = packet.buildingName;
        sendPacket.isSuccess = isSucess;
        sendPacket.posX= packet.posX;
        sendPacket.posY = packet.posY;
        sendPacket.requestNumber= packet.requestNumber;


        BroadcastInGame(sendPacket.Write());
    }
    public void RemoveBuilding(C_RequestRemoveBuilding packet)
    {
        BuildingInfo info = null;
        if(_buildingDictionary.TryGetValue(packet.buildingId, out info))
        {
            Building buildingOrigin = Manager.Data.GetBuilding((Define.BuildingName)info.buildingName);

            Vector2Int cellPos = new Vector2Int(info.cellPosX, info.cellPosY);

            for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
            {
                if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

                Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width, i / buildingOrigin.BuildingSize.width);

                _buildingCoordiate[pos.x][pos.y] = null;
            }

            _buildingDictionary.Remove(packet.buildingId);

            S_BroadcastRemoveBuilding sendPacket = new S_BroadcastRemoveBuilding();
            sendPacket.buildingId = packet.buildingId;
            sendPacket.requestNumber= packet.requestNumber;

            BroadcastInGame(sendPacket.Write());
        }
    }

    public void GenreateItem(C_RequestGenerateItem packet)
    {
        ItemInfo itemInfo = new ItemInfo();

        itemInfo.itemName = packet.itemName;
        itemInfo.itemId = ++_itemId;
        itemInfo.posX = packet.posX;
        itemInfo.posY = packet.posY;
        itemInfo.data = string.Empty;

        _itemDictionary.Add(itemInfo.itemId,itemInfo);

        S_BroadcastGenerateItem sendPacket = new S_BroadcastGenerateItem();

        sendPacket.requestNumber= packet.requestNumber;
        sendPacket.isSuccess = true;
        sendPacket.itemId = itemInfo.itemId;
        sendPacket.itemName = (int)itemInfo.itemName;
        sendPacket.posX = itemInfo.posX;
        sendPacket.posY = itemInfo.posY;

        BroadcastInGame(sendPacket.Write());
    }
    public void GenreateProjectile(C_RequestGenerateProjectile packet)
    {
        ProjectileInfo projectileInfo = new ProjectileInfo();

        projectileInfo.projectileName = packet.projectileName;
        projectileInfo.projectileId = ++_projectileId;
        projectileInfo.posX = packet.posX;
        projectileInfo.posY = packet.posY;
        
        projectileInfo.data = string.Empty;

        _projectileDictionary.Add(projectileInfo.projectileId, projectileInfo);

        S_BroadcastGenerateProjectile sendPacket = new S_BroadcastGenerateProjectile();

        sendPacket.requestNumber = packet.requestNumber;
        sendPacket.isSuccess = true;
        sendPacket.projectileId = projectileInfo.projectileId;
        sendPacket.projectileName = projectileInfo.projectileName;
        sendPacket.posX = projectileInfo.posX;
        sendPacket.posY = projectileInfo.posY;
        sendPacket.fireDirectionX= packet.fireDirectionX;
        sendPacket.fireDirectionY = packet.fireDirectionY;
        sendPacket.characterTag1 = packet.characterTag1;
        sendPacket.characterTag2 = packet.characterTag2;

        BroadcastInGame(sendPacket.Write());
    }
    public void RemoveProjectile(C_RequestRemoveProjectile packet)
    {
        ProjectileInfo Info = null;

        if (_projectileDictionary.TryGetValue(packet.projectileId, out Info))
        {
            _projectileDictionary.Remove(packet.projectileId);

            S_BroadcastRemoveProjectile sendPacket = new S_BroadcastRemoveProjectile();
            sendPacket.requestNumber = packet.requestNumber;
            sendPacket.projectileId = packet.projectileId;
           
            BroadcastInGame(sendPacket.Write());
        }
    }

    public void RemoveItem(C_RequestRemoveItem packet)
    {
        ItemInfo itemInfo = null;

        if(_itemDictionary.TryGetValue(packet.itemId, out itemInfo))
        {
            _itemDictionary.Remove(packet.itemId);

            S_BroadcastRemoveItem sendPacket = new S_BroadcastRemoveItem();
            sendPacket.itemId = packet.itemId;
            sendPacket.requestNumber= packet.requestNumber;
            sendPacket.isSuccess = true;

            BroadcastInGame(sendPacket.Write());
        }
    }

    public void Damage(C_Damage packet)
    {
        S_BroadcastDamage sendPacket = new S_BroadcastDamage();

        sendPacket.characterId = packet.characterId;
        sendPacket.directionX = packet.directionX;
        sendPacket.directionY = packet.directionY;
        sendPacket.characterId = packet.characterId;
        sendPacket.power = packet.power;
        sendPacket.stagger = packet.stagger;

        foreach (var s in _sessions)
        {
            if (s.SessionId == sendPacket.characterId)
            {
                s.Send(sendPacket.Write());
                return;
            }
        }
    }


}
