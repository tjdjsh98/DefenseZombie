using System;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
            if (player.CharacterId == pkt.playerId)
            {
                if (player.IsDummy)
                {
                    player.DummyController.SetMovePacket(pkt);
                    return;
                }
            }
            
        }

        foreach (var enemy in Manager.Character.EnemyList)
        {
            if (enemy.CharacterId == pkt.playerId)
            {
                if (enemy.IsDummy)
                {
                    enemy.DummyController.SetMovePacket(pkt);
                    return;
                }
            }
        }

    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList pkt = packet as S_PlayerList;

        foreach (var player in pkt.players)
        {
            Vector3 pos = new Vector3(player.posX, player.posY, player.posZ);
            Character character = Manager.Character.GenerateCharacter("SpannerCharacter", pos, !player.isSelf);
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
        ClientSession clientSession = session as ClientSession;
        C_Attack pkt = pakcet as C_Attack;

        if (pkt == null || clientSession.Room == null)
        {
            return;
        }

        GameRoom room = clientSession.Room;
        room.Push(() => { room.Attack(pkt); });
    }

    public static void S_BroadcastAttackHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastAttack pkt = packet as S_BroadcastAttack;
        if (pkt == null) return;

        if (pkt.attackerId == Client.Instance.ClientId) return;

        Character character = Manager.Character.GetCharacter(pkt.attackerId);

        if (character == null) return;

        GameObject effectOrigin = Manager.Data.GetEffect(pkt.attackEffectName);
        Vector3 effectPoint = new Vector3(pkt.attackPointX, pkt.attackPointY,0);
        
        if (effectOrigin != null)
        {
            GameObject effect = GameObject.Instantiate(effectOrigin);
            effect.transform.parent = character.transform;
            effect.transform.localPosition = effectPoint;
            effect.transform.localScale = Vector3.one;
        }

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
        if (Client.Instance.IsMain) return;

        S_BroadcastGenerateCharacter pkt = packet as S_BroadcastGenerateCharacter;

        Manager.Character.GenerateDummyCharacter(pkt);
    }

    public static void S_CharacterListHandler(PacketSession session, IPacket packet)
    {
    }

    public static void C_HitHandler(PacketSession session, IPacket packet)
    {
        C_Hit pkt = packet as C_Hit;
    }

    public static void S_BroadcastHitHandler(PacketSession session, IPacket packet)
    {
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

    public static void C_AddForceHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession == null) return;

        C_AddForce pkt= packet as C_AddForce;
        S_BroadcastAddForce sendPacket = new S_BroadcastAddForce();

        sendPacket.characterId = pkt.characterId;
        sendPacket.forceX= pkt.forceX;
        sendPacket.forceY = pkt.forceY;
        sendPacket.power = pkt.power;
        sendPacket.constraints = pkt.constraints;
     
        GameRoom room = clientSession.Room;

        room?.Push(() => { room.Broadcast(sendPacket.Write()); });
    }

    public static void S_BroadcastAddForceHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastAddForce pkt = packet as S_BroadcastAddForce;

        Character character= Manager.Character.GetCharacter(pkt.characterId);

        if (character == null) return;

        character.AddForce(new Vector2(pkt.forceX, pkt.forceY), pkt.power,pkt.constraints);
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
