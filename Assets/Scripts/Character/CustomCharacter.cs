using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D.Animation;
using static Define;

public class CustomCharacter : Character
{
    AnimatorHandler _animatorHandler;
    SpriteChanager _chanager;
    CharacterEquipment _characterEquipment;

    [SerializeField] GameObject _frontHandPos;
    [SerializeField] GameObject _frontHand;

    [SerializeField] GameObject _behindHandPos;
    [SerializeField] GameObject _behindHand;

    [SerializeField] GameObject _liftPos;
    public GameObject LiftPos => _liftPos;

    // 캐릭터 상태
    public bool IsItemInteract { set; get; }

    int _holdingItemId;
    public Item HoldingItem => _holdingItemId == 0 ? null : Manager.Item.GetItem(_holdingItemId);


    // 가지고 나올 장구류
    public SetupData _setupData;

    public WeaponData WeaponData
    {
        get
        {
            if (_characterEquipment == null) return null;
            return Manager.Data.GetWeaponData(_characterEquipment.EquipWeaponName);
        }
    }
    public bool IsEquipWeapon => _characterEquipment.EquipWeaponName != WeaponName.None;

    public int attackType = 0;

    // 건물이 안 돌아가게 하기 위해서 필요
    Vector3 _preLiftBuildingCharacterScale;
    Vector3 _preLiftBuildingBuildingScale;

    public override void Init()
    {
        base.Init();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _chanager = GetComponent<SpriteChanager>();
        _characterEquipment = GetComponent<CharacterEquipment>();

        _animatorHandler.DodgeEndHandler += () => { IsEnableMove = true; };

        SetAnimatorBool("IsFrontWeapon", true);

        if(_setupData!= null && (Client.Instance.IsSingle || Client.Instance.IsMain))
        {
            SetSetup(_setupData);
        }

    }

    public void SetSetup(SetupData data)
    {
        ItemData itemData = Manager.Data.GetItemData(data.HatItem);
        if (itemData.ItemType == ItemType.Equipment)
        {
            if (itemData.EquipmentData != null && itemData.EquipmentData.CharacterParts == CharacterParts.Hat)
            {
                Item item = null;
                int requsetNumber = Manager.Item.GenerateItem(itemData.ItemName, transform.position, ref item,
                    (item) =>
                    {
                        _characterEquipment.EquipItem(item);
                    });
                if (item != null)
                {
                    _characterEquipment.EquipItem(item);
                }
            }
        }
        itemData = Manager.Data.GetItemData(data.WeaponItem);
        if(itemData.ItemType == ItemType.Equipment)
        {
            if (itemData.EquipmentData == null)
            {
                Item item = null;
                int requsetNumber = Manager.Item.GenerateItem(itemData.ItemName, transform.position, ref item,
                        (item) =>
                    {
                        _characterEquipment.EquipItem(item);
                    });

                if (item != null)
                {
                    _characterEquipment.EquipItem(item);
                }
            }
        }

    }

