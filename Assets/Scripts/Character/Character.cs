using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Character : MonoBehaviour,IHp
{
    protected Rigidbody2D _rigidBody;
    public Rigidbody2D RigidBody => _rigidBody;
    protected SpriteRenderer _spriteRenderer;
    protected CapsuleCollider2D _capsuleCollider;
    protected Animator _animator;

    [SerializeField] protected Range _characterSize;

    public CharacterState CharacterState { set; get; }
    protected Vector2 _characterMoveDirection;
    public Vector2 CharacterMoveDirection => _characterMoveDirection;


    // ĳ���� ����
    [SerializeField] protected int _maxHp;
    public int MaxHp => _maxHp;
    int _hp;
    public int Hp { get { return _hp; } set { _hp = value; if (_hp <= 0) Dead(); } }
    public bool _isSuperArmerWhenAttack;

    [Range(0,100)][SerializeField] protected int _standing;

    public int AttackType { get; set; } = 0;
    // �ӵ�
    protected float _currentSpeed;
    [SerializeField]protected float _accelSpeed = 2.0f;
    [SerializeField] protected float _groundBreakSpeed;
    [SerializeField] protected float _airBreakSpeed;
    public float CurrentSpeed => _currentSpeed;
    protected float BreakSpeed => IsContactGround? _groundBreakSpeed : _airBreakSpeed;
    [SerializeField]protected float _speed = 5.0f;
    public float Speed { set { _speed = value; } get { return _speed; } }

    // ����
    [SerializeField] protected Range _groundCheckRange;
    [SerializeField] protected float _jumpPower = 10.0f;
    protected int _jumpCount = 0;

    public Action<float> TurnedHandler;

    protected float _ySpeed;
    public float YSpeed=>_ySpeed;
    
    // �ൿ ����
    public bool IsAttacking { set; get; }
    public bool IsJumping { set; get; }
    public bool IsContactGround { set; get; }
    public bool IsConncetCombo { set; get; }
    public bool IsDodge { set; get; }
    public bool IsStagger { set; get; }

    public bool IsHide { set; get; }

    public int CharacterId { set; get; } = -1;
    public bool IsDummy { set; get; }

    // �ൿ �ڵ鷯
    public Action RegisterAttackHandler;
    public Action DeadHandler;

    public Vector2 GetVelocity => _rigidBody.velocity;

    Coroutine _damageCoroutine;
    DummyController _dummyController;
    public DummyController DummyController
    {
        get
        {
            if (_dummyController == null)
                _dummyController = GetComponent<DummyController>();
            return _dummyController;
        }
    }

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

    protected virtual void Update()
    {
        MoveCharacter();
        CheckGround();
        ControlAnimation();
    }

    protected virtual void ControlAnimation()
    {
    }


    protected virtual void MoveCharacter()
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

    protected void CheckGround()
    {
        _ySpeed = _rigidBody.velocity.y;

        if(Util.GetGameObjectByPhysics(transform.position, _groundCheckRange, LayerMask.GetMask("Ground")))
        {
            IsJumping = false;

            IsContactGround = true;

            if (_ySpeed < 0)
            {
                _jumpCount = 0;
            }
        }
        else
        {
            IsContactGround = false;
        }
    }

    public void SetCharacterDirection(Vector2 moveDirection)
    {
        _characterMoveDirection = moveDirection;
    }
    public void AddForce(Vector2 direction, float power, int constraints = -1)
    {
        if(constraints != -1)
            _rigidBody.constraints = (RigidbodyConstraints2D)constraints;
        
        if(power != 0)
            _rigidBody.AddForce(direction.normalized * (power * (1-_standing/100f)), ForceMode2D.Impulse);
    }
    public void Jump()
    {
        if(_jumpCount == 0)
        {
            IsJumping = true;
        }
    }

    protected virtual void Turn(float direction)
    {
        if (direction == 0) return;
        if (TurnedHandler != null)
            TurnedHandler(direction);

        Vector3 scale = Vector3.one;
        scale.x = direction > 0 ? 1 : -1;
        transform.localScale = scale;
    }

    public void Damage(int dmg, Vector2 attackDirection, float power, float staggerTime)
    {
        if (IsDodge) return;

        _rigidBody.velocity = Vector2.zero;
        AddForce(attackDirection, power);

        if (_damageCoroutine != null) StopCoroutine(_damageCoroutine);
        _damageCoroutine = StartCoroutine(CorTurnToIdle(staggerTime));

        Hp -= dmg;

        // ���� Ŭ�� �ƴϰ� �ٸ� Ŭ���� ������ ��Ŷ�� �����ϴ�.
        if (Client.Instance.IsMain)
        {
            if (CharacterId < 100 && Client.Instance.ClientId != CharacterId)
            {
                Client.Instance.SendDamage(CharacterId, attackDirection, power, staggerTime);
            }
        }
    }

    protected void Dead()
    {
        DeadHandler?.Invoke();
        Manager.Character.RemoveCharacter(CharacterId);
    }

    IEnumerator CorTurnToIdle(float time)
    {
        if (CharacterState != CharacterState.Attack || !_isSuperArmerWhenAttack)
        {
            CharacterState = CharacterState.Damage;
            yield return new WaitForSeconds(time);

            CharacterState = CharacterState.Idle;
            Client.Instance.SendCharacterInfo(this);

            _damageCoroutine = null;
        }
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

    // ĳ������ ������� ����ϴ�.
    public virtual void HideCharacter()
    {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.isKinematic = true;
        _capsuleCollider.enabled = false;
        _spriteRenderer.enabled = false;
        IsHide = true;
    }

    public virtual void ShowCharacter()
    {
        _rigidBody.isKinematic = false;
        _capsuleCollider.enabled = true;
        _spriteRenderer.enabled = true;
        IsHide = false;
    }
    public void SetVelocity(Vector2 velocity)
    {
        _rigidBody.velocity = velocity;
    }
    public void SetCurrentSpeed(float speed)
    {
        _currentSpeed = speed;
    }
    public void Dodge()
    {
        if(CharacterState == CharacterState.Idle)
            CharacterState = CharacterState.Dodge;
    }

    public void SetAnimatorBool(string name, bool value) => _animator.SetBool(name, value);
    public void SetAnimatorInteger(string name, int value) => _animator.SetInteger(name, value);
    public void SetAnimatorTrigger(string name) => _animator.SetTrigger(name);
    public void ResetAnimatorTrigger(string name) => _animator.ResetTrigger(name); 

    public void SetAnimatorFloat(string name, float value) => _animator.SetFloat(name, value);

    public void RegisterAttack()
    {
        RegisterAttackHandler?.Invoke();
    }
    public void SetMovePacket(S_BroadcastCharacterInfo packet)
    {
        SetCharacterDirection(new Vector2(packet.characterMoveDirection, 0));
        SetCurrentSpeed(packet.xSpeed);
        SetVelocity(new Vector2(packet.xSpeed, packet.ySpeed));
        AttackType = packet.attackType;
        IsJumping = packet.isJumping;
        IsAttacking = packet.isAttacking;
        IsConncetCombo = packet.isConnectCombo;
        CharacterState = (CharacterState)packet.characterState;
    }
}
public enum CharacterState
{
    Idle,
    Attack,
    Damage,
    Dodge,
}