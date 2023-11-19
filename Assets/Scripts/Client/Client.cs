using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
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

    public static float SendPacketInterval = 0.25f;
    public bool IsMain => (ClientId == 1);

    List<int> _clientList = new List<int>();

    public static bool IsFinishLoadScene;

    int SecondPacketCount = 0;
    public int RecPacketMean = 0;
    float _time;

    public void Send(ArraySegment<byte> segment)
    {
        _session.Send(segment);
    }

    public void Init(string ipAddress)
    {
        if (SceneManager.GetActiveScene().name == "InGame") return;

        _clinet = this;
        try
        {

            IPAddress ipAddr = IPAddress.Parse(ipAddress);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            Connector connecter = new Connector();

            ip = ipAddress;

            connecter.Connect(endPoint, () => { return _session; }, EnterGame, this);
        }
        catch
        {
            Destroy(this.gameObject);
        }

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

        if ((int)_time <(int)(_time + Time.deltaTime))
        {
            RecPacketMean = SecondPacketCount / (int)(_time + Time.deltaTime);
        }
        _time += Time.deltaTime;
        if(_time > 10)
        {
            _time /= 2;
            SecondPacketCount /= 2;
        }
        List<IPacket> packets = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in packets)
        {
            SecondPacketCount++;
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
        if (!Client.Instance.IsMain) return;

        C_CharacterInfo packet = new C_CharacterInfo();
        packet.characterId = character.CharacterId;
        packet.hp = character.Hp;
        packet.data = character.SerializeData();

        Send(packet.Write());
    }
    public void SendCharacterControlInfo(Character character)
    {
        if (IsSingle) return;

        C_CharacterControlInfo packet = new C_CharacterControlInfo();
        packet.characterId = character.CharacterId;
        packet.posX = character.transform.position.x;
        packet.posY = character.transform.position.y;
        packet.data = character.SeralizeControlData();

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

    public void SendBuildingControlInfo(Building building)
    {
        if (Client.Instance.IsSingle) return;

        C_BuildingControlInfo packet = new C_BuildingControlInfo();

        packet.buildingId = building.BuildingId;
        packet.data = building.SerializeControlData();

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

    public void SendRequestGenreateEffect(EffectName name, Vector3 position, int requestNumber)
    {
        if (IsSingle) return;

        C_RequestGenerateEffect packet = new C_RequestGenerateEffect();

        packet.requestNumber = requestNumber;
        packet.effectName = (int)name;
        packet.posX = position.x;
        packet.posY = position.y;

        Send(packet.Write());
    }

    public void SendRequestGenreateProjectile(ProjectileName name, Vector3 position, int requestNumber, Vector3 direction, CharacterTag tag1, CharacterTag tag2,int damage)
    {
        if (IsSingle) return;
        C_RequestGenerateProjectile packet = new C_RequestGenerateProjectile();

        packet.requestNumber = requestNumber;
        packet.projectileName = (int)name;
        packet.posX = position.x;
        packet.posY = position.y;
        packet.fireDirectionX = direction.x;
        packet.fireDirectionY = direction.y;
        packet.characterTag1 = (int)tag1;
        packet.characterTag2 = (int)tag2;
        packet.damage = damage;

        Send(packet.Write());
    }
    public void SendRequestRemoveProjectile(int id, int requsetNumber)
    {
        if (ClientId == -1) return;

        C_RequestRemoveProjectile packet = new C_RequestRemoveProjectile();

        packet.projectileId = id;
        packet.requestNumber = requsetNumber;


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

    public void SendManagerInfo(ManagerName name, string data)
    {
        if (IsSingle) return;

        C_ManagerInfo packet = new C_ManagerInfo();
        packet.managerName = (int)name;
        packet.data = data;

        Send(packet.Write());
    }
}
