using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class Client : MonoBehaviour
{
    static Client _clinet;
    public static Client Instance { get 
        {
            if(_clinet == null )
            {
                GameObject g = new GameObject("Clinet");
                _clinet = g.AddComponent<Client>();
                DontDestroyOnLoad(g);
                
            }
            return _clinet; 
        
        } }
    public bool IsEnterStart { get; private set; }

    static ServerSession _session = new ServerSession();

    public int ClientId = -1;

    bool _sendRequest = false;
    bool _recvAnswer = false;
    bool _isEnableEnterGame = false;
    bool _successEnterGame = false;
    public bool IsSingle { get; set; } = false;
    [field:SerializeField]public float Delay = 0;

    public static float SendPacketInterval = 0.1f;
    public bool IsMain => ClientId == 1;
    public void Send(ArraySegment<byte> segment)
    {
        _session.Send(segment);
    }

    public void Init(string ipAddress)
    {
        if (SceneManager.GetActiveScene().name == "InGame") return;

        _clinet = this;
        IPAddress ipAddr = IPAddress.Parse(ipAddress);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
        Connector connecter = new Connector();

        connecter.Connect(endPoint, () => { return _session; },EnterGame);

    }

    void Update()
    {
        List<IPacket> packets = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in packets)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
       
        if (IsEnterStart && !_successEnterGame)
        {
            if(_recvAnswer)
            {
                if(_isEnableEnterGame)
                {
                    SceneManager.LoadScene("InGame");
                    _successEnterGame = true;
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void AnswerRequest(bool enable)
    {
        _recvAnswer = true;
        _isEnableEnterGame = enable;
        Delay = Time.time - Delay;
    }

    public void EnterGame()
    {
        IsEnterStart = true;    
    }

    public void SendDamage(int characterId,Vector2 attackDirection, float power, float staggerTime)
    {
        if (ClientId == -1) return;

        C_Damage sendPacket = new C_Damage();
        sendPacket.characterId = characterId;
        sendPacket.directionX = attackDirection.x;
        sendPacket.directionY = attackDirection.y;
        sendPacket.stagger = staggerTime;
        sendPacket.power = power;

        Send(sendPacket.Write());
    }

    public void SendMove(Character character,bool syncController = false)
    {
        if (ClientId == -1) return;
        C_Move packet = new C_Move();
        packet.characterId = character.CharacterId;
        packet.posX = character.transform.position.x;
        packet.posY = character.transform.position.y;
        packet.posZ = character.transform.position.z;
        packet.xSpeed = character.GetVelocity.x;
        packet.ySpeed = character.GetVelocity.y;
        packet.characterState = (int)character.CharacterState;
        packet.characterMoveDirection = character.CharacterMoveDirection.x;
        packet.attackType = character.AttackType;
        packet.isAttacking = character.IsAttacking;
        packet.isJumping = character.IsJumping;
        packet.isContactGround = character.IsContactGround;
        packet.isConnectCombo = character.IsConncetCombo;

        Send(packet.Write());
    }

    public void SendAddForce(Character character, Vector2 direction, float power,int constraint = -1)
    {
        if (ClientId == -1) return;
        C_AddForce packet = new C_AddForce();
        packet.characterId = character.CharacterId;
        packet.power = power;  
        packet.forceX = direction.x;
        packet.forceY = direction.y;
        packet.constraints = constraint;

        Send(packet.Write());
    }

    public void SendGenreateCharacter(string name,int characterId, Vector3 position)
    {
        if (ClientId == -1) return;
        C_RequestGenerateCharacter pkt = new C_RequestGenerateCharacter();
        pkt.characterId = characterId;
        pkt.characterName = name;
        pkt.posX = position.x;
        pkt.posY = position.y;
        pkt.posZ = position.z;

        Send(pkt.Write());
    }
    public void SendAttack(Character attacker, Attack attack)
    {
        if (ClientId == -1) return;
        C_Attack packet = new C_Attack();

        packet.attackerId = attacker.CharacterId;
        packet.attackPointX = attack.attackEffectPoint.x;
        packet.attackPointY = attack.attackEffectPoint.y;
        packet.attackEffectName = attack.attackEffectName;
        Send(packet.Write());
    }

    public void SendHit(Character attacker, Character hitedCharacter, Attack attack)
    {
        if (ClientId == -1) return;
        C_Hit packet = new C_Hit();

        Vector3 attackDirection = attack.AttackDirection;
        attackDirection.x = attack.AttackDirection.x * (transform.localScale.x > 0 ? 1 : -1);

        packet.attackerId = attacker.CharacterId;
        packet.hitedCharacterId = hitedCharacter.CharacterId;
        packet.attackDirectionX = attackDirection.x;
        packet.attackDirectionY = attackDirection.y;
        packet.power = attack.power;
        packet.damage = attack.damage;
        packet.hitEffectName = attack.hitEffectName;
        packet.stagger = attack.stagger;

        Send(packet.Write());
    }
    public void SendRemoveCharacter(int id)
    {
        if (ClientId == -1) return;
        C_RemoveCharacter pkt = new C_RemoveCharacter();
        pkt.characterId = id;

        Send(pkt.Write());
    }
}
