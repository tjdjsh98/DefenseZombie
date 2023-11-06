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

    [SerializeField] GameObject _frontHandPos;
    [SerializeField] GameObject _frontHand;

    [SerializeField] GameObject _behindHandPos;
    [SerializeField] GameObject _behindHand;

    [SerializeField] GameObject _liftPos;
    public GameObject LiftPos => _liftPos;

    // 캐릭터 상태
    public bool IsItemInteract { set; get; }

    int _holdingBuildingId;
    int _holdingItemId;
    public Item HoldingItem => _holdingItemId == 0 ? null : Manager.Item.GetItem(_holdingItemId);
    public Building HoldingBuilding => _holdingBuildingId == 0 ? null : Manager.Building.GetBuilding(_holdingBuildingId);


    // 가지고 나올 무기 정보
    [SerializeField] WeaponData _preHoldingWeaponData;
    public WeaponData PreHoldingWeaponData => _preHoldingWeaponData;

    // 현재 장착 중인 무기의 정보
    WeaponName _equipWeaponName;
    public WeaponData WeaponData
    {
        get
        {
            return Manager.Data.GetWeaponData(_equipWeaponName);
        }
    }
    public bool IsEquipWeapon => _equipWeaponName != WeaponName.None;

    public int attackType = 0;

    // 건물이 안 돌아가게 하기 위해서 필요
    Vector3 _preLiftBuildingCharacterScale;
    Vector3 _preLiftBuildingBuildingScale;


    bool _isThrow;
    Vector2 _throwDirection;

    public override void Init()
    {
        base.Init();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _chanager = GetComponent<SpriteChanager>();

        _animatorHandler.DodgeEndHandler += () => { IsEnableMove = true; };

        SetAnimatorBool("IsFrontWeapon", true);

        if (_preHoldingWeaponData != null)
        {
            string weaponName = _preHoldingWeaponData.WeaponName.ToString();

            ItemName itemName = Define.ItemName.None;
            Enum.TryParse(weaponName, true, out itemName);

            if (itemName != ItemName.None)
            {
                Item item = null;
                Manager.Item.GenerateItem(itemName, transform.position, ref item);
                GrapItem(item);
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
    public bool EquipWeapon(WeaponName name)
    {
        _equipWeaponName = name;

        WeaponData data = Manager.Data.GetWeaponData(name);

        if (data == null) return false;


        if (data.IsFrontWeapon)
            _chanager.ChangeSprite(CharacterParts.FrontWeapon, data.WeaponSpriteLibrary);
        else
            _chanager.ChangeSprite(CharacterParts.BehindWeapon, data.WeaponSpriteLibrary);

        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        if (data.AttackAnimationClip != null)
        {
            if (data.IsFrontWeapon)
                myOverrideController["CustomCharacterFrontHandAttack"] = data.AttackAnimationClip;
            else
                myOverrideController["CustomCharacterBehindHandAttack"] = data.AttackAnimationClip;
        }

        SetAnimatorBool("IsEquip", true);
        SetAnimatorBool("IsFrontWeapon", data.IsFrontWeapon);

        if (data.IsFrontWeapon)
        {
            _frontHandPos.transform.localPosition = new Vector3(-0.3125f, 0.375f, 0);
            _frontHand.transform.localPosition = new Vector3(0.3125f, -0.375f, 0);
        }
        else
        {
            _behindHandPos.transform.localPosition = new Vector3(-0.3125f, 0.375f, 0);
            _behindHand.transform.localPosition = new Vector3(0.3125f, -0.375f, 0);
        }

        return true;
    }
    public bool TakeOffWeapon()
    {
        WeaponData data = Manager.Data.GetWeaponData(WeaponName.None);

        _chanager.ChangeDefaultSprite(CharacterParts.FrontWeapon);
        _chanager.ChangeDefaultSprite(CharacterParts.BehindWeapon);

        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        if (data.IsFrontWeapon)
            myOverrideController["CustomCharacterFrontHandAttack"] = data.AttackAnimationClip;
        else
            myOverrideController["CustomCharacterBehindHandAttack"] = data.AttackAnimationClip;

        SetAnimatorBool("IsEquip", false);
        SetAnimatorBool("IsFrontWeapon", data.IsFrontWeapon);

        _frontHandPos.transform.localPosition = Vector3.zero;
        _frontHand.transform.localPosition = Vector3.zero;
        _frontHandPos.transform.localRotation = Quaternion.identity;
        _behindHandPos.transform.localPosition = Vector3.zero;
        _behindHand.transform.localPosition = Vector3.zero;
        _behindHandPos.transform.localRotation = Quaternion.identity;

        _equipWeaponName = WeaponName.None;

        return true;
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
            if (!HoldingItem && !HoldingBuilding)
            {
                GrapSomething();
            }
            else
            {
                Putdown();
            }

        }
    }

    public bool ThrowItem()
    {
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            if (HoldingItem && !IsEquipWeapon)
            {
                Item item = HoldingItem;
                ReleaseItem();
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                item.GetComponent<Projectile>()?.Throw(mousePos - transform.position, 5);

                Client.Instance.SendItemInfo(item);
                return true;
            }
        }
        else
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            _isThrow = true;
            _holdingItemId = 0;
            _throwDirection = mousePos - transform.position;

        }

        return false;
    }

    public void ThrowItem(Vector3 direction)
    {
        Item item = HoldingItem;
        ReleaseItem();
        item.GetComponent<Projectile>()?.Throw(direction, 5);

        _isThrow = false;
        _throwDirection = Vector3.zero;

        Client.Instance.SendCharacterInfo(this);
    }

    public GameObject GrapSomething()
    {
        if (HoldingItem != null || HoldingBuilding) return null;

        Item item = GetOverrapGameObject<Item>();
        if (GrapItem(item))
            return item.gameObject;

        Building building = GetOverrapGameObject<Building>();
        if (GrapBuilding(building))
            return building.gameObject;

        return null;
    }

    public bool GrapItem(Item item)
    {
        if (item == null) return false;
        if (HoldingItem || HoldingBuilding || IsEquipWeapon) return false;

        bool isSuccess = true;
        if (!item.IsGraped && item.ItemData.ItemType == ItemType.Etc)
        {
            item.GrapItem(this);
            _holdingItemId = item.ItemId;
        }
        else if (item.ItemData.ItemType == ItemType.Equipment)
        {
            string ItemName = item.ItemData.ItemName.ToString();

            WeaponName weaponName = WeaponName.None;
            Enum.TryParse(ItemName, true, out weaponName);

            if (EquipWeapon(weaponName))
            {
                item.Hide();
                _holdingItemId = item.ItemId;
                item.IsGraped = true;
            }
            else
                isSuccess = false;

        }

        Client.Instance.SendCharacterInfo(this);
       
        return isSuccess;
    }

    public bool GrapBuilding(Building building)
    {
        bool success = false;
        if (building == null) return success;
        if (HoldingItem || HoldingBuilding || IsEquipWeapon) return success;

        if (!building._isTile)
        {
            if (building.BuildingSize.width == 1)
            {
                Manager.Building.RemoveBuildingCoordinate(building);

                _holdingItemId = building.BuildingId;
                building.transform.parent = _liftPos.transform;
                building.transform.localPosition = Vector3.zero;

                _preLiftBuildingBuildingScale = building.transform.localScale;
                _preLiftBuildingCharacterScale = transform.transform.localScale;

                success = true;
            }
        }


        return success;
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
                _isThrow = false;
                holdingItem.ReleaseItem(this, true);
            }
            else if (holdingItem.ItemData.ItemType == ItemType.Equipment)
            {
                if (TakeOffWeapon())
                {
                    _isThrow = false;
                    holdingItem.Show();
                    holdingItem.ReleaseItem(this, true);
                }
            }
            _holdingItemId = 0;
        }
        if (HoldingBuilding)
        {
            if (Manager.Building.SetBuilding(gameObject, transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left), HoldingBuilding))
            {
                _holdingBuildingId = 0;
            }
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

        if (HoldingBuilding)
        {
            GameObject building = HoldingBuilding.gameObject;
            if (_preLiftBuildingCharacterScale.x == transform.localScale.x)
            {
                if (building.transform.localScale.x != _preLiftBuildingBuildingScale.x)
                {
                    building.transform.localScale = _preLiftBuildingBuildingScale;
                }
            }
            else
            {
                if (building.transform.localScale.x == _preLiftBuildingBuildingScale.x)
                {
                    Vector3 temp = _preLiftBuildingBuildingScale;
                    temp.x = -_preLiftBuildingBuildingScale.x;
                    building.transform.localScale = temp;
                }
            }
        }
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

    protected override void Dead()
    {
        Putdown();
        DeadHandler?.Invoke();
        Manager.Character.RemoveCharacter(CharacterId);
    }

    public override string SerializeData()
    {
        Util.StartWriteSerializedData();


        Util.WriteSerializedData(_isThrow);
        Util.WriteSerializedData(_throwDirection.x);
        Util.WriteSerializedData(_throwDirection.y);


        Util.WriteSerializedData(_holdingItemId);
        Util.WriteSerializedData(_holdingBuildingId);

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

        bool isThrow = Util.ReadSerializedDataToBoolean();
        float throwDirectionX = Util.ReadSerializedDataToFloat();
        float throwDirectionY = Util.ReadSerializedDataToFloat();

        _holdingItemId = Util.ReadSerializedDataToInt();
        _holdingBuildingId = Util.ReadSerializedDataToInt();


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
        Util.WriteSerializedData(IsJumping);
        Util.WriteSerializedData(IsContactGround);
        Util.WriteSerializedData(IsConncetCombo);
        Util.WriteSerializedData(IsHide);
        Util.WriteSerializedData(IsItemInteract);

        Util.WriteSerializedData(_frontHandPos.transform.localRotation.z);
        Util.WriteSerializedData(_frontHandPos.transform.localRotation.w);
        Util.WriteSerializedData(_behindHandPos.transform.localRotation.z);
        Util.WriteSerializedData(_behindHandPos.transform.localRotation.w);


        return Util.EndWriteSerializeData();
    }
    public override void DeserializeControlData(string stringData)
    {
        Util.StartReadSerializedData(stringData);

        float scale = Util.ReadSerializedDataToFloat();
        Turn(scale);
        SetXVelocity(Util.ReadSerializedDataToFloat());
        SetYVelocity(Util.ReadSerializedDataToFloat());
        SetCharacterDirection(new Vector3(Util.ReadSerializedDataToFloat(), 0, 0));
        AttackType = Util.ReadSerializedDataToInt();
        IsAttacking = Util.ReadSerializedDataToBoolean();
        IsJumping = Util.ReadSerializedDataToBoolean();
        IsContactGround = Util.ReadSerializedDataToBoolean();
        IsConncetCombo = Util.ReadSerializedDataToBoolean();
        bool isHide = Util.ReadSerializedDataToBoolean();
        if (!IsHide && isHide)
            HideCharacter();
        if (IsHide && !isHide)
            ShowCharacter();

        IsItemInteract = Util.ReadSerializedDataToBoolean();

        float frontHandRotationZ = Util.ReadSerializedDataToFloat();
        float frontHandRotationW = Util.ReadSerializedDataToFloat();
        float behindHandRotationZ = Util.ReadSerializedDataToFloat();
        float behindHandRotationW = Util.ReadSerializedDataToFloat();

        _frontHandPos.transform.localRotation = new Quaternion(0, 0, frontHandRotationZ, frontHandRotationW);
        _behindHandPos.transform.localRotation = new Quaternion(0, 0, behindHandRotationZ, behindHandRotationW);

    }

}
