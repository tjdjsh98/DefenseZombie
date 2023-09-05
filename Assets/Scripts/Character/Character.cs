using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    protected Rigidbody2D _rigidBody;
    protected SpriteRenderer _spriteRenderer;
    protected CapsuleCollider2D _capsuleCollider;
    protected Animator _animator;

    [SerializeField] protected Range _characterSize;

    public CharacterState CharacterState { set; get; }
    protected Vector2 _characterMoveDirection;


    // 캐릭터 상태
    [SerializeField] protected int _maxHp;
    int _hp;
    public int Hp { get { return _hp; } set { _hp = value; if (_hp <= 0) Dead(); } }
    public int AttackType { get; set; } = 0;
    // 속도
    protected float _currentSpeed;
    [SerializeField]protected float _accelSpeed = 2.0f;
    [SerializeField] protected float _groundBreakSpeed;
    [SerializeField] protected float _airBreakSpeed;
    protected float BreakSpeed => IsContactGround? _groundBreakSpeed : _airBreakSpeed;
    [SerializeField]protected float _speed = 5.0f;
    public float Speed { set { _speed = value; } get { return _speed; } }

    // 점프
    [SerializeField] protected Range _groundCheckRange;
    [SerializeField] protected float _jumpPower = 10.0f;
    protected int _jumpCount = 0;

    public Action<float> TurnHandler;

    
    // 행동 상태
    public bool IsAttacking { set; get; }
    public bool IsJumping { set; get; }
    public bool IsContactGround { set; get; }

    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();

        Hp = _maxHp;
    }


    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position + _characterSize.center, _characterSize.size);
        Gizmos.DrawWireCube(transform.position + _groundCheckRange.center, _groundCheckRange.size);
    }

    protected void Update()
    {
        MoveCharacter();
        CheckGround();
        ControlAnimation();
    }

    protected virtual void ControlAnimation()
    {

    }


    protected void MoveCharacter()
    {
        if (CharacterState != CharacterState.Idle )
        {
            _currentSpeed = _rigidBody.velocity.x;
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

            _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
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

        _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
    }

    protected void CheckGround()
    {
        float ySpeed = _rigidBody.velocity.y;

        if(Util.GetGameObjectByPhysics(transform.position, _groundCheckRange, LayerMask.GetMask("Ground")))
        {
            IsJumping = false;

            IsContactGround = true;

            if (ySpeed < 0)
            {
                _jumpCount = 0;
            }
        }
        else
        {
            IsJumping = true;
            IsContactGround = false;
        }
    }

    public void SetCharacterDirection(Vector2 moveDirection)
    {
        _characterMoveDirection = moveDirection;
    }
    public void Jump()
    {
        if (_jumpCount == 0)
        {
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            IsJumping = true;
        }
    }

    protected virtual void Turn(float direction)
    {
        if (TurnHandler != null)
            TurnHandler(direction);

        Vector3 scale = Vector3.one;
        scale.x = direction > 0 ? 1 : -1;
        transform.localScale = scale;
    }

    public void Damage(int dmg, Vector2 attackDirection, float power, float staggerTime)
    {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.AddForce(attackDirection.normalized * power, ForceMode2D.Impulse);
        CharacterState = CharacterState.Damage;
        Invoke("TurnToIdle", staggerTime);

        Hp -= dmg;

    }

    protected void Dead()
    {
        Destroy(this.gameObject);
    }

    protected void TurnToIdle()
    {
        CharacterState = CharacterState.Idle;
    }

    public GameObject GetOverrapGameObject(int layerMask = -1)
    {
        GameObject gameObject = null;

        gameObject = Util.GetGameObjectByPhysics(transform.position, _characterSize, layerMask);

        return gameObject;
    }

    public T GetOverrapGameObject<T>(int layerMask = -1) where T : MonoBehaviour
    {
        T gameObject = null;

        gameObject = Util.GetGameObjectByPhysics<T>(transform.position, _characterSize, layerMask);

        return gameObject;
    }

    // 캐릭터의 모습만을 숨깁니다.
    public virtual void HideCharacter()
    {
        _rigidBody.isKinematic = true;
        _capsuleCollider.enabled = false;
        _spriteRenderer.enabled = false;
    }

    public virtual void ShowCharacter()
    {
        _rigidBody.isKinematic = false;
        _capsuleCollider.enabled = true;
        _spriteRenderer.enabled = true;
    }

    public void SetAnimatorBool(string name, bool value) => _animator.SetBool(name, value);
    public void SetAnimatorInteger(string name, int value) => _animator.SetInteger(name, value);
    public void SetAnimatorTrigger(string name) => _animator.SetTrigger(name);
    public void ResetAnimatorTrigger(string name) => _animator.ResetTrigger(name); 

    public void SetAnimatorFloat(string name, float value) => _animator.SetFloat(name, value);
}
public enum CharacterState
{
    Idle,
    Attack,
    Damage,
}