    protected override void MoveCharacter()
    {
        if (IsDamaged || !IsEnableMove || IsHide)
        {
            if (_rigidBody.velocity.x < 0)
            {
                SetXVelocity(_rigidBody.velocity.x + BreakSpeed * Time.deltaTime);

                if (_rigidBody.velocity.x > 0)
                    SetXVelocity(0 * Time.deltaTime);
            }
            if (_rigidBody.velocity.x > 0)
            {
                SetXVelocity(_rigidBody.velocity.x - BreakSpeed * Time.deltaTime);
                if (_rigidBody.velocity.x < 0)
                    SetXVelocity(0);
            }

            if (_isEnableFly)
            {
                if (_rigidBody.velocity.y < 0)
                {
                    SetYVelocity(_rigidBody.velocity.y + BreakSpeed * Time.deltaTime);
                    if (_rigidBody.velocity.y > 0)
                        SetYVelocity(0);
                }
                if (_rigidBody.velocity.y > 0)
                {
                    SetYVelocity(_rigidBody.velocity.y - BreakSpeed * Time.deltaTime);
                    if (_rigidBody.velocity.y < 0)
                        SetYVelocity(0);
                }
            }
            return;
        }

        // 속도 줄이기, 브레이크
        if (_characterMoveDirection.x == 0)
        {
            if (_rigidBody.velocity.x < 0)
            {
                SetXVelocity(_rigidBody.velocity.x + BreakSpeed * Time.deltaTime);
                if (_rigidBody.velocity.x > 0)
                    SetXVelocity(0 * Time.deltaTime);
            }
            if (_rigidBody.velocity.x > 0)
            {
                SetXVelocity(_rigidBody.velocity.x - BreakSpeed * Time.deltaTime);
                if (_rigidBody.velocity.x < 0)
                    SetXVelocity(0);
            }
        }
        else if (_characterMoveDirection.x > 0)
        {
            // 관성
            if (_rigidBody.velocity.x < 0)
            {
                SetXVelocity(_rigidBody.velocity.x + BreakSpeed * Time.deltaTime);
            }
            // 가속
            else
            {
                SetXVelocity(_rigidBody.velocity.x + _accelSpeed * Mathf.Clamp01(_characterMoveDirection.x) * Time.deltaTime);
            }

            if (_rigidBody.velocity.x > _maxSpeed)
                SetXVelocity(_maxSpeed);
        }
        else if (_characterMoveDirection.x < 0)
        {
            // 관성
            if (_rigidBody.velocity.x > 0)
            {
                SetXVelocity(_rigidBody.velocity.x - BreakSpeed * Time.deltaTime);
            }
            // 가속
            else
            {
                SetXVelocity(_rigidBody.velocity.x - _accelSpeed * Mathf.Clamp01(Mathf.Abs(_characterMoveDirection.x)) * Time.deltaTime);
            }

            if (_rigidBody.velocity.x < -_maxSpeed)
                SetXVelocity(-_maxSpeed);
        }


        if (_isEnableFly)
        {
            if (_characterMoveDirection.y == 0)
            {
                if (_rigidBody.velocity.y < 0)
                {
                    SetYVelocity(_rigidBody.velocity.y + BreakSpeed * Time.deltaTime);
                    if (_rigidBody.velocity.y > 0)
                        SetYVelocity(0);
                }
                if (_rigidBody.velocity.y > 0)
                {
                    SetYVelocity(_rigidBody.velocity.y - BreakSpeed * Time.deltaTime);
                    if (_rigidBody.velocity.y < 0)
                        SetYVelocity(0);
                }
            }
            else if (_characterMoveDirection.y > 0)
            {
                if (_rigidBody.velocity.y < 0)
                {
                    SetYVelocity(_rigidBody.velocity.y + BreakSpeed * Time.deltaTime);
                }
                else
                {
                    SetYVelocity(_rigidBody.velocity.y + _accelSpeed * Time.deltaTime);
                }

                if (_rigidBody.velocity.y > _maxSpeed)
                    SetYVelocity(_maxSpeed);
            }
            else if (_characterMoveDirection.y < 0)
            {
                if (_rigidBody.velocity.y > 0)
                {
                    SetYVelocity(_rigidBody.velocity.y - BreakSpeed * Time.deltaTime);
                }
                else
                {
                    SetYVelocity(_rigidBody.velocity.y - _accelSpeed * Time.deltaTime);
                }

                if (_rigidBody.velocity.y < -_maxSpeed)
                    SetYVelocity(-_maxSpeed);
            }
        }


        if (!_isEnableFly && _jumpCount == 0 && IsJumping)
        {
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            IsJumping = false;
            _jumpCount++;
        }

    }

