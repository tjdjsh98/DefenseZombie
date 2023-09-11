using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

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
        // �÷��̾� �߰�
        _sessions.Add(session);
        session.Room = this;
        
        // ���� Ŭ���̾�Ʈ���� ���� ������ �˸�
        S_AnswerEnterGame answer = new S_AnswerEnterGame();
        answer.playerId = session.SessionId;
        answer.permission = true;

        session.Send(answer.Write());

    
    }

    public void SuccessEnter(ClientSession session)
    {
        // ���ο� �÷��̾�� ��� �÷��̾��� ������ ������.
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

        // ���Ի� ������ ��ο��� �˸���.
        S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
        enter.playerId = session.SessionId;
        enter.posX = 0;
        enter.posY = 0;
        enter.posZ = 0;
        Broadcast(enter.Write());
    }

    public void Leave(ClientSession session)
    {
        //�÷��̾� ����
         _sessions.Remove(session);

        // ��ο��� �˸���
        S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
        leave.playerId = session.SessionId;
        Broadcast(leave.Write());
    }

    public void Move(ClientSession session, C_Move packet)
    {
        // ��ǥ�� �ٲ��ְ�
        session.PosX = packet.posX;
        session.PosY = packet.posY;
        session.PosZ = packet.posZ;

        // ��ο��� �˸���
        S_BroadcastMove move = new S_BroadcastMove();
        move.playerId = session.SessionId;
        move.posX = packet.posX;
        move.posY = packet.posY;
        move.posZ = packet.posZ;
        move.currentSpeed = packet.currentSpeed;
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
}