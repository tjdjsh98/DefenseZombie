using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    CustomCharacter _character;
    InteractableObject _interactingObject;

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

    int _selectBuildingNameIndex;

    public bool IsControllerable { get {
            return (Client.Instance.ClientId == -1 || Client.Instance.ClientId == _character.CharacterId); } }
    private void Awake()
    {
        _character = GetComponent<CustomCharacter>();
        AttackKeyDownHandler += OnAttackKeyDown;
        if (Client.Instance.ClientId != -1)
        {
            StartCoroutine(CorSendMovePacket());
        }
    }

    private void Update()
    {
        Control();
        RotateWeapon();
    }

    void RotateWeapon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        _character.Turn(mousePos.x - transform.position.x);

        if (_character.IsEquip)
            _character.RotationFrontHand(mousePos);
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
            if (_interactingObject == null)
                Interact();
            else
            {
                if(_interactingObject.ExitInteract(_character))
                    _interactingObject = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!Manager.Building.IsDrawing)
                Manager.Building.StartBuildingDraw(this.gameObject, Define.BuildingName.Barricade);
            else
                Manager.Building.PlayerRequestBuilding();
        }
        if (Manager.Building.IsDrawing)
        { 
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _selectBuildingNameIndex--;
                if(_selectBuildingNameIndex < 0)
                {
                    _selectBuildingNameIndex = Enum.GetValues(typeof(BuildingName)).Length-1;
                }
                Manager.Building.StartBuildingDraw(this.gameObject, (BuildingName)_selectBuildingNameIndex);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                _selectBuildingNameIndex++;
                if (Enum.GetValues(typeof(BuildingName)).Length <= _selectBuildingNameIndex)
                {
                    _selectBuildingNameIndex = 0;
                }

                Manager.Building.StartBuildingDraw(this.gameObject, (BuildingName)_selectBuildingNameIndex);
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Manager.Item.GenerateItem(Define.ItemName.Stone, transform.position + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Manager.Item.GenerateItem(Define.ItemName.Spear, transform.position + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CustomCharacter player = _character as CustomCharacter;
            if (player == null) return;

            if(!player.GetIsLiftSomething())
            {
                player.GrapSomething();
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
            int num = UnityEngine.Random.Range(0, 5);
            Manager.Game.AddItem(Manager.Data.GetItemData((ItemName)num));
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyDown(KeyCode.Space))
        {
            Client.Instance.SendCharacterInfo(_character);
        }
    }

    void Interact()
    {
        InteractableObject obj = _character.GetOverrapGameObject<InteractableObject>();

        if (obj.Interact(_character))
        {
            _interactingObject = obj;
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
            if(_character.IsLift && _character.TakenItem)
            {
                Item item = _character.TakenItem;
                _character.ReleaseItem();
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                item.GetComponent<Projectile>().Throw(mousePos - _character.transform.position, 20);
            }
        }
    }

}