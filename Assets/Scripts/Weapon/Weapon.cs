using UnityEngine;
using static Define;

public class Weapon : MonoBehaviour,ICharacterOption
{
    [SerializeField]protected Character _character;
    [SerializeField]protected CustomCharacter _customCharacter;
    protected PlayerController _playerController;
    protected HelperAI _helperAi;
    protected AnimatorHandler _animatorHandler;

    [SerializeField] protected int _attackType;
    [SerializeField] protected AttackData _defaultAttack;
    
    [SerializeField] GameObject _frontWeapon;
    [SerializeField] GameObject _frontfirePoint;

    [SerializeField] GameObject _behindWeapon;
    [SerializeField] GameObject _behindfirePoint;
    public AttackData WeaponAttackData
    {
        get 
        {
            if(_customCharacter != null)
            {
                return _customCharacter.WeaponData.AttackList[_attackType];
            }

          
            return _defaultAttack;
          
        }
    }
    public WeaponData WeaponData
    {
        get
        {
            return _customCharacter.WeaponData;
        }
    }
    [SerializeField] protected string _enableAttackLayer = "Enemy";
    
    public bool Controllable => _playerController != null && _character.CharacterId == Client.Instance.ClientId;

    public bool IsDone { get; set; }

    float _attackTime;

    public Vector3 FireVector;

    public virtual void Init()
    {
        _character = GetComponent<Character>();
        _customCharacter = GetComponent<CustomCharacter>();

        _playerController = GetComponent<PlayerController>();
        _helperAi = GetComponent<HelperAI>();
        _animatorHandler = GetComponent<AnimatorHandler>();

        if(_playerController != null)
        {
            _playerController.AttackKeyDownHandler += OnAttackKeyDown;
            _playerController.AttackKeyUpHandler += OnAttackKeyUp;
        }
        else if(_helperAi != null)
        {
            _helperAi.AttackHanlder += OnAttackKeyDown;
        }

        if (Client.Instance.ClientId == -1  || Client.Instance.IsMain)
        {
            _animatorHandler.AttackHandler += Attack;
        }
        _animatorHandler.AttackEndHandler += OnAttackEnd;

        IsDone = true;
    }

    public void Update()
    {
        if (!IsDone) return;
        if (!_character.IsAttacking!)
            _attackTime += Time.deltaTime;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        WeaponData weaponData = null;
        if (_customCharacter != null)
            weaponData = WeaponData;

        AttackData attackData = weaponData == null ? _defaultAttack : weaponData.AttackList[0];

        if (_customCharacter != null)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(_frontWeapon.transform.position, _frontWeapon.transform.rotation, _frontWeapon.transform.localScale);
            Gizmos.matrix = rotationMatrix;
        }

        if (_character != null)
            attackData.attackRange.center.x = (_character?.gameObject.transform.localScale.x > 0 ? attackData.attackRange.center.x : -attackData.attackRange.center.x);

        Vector3 point = attackData.attackEffectPoint;

        Gizmos.DrawWireSphere(point, 0.1f);

        if (attackData.projectile)
        {
            Gizmos.DrawWireSphere(attackData.firePos, 0.1f);
        }

        if (_customCharacter != null)
        {
            switch (attackData.attacKShape)
            {
                case Define.AttacKShape.Rectagle:
                    Gizmos.DrawWireCube(attackData.attackRange.center, attackData.attackRange.size);
                    break;
                case Define.AttacKShape.Circle:
                    Gizmos.DrawWireSphere(_frontWeapon.transform.position + attackData.attackRange.center, attackData.attackRange.size.x);
                    break;
                case Define.AttacKShape.Raycast:
                    Gizmos.DrawRay(attackData.attackRange.center, (_character == null ? Vector3.right : _character.transform.localPosition.x > 0 ? Vector3.right : Vector3.left) * attackData.attackRange.size.x);
                    break;
            }
        }
        else
        {
            switch (attackData.attacKShape)
            {
                case Define.AttacKShape.Rectagle:
                    Gizmos.DrawWireCube(transform.position + attackData.attackRange.center, attackData.attackRange.size);
                    break;
                case Define.AttacKShape.Circle:
                    Gizmos.DrawWireSphere(transform.position + attackData.attackRange.center, attackData.attackRange.size.x);
                    break;
                case Define.AttacKShape.Raycast:
                    Gizmos.DrawRay(transform.position, (_character == null ? Vector3.right : _character.transform.localPosition.x > 0 ? Vector3.right : Vector3.left) * attackData.attackRange.size.x);
                    break;
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, attackData.AttackDirection.normalized);

    }

    protected virtual void OnAttackKeyDown()
    {
        if (Manager.Building.IsDrawing) return;
        if (_customCharacter != null && (_customCharacter.HoldingItem != null&&!_customCharacter.IsEquipWeapon)) return;
        if (_character.IsAttacking) return;
        if (WeaponData.AttackDelay > _attackTime) return;

        _attackTime = 0;
        _character.IsAttacking = true;
        _character.IsEnableMoveWhileAttack = true;
        _character.AttackType = _attackType;

        GameObject attackEffect = Manager.Data.GetEffect(WeaponAttackData.attackEffectName);

        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect);

            Vector3 point = WeaponAttackData.attackEffectPoint;
            point.x *= _character.transform.localScale.x > 0 ? 1 : -1;