    public void SetEnableFrontHandRotate()
    {
        _frontHandPos.transform.localPosition = new Vector3(-0.3125f, 0.375f, 0);
        _frontHand.transform.localPosition = new Vector3(0.3125f, -0.375f, 0);

        SetAnimatorBool("IsEquip", true);
        SetAnimatorBool("IsFrontWeapon", true);
    }
    public void SetDisableFrontHandRotate()
    {
        _frontHandPos.transform.localPosition = Vector3.zero;
        _frontHand.transform.localPosition = Vector3.zero;
        _frontHandPos.transform.localRotation = Quaternion.identity;

        SetAnimatorBool("IsEquip", true);
        SetAnimatorBool("IsFrontWeapon", true);
    }

    public void SetEnableBehindHandRotate()
    {
        _behindHandPos.transform.localPosition = new Vector3(-0.3125f, 0.375f, 0);
        _behindHand.transform.localPosition = new Vector3(0.3125f, -0.375f, 0);

        SetAnimatorBool("IsEquip", false);
        SetAnimatorBool("IsFrontWeapon", false);
    }
    public void SetDisableBehindHandRotate()
    {
        _behindHandPos.transform.localPosition = Vector3.zero;
        _behindHand.transform.localPosition = Vector3.zero;
        _behindHandPos.transform.localRotation = Quaternion.identity;

        SetAnimatorBool("IsEquip", false);
        SetAnimatorBool("IsFrontWeapon", true);
    }

    protected override void ControlAnimation()
    {
        if (!IsDamaged && (!IsAttacking || IsEnableMoveWhileAttack))
        {
            if (_rigidBody.velocity.x != 0)
            {
                SetAnimatorBool("Walk", true);
            }
            else
            {
                SetAnimatorBool("Walk", false);
            }
        }
        else
        {
            SetAnimatorBool("Walk", false);
        }

        if (IsDodge)
        {
            SetAnimatorBool("Dodge", true);
            _rigidBody.velocity = new Vector2(_maxSpeed * (transform.localScale.x > 0 ? 1 : -1)
                , _rigidBody.velocity.y);
        }
        if (!IsDodge)
        {
            SetAnimatorBool("Dodge", false);
        }


        SetAnimatorBool("Damaged", IsDamaged);

        if (IsEnableAttack)
        {
            SetAnimatorInteger("AttackType", AttackType);
            SetAnimatorBool("Attack", IsAttacking);
        }
        else
        {
            IsAttacking = false;
        }

        SetAnimatorFloat("VelocityY", _rigidBody.velocity.y);

        SetAnimatorBool("IsHoldingItem", _holdingItemId != 0 && !IsEquipWeapon);
        SetAnimatorBool("IsEquip", IsEquipWeapon);

        SetAnimatorBool("ContactGround", IsContactGround);

        if (IsConncetCombo)
        {
            SetAnimatorTrigger("ConnectCombo");
            IsConncetCombo = false;
        }

        if (IsItemInteract)
        {
            if(!IsAttacking && !IsDamaged)
                ItemInteract();
            IsItemInteract = false;
        }

    }

    public void ItemInteract()
    {
        if (!Client.Instance.IsSingle && !Client.Instance.IsMain) return;
        // 무기를 제외한 아이템을 들고 있고 건물이 근처에 있다면 건축에 아이템을 넣음
        if (HoldingItem != null && !IsEquipWeapon)
        {
            List<IEnableInsertItem> insertableList = GetOverrapGameObjects<IEnableInsertItem>();

            bool isSucess = false;
            if (insertableList.Count > 0)
            {
                foreach (var insertable in insertableList)
                {
                    if (insertable.InsertItem(HoldingItem))
                    {
                        Putdown();
                        isSucess = true;
                        break;
                    }
                }
            }
            if (!isSucess)
            {
                Putdown();
            }
        }
        else
        {
            if (!HoldingItem)
            {
                GrapSomething();
            }
            else
            {
                Putdown();
            }

        }
    }

    public void SetHoldingItemId(int id)
    {
        _holdingItemId  = id;
    }

