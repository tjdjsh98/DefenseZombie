using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using static Define;
using static UnityEditor.Progress;

public class CustomCharacter : Character
{
    AnimatorHandler _animatorHandler;
    SpriteChanager _chanager;

    [SerializeField] GameObject _frontHandPos;
    [SerializeField] GameObject _frontHand;

    [SerializeField] GameObject _behindHandPos;
    [SerializeField] GameObject _behindHand;

    Building _takenBuilding;
    Item _takenItem;
    public Item TakenItem => _takenItem;

    // 가지고 나올 무기 정보
    [SerializeField]WeaponData _preTakenWeaponData;
    public WeaponData PreTakenWeaponData => _preTakenWeaponData;
    // 현재 장착 중인 무기의 정보
    WeaponData _weaponData;
    public WeaponData WeaponData
    {
        get {
            if (_weaponData == null) return Manager.Data.GetWeaponData(WeaponName.None);
            return _weaponData; }
    }
    public int attackType = 0;

    public bool IsLift => _takenBuilding != null || (_takenItem != null && !IsEquip);
    public bool IsEquip => _weaponData != null;
  

    [SerializeField] GameObject _liftPos;

    Vector3 _preLiftBuildingCharacterScale;
    Vector3 _preLiftBuildingBuildingScale;

    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _chanager = GetComponent<SpriteChanager>();

        _animatorHandler.DodgeEndHandler += () => { CharacterState = CharacterState.Idle; };

        SetAnimatorBool("IsFrontWeapon", true);   

        if(_preTakenWeaponData != null)
        {
            string weaponName = _preTakenWeaponData.WeaponName.ToString();

            ItemName itemName = Define.ItemName.None;
            Enum.TryParse(weaponName, true, out itemName);

            if(itemName != ItemName.None)
            {
                Item item = Manager.Item.GenerateItem(itemName, transform.position);
                GrapItem(item);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void MoveCharacter()
    {
        if (IsHide || !IsEnableMove) return;
        if (CharacterState != CharacterState.Idle)
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

        WeaponData data = Manager.Data.GetWeaponData(name);

        if (data == null) return false;

        _weaponData = data;

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

        if(data.IsFrontWeapon)
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

        _weaponData = null;

        return true;
    }

    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        if (CharacterState == CharacterState.Idle)
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

        if (CharacterState == CharacterState.Dodge && !IsDodge)
        {
            SetAnimatorBool("Dodge", true);
            IsDodge = true;
        }
        if (IsDodge)
        {
            _rigidBody.velocity = new Vector2(_maxSpeed * (transform.localScale.x > 0 ? 1 : -1)
                , _rigidBody.velocity.y);
        }
        if (CharacterState != CharacterState.Dodge && IsDodge)
        {
            SetAnimatorBool("Dodge", false);
            IsDodge = false;
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

        SetAnimatorBool("IsLift", IsLift);
        SetAnimatorBool("IsEquip", IsEquip);

        SetAnimatorBool("ContactGround", IsContactGround);

        if (IsConncetCombo)
        {
            SetAnimatorTrigger("ConnectCombo");
            IsConncetCombo = false;
        }
    }

    public bool GetIsLiftSomething()
    {
        return IsEquip || IsLift;
    }

    public GameObject GrapSomething()
    {
        if (_takenItem || _takenBuilding) return null;

        Item item = GetOverrapGameObject<Item>();
        if (item != null)
        {
            GrapItem(item);
            return item.gameObject;
        }
        else
        {
            Building building = GetOverrapGameObject<Building>();

            if (building != null && !building._isTile&&building != null)
            {
                if (building.BuildingSize.width == 1)
                {
                    Manager.Building.RemoveBuilding(building);

                    _takenBuilding = building;
                    building.transform.parent = _liftPos.transform;
                    building.transform.localPosition = Vector3.zero;

                    _preLiftBuildingBuildingScale = _takenBuilding.transform.localScale;
                    _preLiftBuildingCharacterScale = transform.transform.localScale;

                }
                return building.gameObject;

            }
        }
        return null;
    }

    public void GrapItem(Item item)
    {
        if (_takenItem || _takenBuilding || IsEquip) return;

        if (item != null)
        {
            if (item.ItemData.ItemType == ItemType.Etc)
            {
                _takenItem = item;
                _takenItem.FreezeRigidBody();
                _takenItem.transform.parent = _liftPos.transform;
                _takenItem.transform.localPosition = Vector3.zero;

            }
            else if (item.ItemData.ItemType == ItemType.Equipment)
            {
                string ItemName = item.ItemData.ItemName.ToString();

                WeaponName weaponName = WeaponName.None;
                Enum.TryParse(ItemName, true, out weaponName);

                if (EquipWeapon(weaponName))
                {
                    item.Hide();
                    _takenItem = item;
                }

            }
        }
    }

    // 아이템의 강체 고정을 해제
    public void ReleaseItem()
    {
        _takenItem.ReleaseRigidBody();
        _takenItem.transform.parent = transform.parent;
        _takenItem = null;
    }

    // 손에 들고있는 아이템, 작은 건물, 무기를 내려놓음
    public void PutdownItem()
    {
        if(_takenItem != null)
        {
            if (_takenItem.ItemData.ItemType == ItemType.Etc)
            {
                _takenItem.ReleaseRigidBody();
                _takenItem.transform.parent = transform.parent;
                _takenItem.transform.position = transform.position;
            }
            else if(_takenItem.ItemData.ItemType == ItemType.Equipment)
            {
                if (TakeOffWeapon())
                {
                    _takenItem.ReleaseRigidBody();
                    _takenItem.transform.parent = transform.parent;
                    _takenItem.transform.position = transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left);
                    _takenItem.Show();
                }
            }

            _takenItem = null;
        }
        if(_takenBuilding != null) {
            if (Manager.Building.SetBuilding(gameObject,transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left), _takenBuilding))
            {
                _takenBuilding = null;
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

        if(_takenBuilding != null)
        {
            if(_preLiftBuildingCharacterScale.x == transform.localScale.x)
            {
                if(_takenBuilding.transform.localScale.x != _preLiftBuildingBuildingScale.x)
                {
                    _takenBuilding.transform.localScale = _preLiftBuildingBuildingScale;
                }
            }
            else
            {
                if (_takenBuilding.transform.localScale.x == _preLiftBuildingBuildingScale.x)
                {
                    Vector3 temp = _preLiftBuildingBuildingScale;
                    temp.x = -_preLiftBuildingBuildingScale.x;
                    _takenBuilding.transform.localScale = temp;
                }
            }
        }
    }

    public void RotationFrontHand(Vector3 targetPos)
    {
        if (_weaponData != null && _weaponData.IsFrontWeapon)
        {
            float rotation = Mathf.Atan2((targetPos.y - _frontHandPos.transform.position.y), Mathf.Abs(targetPos.x - _frontHandPos.transform.position.x));
            rotation = rotation / Mathf.PI * 180f;

            _frontHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        }

        if (_weaponData != null && !_weaponData.IsFrontWeapon)
        {
            float rotation = Mathf.Atan2((targetPos.y - _behindHandPos.transform.position.y), Mathf.Abs(targetPos.x - _behindHandPos.transform.position.x));
            rotation = rotation / Mathf.PI * 180f;

            _behindHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
        }
    }

    protected override void Dead()
    {
        PutdownItem(); 
        DeadHandler?.Invoke();
        Manager.Character.RemoveCharacter(CharacterId);
    }
}
