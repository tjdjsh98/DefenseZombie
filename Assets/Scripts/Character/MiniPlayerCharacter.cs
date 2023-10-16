using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MiniPlayerCharacter : PlayerCharacter
{
    AnimatorHandler _animatorHandler;

    bool _isStartAttackAnimation;

    [SerializeField]string _changeClipName;
    [SerializeField] AnimationClip _changeClip;
    [SerializeField] AnimationClip _changeClip2;

    [System.Serializable]
    public class Weapon
    {
        public string name;
        public Sprite idleSprite;
        public AnimationClip animationClip;
    }

    [SerializeField] Sprite _emptyFrontHand;
    [SerializeField] Sprite _grabFrontHand;

    [SerializeField]Weapon _grabedWeapon;
    [SerializeField] bool _equipButton;
    bool _equipWeapon;

    [SerializeField] SpriteRenderer _frontHandSpriteRenderer;
    [SerializeField] SpriteRenderer _weaponSpriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _animatorHandler.DodgeEndHandler += () => { CharacterState = CharacterState.Idle; };
      
        _animatorHandler.AttackEndHandler += () => { _isStartAttackAnimation = false; };

        if(_grabedWeapon == null)
        {
            _frontHandSpriteRenderer.sprite = _emptyFrontHand;
            _weaponSpriteRenderer.sprite = null;
            _equipWeapon = false;
        }
        else
        {
            _frontHandSpriteRenderer.sprite = _grabFrontHand;
            _weaponSpriteRenderer.sprite = _grabedWeapon.idleSprite;
            _equipWeapon = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        if(_equipButton)
        {
            _equipButton = false;
            if(_equipWeapon)
            {
                _weaponSpriteRenderer.sprite = null;
                _equipWeapon = false;
                RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

                AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

                myOverrideController[_changeClipName] = _changeClip;
            }
            else
            {
                _weaponSpriteRenderer.sprite = _grabedWeapon.idleSprite;
                _equipWeapon = true;
                RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

                AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

                myOverrideController[_changeClipName] = _changeClip2;
            }
        }
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
