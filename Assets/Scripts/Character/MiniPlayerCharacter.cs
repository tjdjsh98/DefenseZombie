using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using static Define;

public class MiniPlayerCharacter : PlayerCharacter
{
    AnimatorHandler _animatorHandler;
    SpriteChanager _chanager;

    [SerializeField] GameObject _frontHandPos;
    [SerializeField] GameObject _frontHand;

    [SerializeField] WeaponName _equipWeaponName;
    [SerializeField] bool _equipButton;

    Building _liftBuilding;
    Item _liftItem;
    public Item LiftItem => _liftItem;

    [SerializeField] WeaponData _weaponData;
    public WeaponData WeaponData => _weaponData;

    public int attackType = 0;

    public bool IsLift => _liftBuilding != null || (_liftItem != null && !IsEquip);
    public bool IsEquip => _weaponData != null;
    public Attack Attack
    {
        get
        {
            if (_weaponData == null || _weaponData.AttackList.Count <= attackType)
                return new Attack();

            return _weaponData.AttackList[attackType];
        }
    }

    [SerializeField] GameObject _liftPos;


    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _chanager = GetComponent<SpriteChanager>();

        _animatorHandler.DodgeEndHandler += () => { CharacterState = CharacterState.Idle; };

        TakeOffWeapon();
    }

    protected override void Update()
    {
        base.Update();
        RotateWeapon();
    }
    protected override void MoveCharacter()
    {
        if (IsHide) return;
        _currentSpeed = _rigidBody.velocity.x;
        if (CharacterState != CharacterState.Idle)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += BreakSpeed * Time.deltaTime;
                if (_currentSpeed > 0)
                    _currentSpeed = 0;
            }
            if (_currentSpeed > 0)
            {
                _currentSpeed -= BreakSpeed * Time.deltaTime;
                if (_currentSpeed < 0)
                    _currentSpeed = 0;
            }
            return;
        }

        if (_characterMoveDirection.x == 0)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += BreakSpeed * Time.deltaTime;
                if (_currentSpeed > 0)
                    _currentSpeed = 0;
            }
            if (_currentSpeed > 0)
            {
                _currentSpeed -= BreakSpeed * Time.deltaTime;
                if (_currentSpeed < 0)
                    _currentSpeed = 0;
            }
        }
        else if (_characterMoveDirection.x > 0)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += BreakSpeed * Time.deltaTime;
            }
            else
            {
               
                _currentSpeed += _accelSpeed * Time.deltaTime;
            }

            if (_currentSpeed > _speed)
                _currentSpeed = _speed;
        }
        else if (_characterMoveDirection.x < 0)
        {
            if (_currentSpeed > 0)
            {
                _currentSpeed -= BreakSpeed * Time.deltaTime;
            }
            else
            {
               
                _currentSpeed -= _accelSpeed * Time.deltaTime;
            }

            if (_currentSpeed < -_speed)
                _currentSpeed = -_speed;
        }
        if (_jumpCount == 0 && IsJumping)
        {
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            IsJumping = false;
            _jumpCount++;
        }
        _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
    }

    void RotateWeapon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Turn(mousePos.x - transform.position.x);

        if (IsEquip ) 
        {
            float rotation = Mathf.Atan2((mousePos.y - _frontHandPos.transform.position.y) , Mathf.Abs(mousePos.x - _frontHandPos.transform.position.x));
            rotation = rotation / Mathf.PI * 180f;

            _frontHandPos.transform.localRotation = Quaternion.Euler(new Vector3(0,0,rotation));
        }
    }

    public bool EquipWeapon(WeaponName name)
    {
        
        WeaponData data = Manager.Data.GetWeaponData(name);
        _weaponData = data;
        if (data == null) return false;

        if(data.IsFrontWeapon)
            _chanager.ChangeSprite(CharacterParts.FrontWeapon, data.WeaponSpriteLibrary);
        else
            _chanager.ChangeSprite(CharacterParts.BehindWeapon, data.WeaponSpriteLibrary);

        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        myOverrideController["UpperAttack"] = data.AttackAnimationClip;
        SetAnimatorBool("IsEquip", true);

        _frontHandPos.transform.localPosition = new Vector3(-0.3125f, 0.375f, 0);
        _frontHand.transform.localPosition = new Vector3(0.3125f, -0.375f, 0);

        return true;
    }

    public bool TakeOffWeapon()
    {
        WeaponData data = Manager.Data.GetWeaponData(WeaponName.None);

        if(_weaponData.IsFrontWeapon)
            _chanager.ChangeDefaultSprite(CharacterParts.FrontWeapon);
        else
            _chanager.ChangeDefaultSprite(CharacterParts.BehindWeapon);
        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        myOverrideController["UpperAttack"] = data.AttackAnimationClip;
        SetAnimatorBool("IsEquip", false);

        _frontHandPos.transform.localPosition = Vector3.zero;
        _frontHand.transform.localPosition = Vector3.zero;
        _frontHandPos.transform.localRotation = Quaternion.identity;

        _weaponData = null;

        return true;
    }

    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        if (CharacterState == CharacterState.Idle)
        {
            if (_currentSpeed != 0)
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
            _rigidBody.velocity = new Vector2(_speed * (transform.localScale.x > 0 ? 1 : -1)
                , _rigidBody.velocity.y);
        }
        if (CharacterState != CharacterState.Dodge && IsDodge)
        {
            SetAnimatorBool("Dodge", false);
            IsDodge = false;
        }

      
        SetAnimatorInteger("AttackType", AttackType);
        SetAnimatorBool("Attack", IsAttacking);

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

    public GameObject LiftSomething()
    {
        if (_liftItem || _liftBuilding) return null;

        Item item = GetOverrapGameObject<Item>();
        if (item != null)
        {
            if (item.ItemData.ItemType == ItemType.Etc)
            {
                _liftItem = item;
                _liftItem.FreezeRigidBody();
                _liftItem.transform.parent = _liftPos.transform;
                _liftItem.transform.localPosition = Vector3.zero;

            }
            else if(item.ItemData.ItemType == ItemType.Equipment)
            {
                string ItemName = item.ItemData.ItemName.ToString();

                WeaponName weaponName = WeaponName.None;
                Enum.TryParse(ItemName,true, out weaponName);

                if (EquipWeapon(weaponName))
                {
                    item.Hide();
                    _liftItem = item;
                }

                SetAnimatorBool("IsEquip", true);
                return item.gameObject;
            }
        }
        else
        {
            Building building = GetOverrapGameObject<Building>();
            if (building != null)
            {
                if (building.BuildingSize.width == 1)
                {
                    Manager.Building.RemoveBuilding(building);

                    _liftBuilding = building;
                    building.transform.parent = _liftPos.transform;
                    building.transform.localPosition = Vector3.zero;

                }
                return building.gameObject;

            }
        }
        return null;
    }

    public void ReleaseItem()
    {
        _liftItem.ReleaseRigidBody();
        _liftItem.transform.parent = transform.parent;
        _liftItem = null;
    }
    public void Putdown()
    {
        if(_liftItem != null)
        {
            if (_liftItem.ItemData.ItemType == ItemType.Etc)
            {
                _liftItem.ReleaseRigidBody();
                _liftItem.transform.parent = transform.parent;
                _liftItem.transform.position = transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left);
            }
            else if(_liftItem.ItemData.ItemType == ItemType.Equipment)
            {
                if (TakeOffWeapon())
                {
                    _liftItem.ReleaseRigidBody();
                    _liftItem.transform.parent = transform.parent;
                    _liftItem.transform.position = transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left);
                    _liftItem.Show();
                }
            }

            _liftItem = null;
        }
        if(_liftBuilding != null) {
            if (Manager.Building.SetBuilding(transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left), _liftBuilding))
            {
                _liftBuilding = null;
            }
        }
    }
}
