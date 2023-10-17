using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class MiniPlayerCharacter : PlayerCharacter
{
    AnimatorHandler _animatorHandler;

    [SerializeField] Sprite _emptyFrontHand;
    [SerializeField] Sprite _emptyBehindHand;

    [SerializeField] Sprite _grabFrontHand;
    [SerializeField] Sprite _grabBehindHand;

    [SerializeField] Sprite _liftStuffFrontHand;
    [SerializeField] Sprite _liftStuffBehindHand;

    [SerializeField]WeaponName _equipWeaponName;
    [SerializeField] bool _equipButton;
    bool _equipWeapon;

    [SerializeField] SpriteRenderer _frontHandSpriteRenderer;
    [SerializeField] SpriteRenderer _behindHandSpriteRenderer;
    [SerializeField] SpriteRenderer _weaponSpriteRenderer;

    [SerializeField] AnimationClip _defaultAttackAnimationClip;

    Building _liftBuilding;
    Item _liftItem;
    [SerializeField] GameObject _liftPos;


    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _animatorHandler.DodgeEndHandler += () => { CharacterState = CharacterState.Idle; };

        if(_equipWeaponName == WeaponName.None)
        {
            _frontHandSpriteRenderer.sprite = _emptyFrontHand;
            _weaponSpriteRenderer.sprite = null;
            _equipWeapon = false;
        }
        else
        {
            WeaponData data = Manager.Data.GetWeaponData(_equipWeaponName);
            _frontHandSpriteRenderer.sprite = _grabFrontHand;
            _weaponSpriteRenderer.sprite = data.EquipSprite;
            _equipWeapon = true;
            RuntimeAnimatorController myController = _animator.runtimeAnimatorController;
            AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;
            myOverrideController["UpperAttack"] = data.AttackAnimationClip;
        }
    }

    public bool EquipWeapon(WeaponName name)
    {
        
        WeaponData data = Manager.Data.GetWeaponData(name);

        if (data == null) return false;

        _frontHandSpriteRenderer.sprite = _grabFrontHand;
        _weaponSpriteRenderer.sprite = data.EquipSprite;
        _equipWeapon = true;
        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        myOverrideController["UpperAttack"] = data.AttackAnimationClip;

        return true;
    }

    public bool TakeOffWeapon()
    {
        _weaponSpriteRenderer.sprite = null;
        _equipWeapon = false;
        _frontHandSpriteRenderer.sprite = _emptyFrontHand;
        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        myOverrideController["UpperAttack"] = _defaultAttackAnimationClip;

        return true;
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
                Turn(_currentSpeed);
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
                Turn(_currentSpeed);
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


        SetAnimatorBool("ContactGround", IsContactGround);

        if (IsConncetCombo)
        {
            SetAnimatorTrigger("ConnectCombo");
            IsConncetCombo = false;
        }
    }

    public bool GetIsLiftSomething()
    {
        return _equipWeapon || _liftBuilding || _liftItem;
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

                _frontHandSpriteRenderer.sprite = _liftStuffFrontHand;
                _behindHandSpriteRenderer.sprite = _liftStuffBehindHand;

                SetAnimatorBool("IsLift", true);
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

                    _frontHandSpriteRenderer.sprite = _liftStuffFrontHand;
                    _behindHandSpriteRenderer.sprite = _liftStuffBehindHand;
                    SetAnimatorBool("IsLift", true);
                }
                return building.gameObject;

            }
        }
        return null;
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
                _frontHandSpriteRenderer.sprite = _emptyFrontHand;
                _behindHandSpriteRenderer.sprite = _emptyBehindHand;
                SetAnimatorBool("IsLift", false);
            }
            else if(_liftItem.ItemData.ItemType == ItemType.Equipment)
            {
                if (TakeOffWeapon())
                {
                    _liftItem.transform.position = transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left);
                    _liftItem.Show();
                }
            }

            _liftItem = null;
        }
        if(_liftBuilding != null) {
            if (Manager.Building.SetBuilding(transform.position + (transform.localScale.x > 0 ? Vector3.right : Vector3.left), _liftBuilding))
            {
                _frontHandSpriteRenderer.sprite = _emptyFrontHand;
                _behindHandSpriteRenderer.sprite = _emptyBehindHand;
                SetAnimatorBool("IsLift", false);
                _liftBuilding = null;
            }
        }
    }
}
