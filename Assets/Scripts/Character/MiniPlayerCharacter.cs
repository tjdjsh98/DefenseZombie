using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Define;

public class MiniPlayerCharacter : PlayerCharacter
{
    AnimatorHandler _animatorHandler;

    [SerializeField] Sprite _emptyFrontHand;
    [SerializeField] Sprite _emptyBehindHand;

    [SerializeField] Sprite _grabFrontHand;
    [SerializeField] Sprite _grabBehindHand;

    [SerializeField] Sprite _leftStuffFrontHand;
    [SerializeField] Sprite _leftStuffBehindHand;

    [SerializeField]WeaponName _equipWeaponName;
    [SerializeField] bool _equipButton;
    bool _equipWeapon;

    [SerializeField] SpriteRenderer _leftStuffSpriteRenderer;
    [SerializeField] SpriteRenderer _frontHandSpriteRenderer;
    [SerializeField] SpriteRenderer _behindHandSpriteRenderer;
    [SerializeField] SpriteRenderer _weaponSpriteRenderer;

    [SerializeField] AnimationClip _defaultAttackAnimationClip;


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
        }
    }

    protected override void Update()
    {
        base.Update();
        if(_equipButton)
        {
            _equipButton = false;
            if(_equipWeapon || _equipWeaponName == WeaponName.None)
            {
                _weaponSpriteRenderer.sprite = null;
                _equipWeapon = false;
                _frontHandSpriteRenderer.sprite = _emptyFrontHand;
                RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

                AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

                myOverrideController["UpperAttack"] = _defaultAttackAnimationClip;
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
}
