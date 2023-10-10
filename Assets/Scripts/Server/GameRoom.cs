using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using static System.Collections.Specialized.BitVector32;
using CharacterInfo = S_EnterSyncInfos.CharacterInfo;

public class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    Dictionary<int, CharacterInfo> _characterDictionary = new Dictionary<int, CharacterInfo>();

    static int _characterId = 1000;

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
            UnityEngine.Debug.Log($" {c.characterId} 패킹");

            packet.characterInfos.Add(new CharacterInfo()
            {
                characterId = c.characterId,
                characterName = c.characterName,
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

    public void GenerateCharacter(ClientSession clientSession, C_GenerateCharacter packet)
    {
        S_BroadcastGenerateCharacter gen = new S_BroadcastGenerateCharacter();

        if(packet.isPlayerCharacter)
            gen.characterId = clientSession.SessionId;
        else
            gen.characterId = ++_characterId;
        gen.isPlayerCharacter = packet.isPlayerCharacter;
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

        UnityEngine.Debug.Log($"캐릭생성 {gen.characterId}");

        Broadcast(gen.Write());
    }

    public void RemoveCharacter(C_RemoveCharacter packet)
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
}