    public GameObject GrapSomething()
    {
        if (HoldingItem != null ) return null;

        List<Item> items = GetOverrapGameObjects<Item>();
        Item item = null;
        float distance = 1000;
        foreach(var i in items)
        {
            if ((i.transform.position - transform.position).magnitude < distance)
            {
                distance = (i.transform.position - transform.position).magnitude;
                item = i;
            }

        }
        if (GrapItem(item))
            return item.gameObject;

        return null;
    }

    public bool GrapItem(Item item)
    {
        if (item == null) return false;
        if (HoldingItem || IsEquipWeapon) return false;


        bool isSuccess = true;
        if (!item.IsGraped && item.ItemData.ItemType == ItemType.Etc)
        {
            item.GrapItem(this);
            _holdingItemId = item.ItemId;
        }
        else if (_characterEquipment.EquipItem(item))
        {
        }
        else
        {
            isSuccess = false;
        }

        Client.Instance.SendCharacterInfo(this);
       
        return isSuccess;
    }

    // 아이템의 강체 고정을 해제
    public void ReleaseItem()
    {
        if (_holdingItemId != 0)
        {
            Item holdingItem = HoldingItem;
            holdingItem.ReleaseItem();
            _holdingItemId = 0;
        }
    }

    // 손에 들고있는 아이템, 작은 건물, 무기를 내려놓음
    public void Putdown()
    {
        if (HoldingItem)
        {
            Item holdingItem = HoldingItem;
            if (holdingItem.ItemData.ItemType == ItemType.Etc)
            {
                holdingItem.ReleaseItem(this, true);
            }
            else if (holdingItem.ItemData.ItemType == ItemType.Equipment)
            {
                if (_characterEquipment.TakeOffWeapon())
                {
                }
            }
            _holdingItemId = 0;
        }
        else if(_holdingItemId != 0) 
        {
            _holdingItemId = 0;
        }

    }

    public override void Turn(float direction)
    {
        if (direction == 0) return;
        if (TurnedHandler != null)
            TurnedHandler(direction);

        Vector3 scale = Vector3.one;
        scale.x = direction > 0 ? 1 : -1;
        transform.localScale = scale;

    }

