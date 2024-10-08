using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour, IHp, IDataSerializable
{
    [field: SerializeField] public int CharacterId { set; get; } = -1;
    protected Rigidbody2D _rigidBody;
    [field: SerializeField] public CharacterName CharacterName { private set; get; }
    public Rigidbody2D RigidBody => _rigidBody;
    protected SpriteRenderer[] _spriteRenderers;
    protected CapsuleCollider2D _capsuleCollider;
    protected Animator _animator;

    GameObject _model;

    [SerializeField] protected Range _characterSize;
    public Range CharacterSize => _characterSize;

    [SerializeField] protected bool _isEnableFly;

    protected Vector2 _characterMoveDirection;
    public Vector2 CharacterMoveDirection => _characterMoveDirection;


    // 캐릭터 상태
    [SerializeField] protected int _maxHp;
    public int MaxHp => _maxHp + AddedHp;
    public int AddedHp { set; get; }
    protected int _hp;
    public int Hp { get { return _hp; } set { _hp = value; } }

    int _defense = 0;
    public int AddedDefense { set; get; }
    public int Defense=>_defense + AddedDefense;

    public bool _isSuperArmerWhenAttack;

    [Range(0, 100)][SerializeField] protected int _standing;

    public int AttackType { get; set; } = 0;
    // 속도
    [SerializeField] protected float _accelSpeed = 2.0f;
    [SerializeField] protected float _groundBreakSpeed;
    [SerializeField] protected float _airBreakSpeed;
    protected float BreakSpeed => IsContactGround ? _groundBreakSpeed : _airBreakSpeed;
    [SerializeField] protected float _maxSpeed = 5.0f;
    public float Speed=>_maxSpeed + AddedSpeed; 
    public float AddedSpeed { set; get; }
    // 점프
    [SerializeField] protected Range _groundCheckRange;
    [SerializeField] protected float _jumpPower = 10.0f;
    protected int _jumpCount = 0;

    public Action<float> TurnedHandler;

    protected float _ySpeed;
    public float YSpeed => _ySpeed;


    // 행동 제약
    public bool IsEnableMove { get; set; } = true;
    public bool IsEnableAttack { get; set; } = true;
    public bool IsEnableMoveWhileAttack { get; set; } = false;

    // 캐릭터 상태
    public bool IsAttacking { set; get; }
    public bool IsJumping { set; get; }
    public bool IsContactGround { set; get; }
    public bool IsConncetCombo { set; get; }
    public bool IsDodge { set; get; }

    public bool IsDamaged { set; get; }
    public bool IsHide { set; get; }

    // 행동 핸들러
    public Action RegisterAttackHandler;
    public Action DeadHandler;

    public Vector2 GetVelocity => _rigidBody.velocity;

    protected Coroutine _damageCoroutine;
    protected Coroutine _shakingCoroutine;

    //캐릭터가 더미라면 필요한 변수
    protected Vector3 _currentPos;

    float _time = 0;
    float _interval = 0.1f;

    // 멀티에서 UI를 띄어주기위한 변수
    protected bool _isHited;
    protected int _damage;

    // 캐릭터의 옵션
    protected List<ICharacterOption> _optionList = new List<ICharacterOption>();

    float _recoverTime;
    public bool InitDone = false;

    public virtual void Init()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _model = transform.Find("Model").gameObject;
        _spriteRenderers = _model.GetComponentsInChildren<SpriteRenderer>();


        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();

        Hp = _maxHp;

        ICharacterOption[] options = GetComponents<ICharacterOption>();
        foreach (var option in options)
        {
            _optionList.Add(option);
            option.Init();
        }
        InitDone = true;
    }

    public void AddOption(ICharacterOption option)
    {
        _optionList.Add(option);
        option.Init();
    }

    public void RemoveOption(ICharacterOption option)
    {
        if (_optionList.Contains(option))
        {
            option.Remove();
            _optionList.Remove(option);
        }
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
        if (tag == CharacterTag.Player.ToString() && InitDone && Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            _recoverTime += Time.deltaTime;

            if (_recoverTime > 5)
            {
                _recoverTime = 0;
                if (Hp < MaxHp)
                {
                    Hp++;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (Client.Instance.IsSingle) return;
        if (Client.Instance.ClientId == CharacterId) return;
        if (CharacterId > 100 && Client.Instance.IsMain) return;

        _time += Time.fixedDeltaTime;

        transform.position = Vector3.Lerp(transform.position, _currentPos, 0.01f);

    }
    protected virtual void ControlAnimation()
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

        SetAnimatorBool("Damaged", IsDamaged);

        SetAnimatorBool("Attack", IsAttacking);
    }


    protected virtual void MoveCharacter()
    {
        if (IsDamaged || !IsEnableMove || IsHide || (IsAttacking && !IsEnableMoveWhileAttack))
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
                Turn(_rigidBody.velocity.x);
                SetXVelocity(_rigidBody.velocity.x + _accelSpeed * Mathf.Clamp01(_characterMoveDirection.x) * Time.deltaTime);
            }

            if (_rigidBody.velocity.x > Speed)
                SetXVelocity(Speed);
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
                Turn(_rigidBody.velocity.x);
                SetXVelocity(_rigidBody.velocity.x - _accelSpeed * Mathf.Clamp01(Mathf.Abs(_characterMoveDirection.x)) * Time.deltaTime);
            }

            if (_rigidBody.velocity.x < -Speed)
                SetXVelocity(-Speed);
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

                if (_rigidBody.velocity.y > Speed)
                    SetYVelocity(Speed);
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

                if (_rigidBody.velocity.y < -Speed)
                    SetYVelocity(-Speed);
            }
        }


        if (!_isEnableFly && _jumpCount == 0 && IsJumping)
        {
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            IsJumping = false;
            _jumpCount++;
        }

    }

    public void SetHp(int hp)
    {
        _maxHp = hp;
        _hp = hp;
    }

    protected void SetXVelocity(float x)
    {
        _rigidBody.velocity = new Vector3(x, _rigidBody.velocity.y);
    }
    protected void SetYVelocity(float y)
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, y);
    }

    protected void CheckGround()
    {
        _ySpeed = _rigidBody.velocity.y;

        if (Util.GetGameObjectByPhysics(transform.position, _groundCheckRange, LayerMask.GetMask("Ground")))
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
        if (constraints != -1)
            _rigidBody.constraints = (RigidbodyConstraints2D)constraints;

        if (power != 0)
            _rigidBody.AddForce(direction.normalized * (power * (1 - _standing / 100f)), ForceMode2D.Impulse);
    }
    public void Jump()
    {
        if (_jumpCount == 0)
        {
            IsJumping = true;
        }
    }
    public void StopMove()
    {
        _rigidBody.velocity = new Vector2(0, _isEnableFly ? 0 : _rigidBody.velocity.y);
    }
    public virtual void Turn(float direction)
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

        dmg = dmg - Defense;
        if (dmg < 0) dmg = 0;

        if (dmg != 0)
        {
            _recoverTime = 0;
            _rigidBody.velocity = Vector2.zero;
            AddForce(attackDirection, power);


            if (_shakingCoroutine != null) StopCoroutine(_shakingCoroutine);
            _shakingCoroutine = StartCoroutine(CorShaking());


            if (staggerTime > 0)
            {
                if (_damageCoroutine != null) StopCoroutine(_damageCoroutine);
                _damageCoroutine = StartCoroutine(CorTurnToIdle(staggerTime));
            }

            Hp -= dmg;
            (Manager.UI.GetUI(UIName.TextDisplayer) as UI_TextDisplayer).DisplayText(dmg.ToString(), transform.position + Vector3.up, Color.red, 5);

            _isHited = true;
            _damage = dmg;
            Client.Instance.SendCharacterInfo(this);
            _isHited = false;


            if (Hp < 0) Hp = 0;

            if (Hp <= 0)
                Dead();
        }
        else
        {
            (Manager.UI.GetUI(UIName.TextDisplayer) as UI_TextDisplayer).DisplayText("가드", transform.position + Vector3.up, new Color(0.3f,0.3f,.9f,1f), 5);
            _isHited = true;
            _damage = 0;
            Client.Instance.SendCharacterInfo(this);
            _isHited = false;

        }
    }

    protected virtual void Dead()
    {
        DeadHandler?.Invoke();
        Manager.Character.RemoveCharacter(CharacterId);
    }

    protected IEnumerator CorTurnToIdle(float time)
    {
        if (!_isSuperArmerWhenAttack)
        {
            IsDamaged = true;
            yield return new WaitForSeconds(time);

            IsDamaged = false;
            Client.Instance.SendCharacterInfo(this);

            _damageCoroutine = null;
        }
    }

    protected IEnumerator CorShaking()
    {
        int count = 10;
        while (count-- > 0)
        {
            _model.transform.localPosition = new Vector3((count % 2 == 0 ? 1 : -1) * Random.Range(0.01f, 0.03f), 0, 0);

            yield return new WaitForSeconds(0.05f);
        }

        _model.transform.localPosition = Vector3.zero;
    }
    public GameObject GetOverrapGameObject(int layerMask = -1)
    {
        GameObject gameObject = null;

        gameObject = Util.GetGameObjectByPhysics(transform.position, _characterSize, layerMask);

        return gameObject;
    }
    public List<T> GetOverrapGameObjects<T>(int layerMask = -1)
    {
        List<T> gameObjectList;

        gameObjectList = Util.GetGameObjectsByPhysics<T>(transform.position, _characterSize, layerMask);

        return gameObjectList;
    }

    public T GetOverrapGameObject<T>(int layerMask = -1)
    {
        T gameObject = default(T);

        gameObject = Util.GetGameObjectByPhysics<T>(transform.position, _characterSize, layerMask);

        return gameObject;
    }

    // 캐릭터의 모습만을 숨깁니다.
    public virtual void HideCharacter()
    {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.isKinematic = true;
        _capsuleCollider.enabled = false;

        foreach (var sr in _spriteRenderers)
        {
            sr.enabled = false;
        }
        IsHide = true;
    }

    public virtual void ShowCharacter()
    {
        _rigidBody.isKinematic = false;
        _capsuleCollider.enabled = true;
        foreach (var sr in _spriteRenderers)
        {
            sr.enabled = true;
        }
        IsHide = false;
    }
    public void SetVelocity(Vector2 velocity)
    {
        _rigidBody.velocity = velocity;
    }

    public void Dodge()
    {
        if (!IsContactGround) return;

        IsDodge = true;
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

    // 각 클라이언트가 보낸 정보를 메인 클라이언트에서 취합한다.
    public void SetCharacterControlInfoPacket(S_BroadcastCharacterControlInfo packet)
    {
        if (packet.characterId == Client.Instance.ClientId) return;       // 자기 자신 캐릭터 제외
        if (packet.characterId > 100 && Client.Instance.IsMain) return;   // 플레이어블 캐릭터 이외 제외

        _currentPos = new Vector3(packet.posX, packet.posY, 0);

        _interval = _time;
        _time = 0;

        DeserializeControlData(packet.data);
    }

    // 메인 클라이언트에서 보낸 정보를 각 클라리언트에서 취합한다.
    public void SetCharacterInfoPacket(S_BroadcastCharacterInfo packet)
    {
        if (Client.Instance.IsMain) return;   // 메인 클라이언트는 모든 정보를 버림


        Hp = packet.hp;

        DeserializeData(packet.data);
    }



    public void FreezeRigidbody()
    {
        _rigidBody.isKinematic = true;
    }

    public void ReleaseRigidbody()
    {
        _rigidBody.isKinematic = false;
    }

    public virtual string SerializeData()
    {
        _interval = _time;
        Util.StartWriteSerializedData();

        Util.WriteSerializedData(MaxHp);
        Util.WriteSerializedData(IsHide);
        Util.WriteSerializedData(IsDamaged);
        Util.WriteSerializedData(_isHited);
        Util.WriteSerializedData(_damage);

        foreach (var option in _optionList)
        {
            option.DataSerialize();
        }

        string stringData = Util.EndWriteSerializeData();


        return stringData;
    }
    public virtual void DeserializeData(string stringData)
    {
        if (string.IsNullOrEmpty(stringData)) return;

        Util.StartReadSerializedData(stringData);

        _maxHp = Util.ReadSerializedDataToInt();
        bool isHide = Util.ReadSerializedDataToBoolean();
        if (!IsHide && isHide)
            HideCharacter();
        if (IsHide && !isHide)
            ShowCharacter();
        bool _isDamaged = Util.ReadSerializedDataToBoolean();
        SetAnimatorBool("Damaged", _isDamaged);
        if (_isDamaged && _isDamaged != IsDamaged)
        {
            if (_shakingCoroutine != null) StopCoroutine(_shakingCoroutine);
            _shakingCoroutine = StartCoroutine(CorShaking());
        }
        IsDamaged = _isDamaged;

        bool isHited = Util.ReadSerializedDataToBoolean();
        int damage = Util.ReadSerializedDataToInt();
        if(isHited)
        {
            if (damage == 0)
                (Manager.UI.GetUI(UIName.TextDisplayer) as UI_TextDisplayer).DisplayText("가드", transform.position + Vector3.up, new Color(0.3f, 0.3f, .9f, 1f), 5);
            else
                (Manager.UI.GetUI(UIName.TextDisplayer) as UI_TextDisplayer).DisplayText(damage.ToString(), transform.position + Vector3.up, Color.red, 5);
        }


        foreach (var option in _optionList)
        {
            option.DataDeserialize();
        }
    }

    public virtual string SeralizeControlData()
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

        foreach (var option in _optionList)
        {
            option.SerializeControlData();
        }

        return Util.EndWriteSerializeData();
    }

    public virtual void DeserializeControlData(string stringData)
    {
        Util.StartReadSerializedData(stringData);

        Turn(Util.ReadSerializedDataToFloat());
        SetXVelocity(Util.ReadSerializedDataToFloat());
        SetYVelocity(Util.ReadSerializedDataToFloat());
        SetCharacterDirection(new Vector3(Util.ReadSerializedDataToFloat(), 0, 0));
        AttackType = Util.ReadSerializedDataToInt();
        IsAttacking = Util.ReadSerializedDataToBoolean();
        IsJumping = Util.ReadSerializedDataToBoolean();
        IsContactGround = Util.ReadSerializedDataToBoolean();
        IsConncetCombo = Util.ReadSerializedDataToBoolean();

        foreach (var option in _optionList)
        {
            option.DeserializeControlData();
        }
    }
}

public enum CharacterState
{
    Idle,
    Attack,
    Damage,
    Dodge,
}