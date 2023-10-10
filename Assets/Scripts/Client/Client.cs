using System;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    static Client _clinet;

    public string ip;
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

    bool _recvAnswer = false;
    bool _isEnableEnterGame = false;
    bool _successEnterGame = false;
    bool _isRequestAllInfo = false;
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

        ip = ipAddress;

        connecter.Connect(endPoint, () => { return _session; },EnterGame);

    }

    void Update()
    {
        if (IsEnterStart && !_successEnterGame)
        {
            if(_recvAnswer)
            {
                if(_isEnableEnterGame)
                {
                    SceneManager.LoadScene("InGame");
                    _successEnterGame = true;
                    return;
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }

        if(_successEnterGame && !_isRequestAllInfo)
        {
            _isRequestAllInfo = true;
            C_SuccessToEnterServer packet = new C_SuccessToEnterServer();
            Send(packet.Write());

            C_GenerateCharacter genPacket = new C_GenerateCharacter();
            genPacket.isPlayerCharacter = true;
            genPacket.posX = 0;
            genPacket.posY = 0;
            genPacket.posZ = 0;
            genPacket.characterName = "SpannerCharacter";
            Send(genPacket.Write());

            return;
        }

        List<IPacket> packets = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in packets)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
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
        C_RequestEnterGame packet = new C_RequestEnterGame();
        Send(packet.Write());
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

    public void SendCharacterInfo(Character character,bool syncController = false)
    {
        if (ClientId == -1) return;
        C_CharacterInfo packet = new C_CharacterInfo();
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

    public void SendGenreateCharacter(string name,int characterId, Vector3 position)
    {
        if (ClientId == -1) return;
        C_GenerateCharacter pkt = new C_GenerateCharacter();
        pkt.isPlayerCharacter = (characterId == Client.Instance.ClientId);
        pkt.characterName = name;
        pkt.posX = position.x;
        pkt.posY = position.y;
        pkt.posZ = position.z;

        Send(pkt.Write());
    }
    public void SendRemoveCharacter(int id)
    {
        if (ClientId == -1) return;
        C_RemoveCharacter pkt = new C_RemoveCharacter();
        pkt.characterId = id;

        Send(pkt.Write());
    }
}
