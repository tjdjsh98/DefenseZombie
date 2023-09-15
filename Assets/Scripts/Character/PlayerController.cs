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

    float _movePacketDelay = 0.25f;

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
            Client.Instance.SendMove(_character);
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
            Client.Instance.SendMove(_character);
        }
    }

    IEnumerator CorSendMovePacket()
    {
        while (true)
        {
            if (IsControllerable )
            {
                Client.Instance.SendMove(_character);
                yield return new WaitForSeconds(Client.SendPacketInterval);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

}