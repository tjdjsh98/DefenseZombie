using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour, ICharacterOption
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

    bool _isInteractItem;

    public bool IsControllerable { get {
            return ((Client.Instance.ClientId == -1 && Manager.Character.MainCharacter == _character )||
                Client.Instance.ClientId == _character.CharacterId); } }

    public bool IsDone { get;  set; }

    public void Init()
    {
        _character = GetComponent<CustomCharacter>();
        AttackKeyDownHandler += OnAttackKeyDown;
        if (!Client.Instance.IsSingle)
        {
            StartCoroutine(CorSendMovePacket());
        }

        IsDone = true;
    }

    private void Update()
    {
        if (!IsDone) return;
        Control();
    }

    void RotateWeapon()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        _character.Turn(mousePos.x - transform.position.x);

        if (_character.IsEquipWeapon)
            _character.RotationHand(mousePos);
    }


    void Control()
    {
        if (!IsControllerable) return;

        RotateWeapon();


        if (Input.GetMouseButton(0))
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
                ExitInteract();
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Manager.Game.GenerateSupplies();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Item item = null;
            Manager.Item.GenerateItem(Define.ItemName.ZeusSpear, transform.position + Vector3.right,ref item);
        }
        
        if (_character != null)
        {
            if (_character.IsItemInteract)
            {
                _character.IsItemInteract = false;
                Client.Instance.SendCharacterControlInfo(_character);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            if (_character == null) return;

            _character.IsItemInteract = true;
            Client.Instance.SendCharacterControlInfo(_character);
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (_character == null) return;

            UI_Equipment ui = Manager.UI.GetUI(UIName.Equipment) as UI_Equipment;

            if (!ui.gameObject.activeSelf)
                ui.Open(_character);
            else
                ui.Close();
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
            Client.Instance.SendCharacterControlInfo(_character);
        }
    }

    void Interact()
    {
        InteractableObject obj = _character.GetOverrapGameObject<InteractableObject>();

        if (obj != null && obj.Interact(_character))
        {
            _interactingObject = obj;
        }
    }

    public void ExitInteract()
    {
        if(_interactingObject == null ) return;

        if (_interactingObject.ExitInteract(_character))
            _interactingObject = null;
    }
    IEnumerator CorSendMovePacket()
    {
        while (true)
        {
            if (Client.Instance.IsMain)
            {
                Client.Instance.SendCharacterInfo(_character);
            }
            if (IsControllerable)
            {
                Client.Instance.SendCharacterControlInfo(_character);
            }
            yield return new WaitForSeconds(Client.SendPacketInterval);
           
        }
    }

    void OnAttackKeyDown()
    {
        if(_character.ThrowItem())
        {
            
        }
        else if(Manager.Building.IsDrawing)
        {
            Manager.Building.PlayerRequestBuilding();
        }
    }

   
    public void DataSerialize()
    {
    }

    public void DataDeserialize()
    {
    }
}