            effect.transform.position = transform.position + point;
            Vector3 scale = Vector3.one;
            scale.x = _character.transform.localScale.x > 0 ? 1 : -1;
            effect.transform.localScale = scale;
        }
        Client.Instance.SendCharacterControlInfo(_character);
    }
    protected virtual void OnAttackKeyUp()
    {
    }

    public virtual void Attack()
    {   
        if(WeaponAttackData.actualAttackEffectName != Define.EffectName.None)
        {
            int layerMask = gameObject.tag == "Enemy" ? Define.PlayerLayerMask : Define.EnemyLayerMask;
            Character character = Util.GetGameObjectByPhysics<Character>(transform.position,WeaponAttackData.attackRange,layerMask);

            if(character != null)
            {
                GameObject effect = null;
                Manager.Effect.GenerateEffect(WeaponAttackData.actualAttackEffectName, character.transform.position, ref effect);
            }
        }
        else if (WeaponAttackData.projectile == null)
        {
            Range attackRange = WeaponAttackData.attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? WeaponAttackData.attackRange.center.x : -WeaponAttackData.attackRange.center.x);

            int layerMask = LayerMask.GetMask(_enableAttackLayer) | Define.BuildingLayerMask;
            RaycastHit2D[] hits = null ;
            if (_customCharacter != null)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(_frontWeapon.transform.position, _frontWeapon.transform.rotation, _frontWeapon.transform.localScale);
                Vector3 pos = rotationMatrix.MultiplyVector(attackRange.center);

                switch (WeaponAttackData.attacKShape)
                {
                    case Define.AttacKShape.Rectagle:

                        hits = Physics2D.BoxCastAll(_frontWeapon.transform.position + pos, attackRange.size, _frontWeapon.transform.eulerAngles.z, Vector2.zero, 0, layerMask);
                        break;
                    case Define.AttacKShape.Raycast:
                        hits = Physics2D.RaycastAll(_frontWeapon.transform.position + pos, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);
                        break;
                    default:
                        hits = Physics2D.BoxCastAll(_frontWeapon.transform.position + pos, attackRange.size, _frontWeapon.transform.eulerAngles.z, Vector2.zero, 0, layerMask);
                        break;
                }
            }
            else if(_character != null)
            {
                switch (WeaponAttackData.attacKShape)
                {
                    case Define.AttacKShape.Rectagle:

                        hits = Physics2D.BoxCastAll(transform.position + attackRange.center, attackRange.size,0, Vector2.zero, 0, layerMask);
                        break;
                    case Define.AttacKShape.Raycast:
                        hits = Physics2D.RaycastAll(transform.position + attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);
                        break;
                    default:
                        hits = Physics2D.BoxCastAll(transform.position + attackRange.center, attackRange.size, 0, Vector2.zero, 0, layerMask);
                        break;
                }
            }

            int penetration = 0;

            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    Character character = hit.collider.GetComponent<Character>();
                    Building building = hit.collider.GetComponent<Building>();
                    
                    if (character != null && character != _character)
                    {
                        Camera.main.GetComponent<CameraMove>().ShakeCamera(WeaponAttackData.power, 0.4f);

                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            Vector3 attackDirection = WeaponAttackData.AttackDirection;
                            attackDirection.x = _character.transform.localScale.x > 0 ? attackDirection.x : -attackDirection.x;
                            character.Damage(WeaponAttackData.damage, attackDirection, WeaponAttackData.power, WeaponAttackData.stagger);

                            Vector3 point = hit.point;

                            GameObject effect = null;
                            Manager.Effect.GenerateEffect(WeaponAttackData.hitEffectName, point, ref effect);
                        }
                        penetration++;
                        if (penetration > WeaponAttackData.penetrationPower) break;
                    }
                    else if(_character.tag != CharacterTag.Player.ToString() && building != null)
                    {
                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            building.Damage(WeaponAttackData.damage);

                            Vector3 point = hit.point;

                            GameObject effect = null;
                            Manager.Effect.GenerateEffect(WeaponAttackData.hitEffectName, point, ref effect);
                        }
                        penetration++;
                        if (penetration > WeaponAttackData.penetrationPower) break;
                    }
                }

            }
        }
        else
        {
            if (_customCharacter != null)
            {
                float rotation = 0;
                Vector3 firePoint = Vector3.zero;
                if ((_customCharacter.WeaponData != null && _customCharacter.WeaponData.IsFrontWeapon) || _customCharacter.WeaponData == null)
                {
                    _frontfirePoint.transform.localPosition = WeaponAttackData.firePos;
                    firePoint = _frontfirePoint.transform.position;
                    rotation = _customCharacter.GetFrontHandRotation();
                }
                else if (_customCharacter.WeaponData != null && !_customCharacter.WeaponData.IsFrontWeapon)
                {
                    _behindfirePoint.transform.localPosition = WeaponAttackData.firePos;
                    firePoint = _behindfirePoint.transform.position;
                    rotation = _customCharacter.GetBehindHandRotation();
                }
                Vector3 direction = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad) * transform.localScale.x, Mathf.Sin(rotation * Mathf.Deg2Rad)).normalized;
                Projectile projectile = null;

                CharacterTag tag1 = CharacterTag.Enemy;
                CharacterTag tag2 = CharacterTag.Enemy;

                if (gameObject.tag == CharacterTag.Player.ToString())
                {
                    tag1 = CharacterTag.Enemy;
                    tag2 = CharacterTag.Enemy;
                }
                else if (gameObject.tag == CharacterTag.Enemy.ToString())
                {
                    tag1 = CharacterTag.Player;
                    tag2 = CharacterTag.Building;
                }
                Manager.Projectile.SetPacketDetail(direction, tag1, tag2);
                Manager.Projectile.GenerateProjectile(WeaponAttackData.projectile.ProjectileName, firePoint, ref projectile);
                
                projectile?.Fire(direction, tag1, tag2);
            }
        }
    }

    protected virtual void OnAttackEnd()
    {
        Client.Instance.SendCharacterControlInfo(_character);
    }

    public void DataSerialize()
    {

    }

    public void DataDeserialize()
    {

    }
}

