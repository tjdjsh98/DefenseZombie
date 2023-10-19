using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MiniPlayerCharacter _character;
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

    public Action AttackKeyDownHandler;
    public Action AttackKeyUpHandler;

    public bool IsControllerable { get {
            return (Client.Instance.ClientId == -1 || Client.Instance.ClientId == _character.CharacterId); } }
    private void Awake()
    {
        _character = GetComponent<MiniPlayerCharacter>();
        AttackKeyDownHandler += OnAttackKeyDown;
        if (Client.Instance.ClientId != -1)
        {
            StartCoroutine(CorSendMovePacket());
        }
    }

    private void Update()
    {
        Control();
    }

    void Control()
    {
        if (!IsControllerable) return;

        if (Input.GetMouseButtonDown(0))
            AttackKeyDownHandler?.Invoke();
        if (Input.GetMouseButtonUp(0))
            AttackKeyUpHandler?.Invoke();

        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x = -1;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _character.Jump();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_inHouse == null)
            {
                House house = _character.GetOverrapGameObject<House>();
                if (house != null)
                {
                    house.EnterCharacter(_character);
                    Manager.Game.Commander.Open();
                    _inHouse = house;
                }
            }
            else
            {
                Manager.Game.Commander.Close();
                _inHouse.LeaveCharacter();
                _inHouse = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!Manager.Building.IsDrawing)
                Manager.Building.StartBuildingDraw(this.gameObject, Define.BuildingName.Barricade);
            else
                Manager.Building.PlayerRequestBuilding();

        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Manager.Item.GenerateItem(Define.ItemName.Stone, transform.position + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Manager.Item.GenerateItem(Define.ItemName.Sword, transform.position + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            MiniPlayerCharacter player = _character as MiniPlayerCharacter;
            if (player == null) return;

            if(!player.GetIsLiftSomething())
            {
                player.LiftSomething();
            }
            else
            {
                player.Putdown();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Manager.Building.IsDrawing)
                Manager.Building.StopBuildingDrawing();
        }

        _character.SetCharacterDirection(moveDirection);

        if (Input.GetKeyDown(KeyCode.O))
        {
            _character.Dodge();
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyDown(KeyCode.Space))
        {
            Client.Instance.SendCharacterInfo(_character);
        }
    }

    IEnumerator CorSendMovePacket()
    {
        while (true)
        {
            if (IsControllerable)
            {
                Client.Instance.SendCharacterInfo(_character);
            }
            yield return new WaitForSeconds(Client.SendPacketInterval);
           
        }
    }

    void OnAttackKeyDown()
    {
        if(_character.IsLift)
        {
            if(_character.IsLift && _character.LiftItem)
            {
                Item item = _character.LiftItem;
                _character.ReleaseItem();
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                item.GetComponent<Projectile>().Throw(mousePos - _character.transform.position, 20);
            }
        }
    }

}