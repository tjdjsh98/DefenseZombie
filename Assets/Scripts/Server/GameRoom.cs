using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using CharacterInfo = S_EnterSyncInfos.CharacterInfo;

public class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    Dictionary<int, CharacterInfo> _characterDictionary = new Dictionary<int, CharacterInfo>();
    Dictionary<int, BuildingInfo> _buildingDictionary = new Dictionary<int, BuildingInfo>();
    Dictionary<int, Dictionary<int, BuildingInfo>> _buildingCoordiate = new Dictionary<int, Dictionary<int, BuildingInfo>>();

    static int _characterId = 1000;
    static int _buildingId = 0;

    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Flush()
    {
        foreach (var s in _sessions)
            s.Send(_pendingList);

        // Console.WriteLine($"Flushed {_pendingList.Count} itmes");
        _pendingList.Clear();
    }

    public void Broadcast(ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }
    public void Enter(ClientSession session)
    {
        // 플레이어 추가
        _sessions.Add(session);
        session.Room = this;
    }

    public void SuccessEnter(ClientSession session)
    {
        // 새로운 플레이어에게 모든 캐릭터의 정보를 보낸다.
        S_EnterSyncInfos packet = new S_EnterSyncInfos();

        foreach (var c in _characterDictionary.Values)
        {
            packet.characterInfos.Add(new CharacterInfo()
            {
                characterId = c.characterId,
                characterName = c.characterName,
                hp= c.hp,
                posX = c.posX,
                posY = c.posY,
                posZ = c.posZ,
                xSpeed = c.xSpeed,
                ySpeed = c.ySpeed,
                characterState = c.characterState,
                characterMoveDirection = c.characterMoveDirection,
                attackType = c.attackType,
                isAttacking = c.isAttacking,
                isJumping = c.isJumping,
                isContactGround = c.isContactGround,
                isConnectCombo = c.isConnectCombo,
            });
        }
        session.Send(packet.Write());

        // 다른 플레이어들에게 새로운 플레이어의 입장을 알립니다.
        S_BroadcastNewClient broadcastPkt = new S_BroadcastNewClient();
        broadcastPkt.clientId = session.SessionId;

        Broadcast(broadcastPkt.Write());

        // 모두에게 새로운 클라이언트의 캐릭터 생성을 알립니다.
        if (!_characterDictionary.ContainsKey(session.SessionId))
        {

            S_BroadcastGenerateCharacter gen = new S_BroadcastGenerateCharacter();

            Define.CharacterName characterName = (Define.CharacterName.SpannerCharacter);

            gen.characterId = session.SessionId;
            gen.isSuccess = true;
            gen.requestNumber = 0;
            gen.characterName = (int)characterName;
            gen.posX = 0;
            gen.posY = 0;
            gen.posZ = 0;

            CharacterInfo info = new CharacterInfo()
            {
                characterId = gen.characterId,
                characterName = gen.characterName,
                posX = gen.posX,
                posY = gen.posY,
                posZ = gen.posZ,
            };
            _characterDictionary.Add(info.characterId, info);

            Broadcast(gen.Write());
        }
    }

    public void Leave(ClientSession session)
    {
        //플레이어 제거
         _sessions.Remove(session);

        // 모두에게 알린다
        S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
        leave.playerId = session.SessionId;
        Broadcast(leave.Write());
    }

    public void SyncCharacterInfo(ClientSession session, C_CharacterInfo packet)
    {
        // 캐릭터의 정보를 갱신해줍니다.
        CharacterInfo info = null;

        _characterDictionary.TryGetValue(packet.characterId, out info);

        if(info != null)
        {
            info.characterId = packet.characterId;
            info.hp = packet.hp;
            info.posX = packet.posX;
            info.posY = packet.posY;
            info.posZ = packet.posZ;
            info.xSpeed = packet.xSpeed;
            info.ySpeed = packet.ySpeed;
            info.characterState = packet.characterState;
            info.characterMoveDirection = packet.characterMoveDirection;
            info.attackType = packet.attackType;
            info.isAttacking = packet.isAttacking;
            info.isJumping = packet.isJumping;
            info.isContactGround = packet.isContactGround;
            info.isConnectCombo = packet.isConnectCombo;

            // 모두에게 알린다
            S_BroadcastCharacterInfo pkt = new S_BroadcastCharacterInfo();
            pkt.characterId = packet.characterId;
            pkt.hp = packet.hp;
            pkt.posX = packet.posX;
            pkt.posY = packet.posY;
            pkt.posZ = packet.posZ;
            pkt.xSpeed = packet.xSpeed;
            pkt.ySpeed = packet.ySpeed;
            pkt.characterState = packet.characterState;
            pkt.characterMoveDirection = packet.characterMoveDirection;
            pkt.attackType = packet.attackType;
            pkt.isAttacking = packet.isAttacking;
            pkt.isJumping = packet.isJumping;
            pkt.isContactGround = packet.isContactGround;
            pkt.isConnectCombo = packet.isConnectCombo;

            Broadcast(pkt.Write());
        }
    }

    public void GenerateCharacter(ClientSession clientSession, C_RequestGenerateCharacter packet)
    {
        S_BroadcastGenerateCharacter gen = new S_BroadcastGenerateCharacter();

        Define.CharacterName characterName = (Define.CharacterName)packet.characterName;

        gen.isSuccess = true;

        if (packet.isPlayerableChracter && _characterDictionary.ContainsKey(clientSession.SessionId))
            gen.isSuccess = false;
        
        if (packet.isPlayerableChracter) gen.characterId = clientSession.SessionId;
        else gen.characterId = ++_characterId;

        gen.requestNumber = packet.requestNumber;
        gen.characterName = packet.characterName;
        gen.posX = packet.posX;
        gen.posY = packet.posY;
        gen.posZ = packet.posZ;

        CharacterInfo info = new CharacterInfo()
        {
            characterId = gen.characterId,
            characterName = gen.characterName,
            posX = packet.posX,
            posY = packet.posY,
            posZ = packet.posZ,
        };
        _characterDictionary.Add(info.characterId,info);

        Broadcast(gen.Write());
    }

    public void RemoveCharacter(C_RequestRemoveCharacter packet)
    {
        CharacterInfo info = null;
        _characterDictionary.TryGetValue(packet.characterId, out info);

        if (info == null) return;

        _characterDictionary.Remove(packet.characterId);

        S_BroadcastRemoveCharacter sendPacket = new S_BroadcastRemoveCharacter();
        sendPacket.characterId = packet.characterId;

        Broadcast(sendPacket.Write());
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

        foreach(var s in _sessions)
        {
            if(s.SessionId == sendPacket.characterId)
            {
                s.Send(sendPacket.Write());
                return;
            }
        }
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
                    buildingName = (BuildingName)packet.buildingName,
                    cellPos = new Vector2Int(packet.posX, packet.posY),
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


        Broadcast(sendPacket.Write());
    }
}

public class BuildingInfo
{
    public BuildingName buildingName;
    public int buildingId;
    public Vector2Int cellPos;
}