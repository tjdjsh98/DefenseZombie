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
                ExitInteract();
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Manager.Game.GenerateSupplies();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Manager.Item.GenerateItem(Define.ItemName.Spear, transform.position + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CustomCharacter player = _character as CustomCharacter;
            if (player == null) return;

            // 무기를 제외한 아이템을 들고 있고 건물이 근처에 있다면 건축에 아이템을 넣음
            if (_character.TakenItem != null && !_character.IsEquip)
            {
                List<IEnableInsertItem> insertableList = _character.GetOverrapGameObjects<IEnableInsertItem>();

                bool isSucess = false;
                if (insertableList.Count > 0)
                {
                    foreach(var insertable in insertableList)
                    {
                        if (insertable.InsertItem(_character.TakenItem.ItemData.ItemName))
                        {
                            Item item = _character.TakenItem;
                            _character.PutdownItem();
                            Manager.Item.DestroyItem(item.ItemId);
                            isSucess = true;
                            break;
                        }
                    }
                }
                if(!isSucess) 
                {
                    player.PutdownItem();
                }
            }
            else
            {
                if (!player.GetIsLiftSomething())
                {
                    player.GrapSomething();
                }
               
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
        else if(Manager.Building.IsDrawing)
        {
            Manager.Building.PlayerRequestBuilding();
        }
    }
}