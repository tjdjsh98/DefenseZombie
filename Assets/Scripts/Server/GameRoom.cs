using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    List<EnemyData> _enemys = new List<EnemyData>();

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
        
        // 입장 클라이언트에게 접속 성공을 알림
        S_AnswerEnterGame answer = new S_AnswerEnterGame();
        answer.playerId = session.SessionId;
        answer.permission = true;

        session.Send(answer.Write());
    }
    public void SuccessEnter(ClientSession session)
    {
        // 새로운 플레이어에게 모든 플레이어의 정보를 보낸다.
        S_PlayerList players = new S_PlayerList();
        foreach (var s in _sessions)
        {
            players.players.Add(new S_PlayerList.Player()
            {
                isSelf = (s == session),
                playerId = s.SessionId,
                posX = s.PosX,
                posY = s.PosY,
                posZ = s.PosZ,
            });
        }
        session.Send(players.Write());

        // 새로운 플레이어에게 모든 적들의 정보를 보낸다.
        foreach(var e in _enemys)
        {
            S_BroadcastGenerateCharacter pkt = e.GeneratePacket();
            session.Send(pkt.Write());
        }

        // 신입생 입장을 모두에게 알린다.
        S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
        enter.playerId = session.SessionId;
        enter.posX = 0;
        enter.posY = 0;
        enter.posZ = 0;
        Broadcast(enter.Write());
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

    public void Move(ClientSession session, C_Move packet)
    {
        // 좌표를 바꿔주고
        for(int i = 0; i < _sessions.Count; i++) 
        {
            if (_sessions[i].SessionId == packet.characterId)
            {
                _sessions[i].PosX = packet.posX;
                _sessions[i].PosY = packet.posY;
                _sessions[i].PosZ = packet.posZ;
                break;
            }
        }

        for(int i = 0; i < _enemys.Count; i++)
        {
            if (_enemys[i].enemyId== packet.characterId)
            {
                _enemys[i].position.x = packet.posX;
                _enemys[i].position.y = packet.posY;
                _enemys[i].position.z = packet.posZ;
                break;
            }
        }
        

        // 모두에게 알린다
        S_BroadcastMove move = new S_BroadcastMove();
        move.playerId = packet.characterId;
        move.posX = packet.posX;
        move.posY = packet.posY;
        move.posZ = packet.posZ;
        move.xSpeed = packet.xSpeed;
        move.ySpeed = packet.ySpeed;
        move.characterState = packet.characterState;
        move.characterMoveDirection = packet.characterMoveDirection;
        move.attackType = packet.attackType;
        move.isAttacking = packet.isAttacking;
        move.isJumping = packet.isJumping;
        move.isContactGround = packet.isContactGround;
        move.isConnectCombo = packet.isConnectCombo;

        Broadcast(move.Write());
    }

    public void GenerateEnemy(C_RequestGenerateCharacter packet)
    {
        S_BroadcastGenerateCharacter gen = new S_BroadcastGenerateCharacter();

        gen.characterId = packet.characterId;
        gen.characterName = packet.characterName;
        gen.posX = packet.posX;
        gen.posY = packet.posY;
        gen.posZ = packet.posZ;

        EnemyData data = new EnemyData() { enemyId = packet.characterId, enemyName = packet.characterName, position = new Vector3(packet.posX, packet.posY, packet.posZ) };
        _enemys.Add(data);
        Broadcast(gen.Write());
    }

    public void RemoveCharacter(C_RemoveCharacter packet)
    {
        foreach(var data in _enemys)
        {
            if(data.enemyId == packet.characterId)
            {
                _enemys.Remove(data);
                break;
            }
        }

        S_BroadcastRemoveCharacter sendPacket = new S_BroadcastRemoveCharacter();
        sendPacket.characterId = packet.characterId;

        Broadcast(sendPacket.Write());
    }

    public void Attack(C_Attack packet)
    {
        S_BroadcastAttack sendPacket = new S_BroadcastAttack();

        sendPacket.attackerId = packet.attackerId;
        sendPacket.attackPointX = packet.attackPointX;
        sendPacket.attackPointY = packet.attackPointY;
        sendPacket.attackEffectName = packet.attackEffectName;
        
        Broadcast(sendPacket.Write());
    }

    public void Hit(C_Hit packet)
    {
        S_BroadcastHit sendPacket = new S_BroadcastHit();

        sendPacket.attackerId = packet.attackerId;
        sendPacket.hitedCharacterId = packet.hitedCharacterId;
        sendPacket.power = packet.power;
        sendPacket.hitEffectName = packet.hitEffectName;
        sendPacket.attackDirectionX = packet.attackDirectionX;
        sendPacket.attackDirectionY = packet.attackDirectionY;
        sendPacket.stagger =packet.stagger;
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

public class EnemyData{
    public int enemyId;
    public string enemyName;
    public Vector3 position;

    public S_BroadcastGenerateCharacter GeneratePacket()
    {
        S_BroadcastGenerateCharacter pkt = new S_BroadcastGenerateCharacter();
        pkt.characterId = enemyId;
        pkt.characterName = enemyName;
        pkt.posX = position.x;
        pkt.posY = position.y;
        pkt.posZ = position.z;

        return pkt;
    }
}