    public void RotationHand(Vector3 targetPos)
    {
        if (WeaponData != null && WeaponData.IsFrontWeapon)
        {
            float rotation = Mathf.Atan2((targetPos.y - _frontHandPos.transform.position.y), Mathf.Abs(targetPos.x - _frontHandPos.transform.position.x));
            rotation = rotation / Mathf.PI * 180f;

            _frontHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        }

        if (WeaponData != null && !WeaponData.IsFrontWeapon)
        {
            float rotation = Mathf.Atan2((targetPos.y - _behindHandPos.transform.position.y), Mathf.Abs(targetPos.x - _behindHandPos.transform.position.x));
            rotation = rotation / Mathf.PI * 180f;

            _behindHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        }
    }
    public void RotationHandZero()
    {
        if (WeaponData != null && WeaponData.IsFrontWeapon)
        {
            _frontHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        if (WeaponData != null && !WeaponData.IsFrontWeapon)
        {
            _behindHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    protected override void Dead()
    {
        Putdown();
        DeadHandler?.Invoke();
        Manager.Character.RemoveCharacter(CharacterId);
    }

    public override string SerializeData()
    {
        Util.StartWriteSerializedData();

        Util.WriteSerializedData(IsDamaged);
        Util.WriteSerializedData(_holdingItemId);
        Util.WriteSerializedData(IsHide);


        foreach (var option in _optionList)
        {
            option.DataSerialize();
        }

        string stringData = Util.EndWriteSerializeData();


        return stringData;
    }

    public override void DeserializeData(string stringData)
    {
        if (string.IsNullOrEmpty(stringData)) return;
        Util.StartReadSerializedData(stringData);

        bool _isDamaged = Util.ReadSerializedDataToBoolean();
        if (_isDamaged && _isDamaged != IsDamaged)
        {
            
            if (_shakingCoroutine != null) StopCoroutine(_shakingCoroutine);
            _shakingCoroutine = StartCoroutine(CorShaking());
        }
        SetAnimatorBool("Damaged", _isDamaged);
        IsDamaged = _isDamaged;
        _holdingItemId = Util.ReadSerializedDataToInt();
        bool isHide = Util.ReadSerializedDataToBoolean();
        if (!IsHide && isHide)
            HideCharacter();
        if (IsHide && !isHide)
            ShowCharacter();


        foreach (var option in _optionList)
        {
            option.DataDeserialize();
        }
    }

    public override string SeralizeControlData()
    {
        Util.StartWriteSerializedData();
        Util.WriteSerializedData(transform.localScale.x);
        Util.WriteSerializedData(_rigidBody.velocity.x);
        Util.WriteSerializedData(_rigidBody.velocity.y);
        Util.WriteSerializedData(_characterMoveDirection.x);
        Util.WriteSerializedData(AttackType);
        Util.WriteSerializedData(IsAttacking);
        Util.WriteSerializedData(IsEnableMoveWhileAttack);
        Util.WriteSerializedData(IsJumping);
        Util.WriteSerializedData(IsContactGround);
        Util.WriteSerializedData(IsConncetCombo);
        Util.WriteSerializedData(IsItemInteract);

        Util.WriteSerializedData(_frontHandPos.transform.localRotation.z);
        Util.WriteSerializedData(_frontHandPos.transform.localRotation.w);
        Util.WriteSerializedData(_behindHandPos.transform.localRotation.z);
        Util.WriteSerializedData(_behindHandPos.transform.localRotation.w);

        return Util.EndWriteSerializeData();
    }
    public override void DeserializeControlData(string stringData)
    {
        if (string.IsNullOrEmpty(stringData)) return ;
        Util.StartReadSerializedData(stringData);

        float scale = Util.ReadSerializedDataToFloat();
        Turn(scale);
        SetXVelocity(Util.ReadSerializedDataToFloat());
        SetYVelocity(Util.ReadSerializedDataToFloat());
        SetCharacterDirection(new Vector3(Util.ReadSerializedDataToFloat(), 0, 0));
        AttackType = Util.ReadSerializedDataToInt();
        IsAttacking = Util.ReadSerializedDataToBoolean();
        IsEnableMoveWhileAttack = Util.ReadSerializedDataToBoolean();
        IsJumping = Util.ReadSerializedDataToBoolean();
        IsContactGround = Util.ReadSerializedDataToBoolean();
        IsConncetCombo = Util.ReadSerializedDataToBoolean();
       
      

        IsItemInteract = Util.ReadSerializedDataToBoolean();
        if(IsItemInteract)
        {
            ItemInteract();
            IsItemInteract = false;
        }

        float frontHandRotationZ = Util.ReadSerializedDataToFloat();
        float frontHandRotationW = Util.ReadSerializedDataToFloat();
        float behindHandRotationZ = Util.ReadSerializedDataToFloat();
        float behindHandRotationW = Util.ReadSerializedDataToFloat();

        _frontHandPos.transform.localRotation = new Quaternion(0, 0, frontHandRotationZ, frontHandRotationW);
        _behindHandPos.transform.localRotation = new Quaternion(0, 0, behindHandRotationZ, behindHandRotationW);
    }

    public float GetFrontHandRotation()
    {
        return _frontHandPos.transform.localRotation.eulerAngles.z;
    }

    public float GetBehindHandRotation()
    {
        return _behindHandPos.transform.localRotation.eulerAngles.z;
    }

    
    public Vector3 GetFrontHandPosition()
    {
        return _frontHandPos.transform.position;
    }

    public Vector3 GetBehindHandPosition()
    {
        return _behindHandPos.transform.position;
    }
}

