using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerCharacter _character;
    House _inHouse;

    Client _client;
    public Client Client {
        get {
            if (_client == null)
            {
                if (GameObject.Find("Client"))
                    _client = GameObject.Find("Client").GetComponent<Client>();
            }
            return _client;
        }
    }

    float _movePacketDelay = 0.1f;

    public bool IsControllerable { get {
            if (Client == null) return false;
            return Client.ClientId == _character.CharacterId; } }
    private void Awake()
    {
        _character = GetComponent<PlayerCharacter>();
        StartCoroutine(CorSendMovePacket());
    }

    private void Update()
    {
        Control();
    }

    void Control()
    {
        if (!IsControllerable) return;

        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x = -1;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _character.Jump();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            SendMoveData();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_inHouse == null)
            {
                House house = _character.GetOverrapGameObject<House>();
                if (house != null)
                {
                    house.EnterCharacter(_character);
                    _inHouse = house;
                }
            }
            else
            {
                _inHouse.LeaveCharacter();
                _inHouse = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!Manager.Building.IsDrawing)
                Manager.Building.StartBuildingDraw(this.gameObject, "Barricade");
            else
                Manager.Building.GenreateBuilding();

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Manager.Building.IsDrawing)
                Manager.Building.StopBuildingDrawing();
        }

        _character.SetCharacterDirection(moveDirection);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            SendMoveData();
        }
    }

    IEnumerator CorSendMovePacket()
    {
        while (true)
        {
            if (IsControllerable)
            {
                SendMoveData();
                yield return new WaitForSeconds(_movePacketDelay);
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }
        }
    }

    void SendMoveData()
    {
        C_Move packet = new C_Move();
        packet.characterId = _character.CharacterId;
        packet.posX = transform.position.x;
        packet.posY = transform.position.y;
        packet.posZ = transform.position.z;
        packet.currentSpeed = _character.CurrentSpeed;
        packet.ySpeed = _character.YSpeed;
        packet.characterState = (int)_character.CharacterState;
        packet.characterMoveDirection = _character.CharacterMoveDirection.x;
        packet.attackType = _character.AttackType;
        packet.isAttacking = _character.IsAttacking;
        packet.isJumping = _character.IsJumping;
        packet.isContactGround = _character.IsContactGround;
        packet.isConnectCombo = _character.IsConncetCombo;

        Client.Instance.Send(packet.Write());
    }
}
