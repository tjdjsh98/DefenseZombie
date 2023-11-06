using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class Client : MonoBehaviour
{
    static Client _clinet;

    public string ip;
    public static Client Instance
    {
        get
        {
            if (_clinet == null)
            {
                GameObject g = new GameObject("Clinet");
                _clinet = g.AddComponent<Client>();
                DontDestroyOnLoad(g);

            }
            return _clinet;

        }
    }
    public bool IsEnterStart { get; private set; }

    static ServerSession _session = new ServerSession();

    public int ClientId = -1;

    bool _recvAnswer = false;
    bool _isEnableEnterGame = false;
    bool _successEnterGame = false;
    bool _isSendSuccessEnter = false;
    public bool IsSingle { get { return ClientId == -1; } }
    [field: SerializeField] public float Delay = 0;

    public static float SendPacketInterval = 0.1f;
    public bool IsMain => (ClientId == 1);

    List<int> _clientList = new List<int>();

    public static bool IsFinishLoadScene;

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

        connecter.Connect(endPoint, () => { return _session; }, EnterGame);

    }

    void Update()
    {
        if (IsEnterStart && !_successEnterGame)
        {
            if (_recvAnswer)
            {
                if (_isEnableEnterGame)
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

        if (_successEnterGame && !_isSendSuccessEnter && IsFinishLoadScene)
        {
            _isSendSuccessEnter = true;
            C_SuccessToEnterServer packet = new C_SuccessToEnterServer();
            Send(packet.Write());

            return;
        }

        List<IPacket> packets = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in packets)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    public void EnterNewOtherClinet(int clientId)
    {
        _clientList.Add(clientId);
    }
    public void LeaveClient(int clientId)
    {
        _clientList.Remove(clientId);
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
    public void SendDamage(int characterId, Vector2 attackDirection, float power, float staggerTime)
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

    public void SendCharacterInfo(Character character)
    {
        if (ClientId == -1) return;
        C_CharacterInfo packet = new C_CharacterInfo();
        packet.characterId = character.CharacterId;
        packet.posX = character.transform.position.x;
        packet.posY = character.transform.position.y;
        packet.hp = character.Hp;
        packet.data = character.SerializeData();

        Send(packet.Write());
    }

    public void SendItemInfo(Item item)
    {
        if (Client.Instance.IsSingle || !Client.Instance.IsMain) return;

        C_ItemInfo packet = new C_ItemInfo();

        packet.itemId= item.ItemId;
        packet.posX = item.transform.position.x;
        packet.posY = item.transform.position.y;
        packet.data = item.SerializeData();

        Send(packet.Write());
    }

    public void SendBuildingInfo(Building building)
    {
        if (Client.Instance.IsSingle) return;

        C_BuildingInfo packet = new C_BuildingInfo();

        packet.buildingId= building.BuildingId;
        packet.hp = building.Hp;
        Vector2Int cellPos = building.GetCoordinate()[0];
        packet.cellPosX= cellPos.x;
        packet.cellPosY = cellPos.y;
        packet.posX = building.transform.position.x;
        packet.posY = building.transform.position.y;
        packet.data = building.SerializeData();

        Send(packet.Write());
    }


    public void SendRequestGenreateCharacter(CharacterName name, Vector3 position, int requestNumber, bool isPlayerCharacter)
    {
        if (ClientId == -1) return;

        C_RequestGenerateCharacter packet = new C_RequestGenerateCharacter();


        packet.requestNumber = requestNumber;
        packet.isPlayerableChracter = isPlayerCharacter;
        packet.characterName = (int)name;
        packet.posX = position.x;
        packet.posY = position.y;

        Send(packet.Write());
    }
    public void SendRequestRemoveCharacter(int id)
    {
        if (ClientId == -1) return;

        C_RequestRemoveCharacter packet = new C_RequestRemoveCharacter();

        packet.characterId = id;


        Send(packet.Write());
    }
    public void SendRemoveCharacter(int id)
    {
        if (ClientId == -1) return;
        C_RequestRemoveCharacter pkt = new C_RequestRemoveCharacter();
        pkt.characterId = id;

        Send(pkt.Write());
    }

    public void SendRequestGeneratingBuilding(BuildingName name, Vector2Int cellPos, int requestNumber)
    {
        C_RequestGenerateBuilding packet = new C_RequestGenerateBuilding();

        packet.buildingName = (int)name;
        packet.requestNumber = requestNumber;
        packet.posX = cellPos.x;
        packet.posY = cellPos.y;

        Send(packet.Write());
    }

    public void SendRequestRemoveBuilding(int buildingId, int requestNumber)
    {
        C_RequestRemoveBuilding packet = new C_RequestRemoveBuilding();

        packet.buildingId = buildingId;
        packet.requestNumber = requestNumber;

        Send(packet.Write());
    }

    internal void SendRequestGeneratingItem(ItemName itemName, Vector3 position, int requestNumber)
    {
        C_RequestGenerateItem packet = new C_RequestGenerateItem();

        packet.itemName = (int)itemName;
        packet.requestNumber = requestNumber;
        packet.posX = position.x;
        packet.posY = position.y;

        Send(packet.Write());
    }

    public void SendRequestRemoveItem(int itemId, int requestNumber)
    {
        C_RequestRemoveItem packet = new C_RequestRemoveItem();

        packet.itemId = itemId;
        packet.requestNumber = requestNumber;

        Send(packet.Write());
    }
}
