using UnityEngine;

public class Weapon : MonoBehaviour,ICharacterOption
{
    [SerializeField]protected CustomCharacter _character;
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
            if (_character == null)
            {
                return _defaultAttack;
            }

            if(_character.WeaponData== null)
            {
                return _character.DefaultWeapon.AttackList[_attackType];
            }
            else
            {
                return _character.WeaponData.AttackList[_attackType];

            }
        }
    }

    [SerializeField] protected string _enableAttackLayer = "Enemy";
    
    public bool Controllable => _playerController != null && _character.CharacterId == Client.Instance.ClientId;
    public virtual void Init()
    {
        _character = GetComponent<CustomCharacter>();
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
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Range attackRange = WeaponAttackData.attackRange;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(_frontWeapon.transform.position, _frontWeapon.transform.rotation, _frontWeapon.transform.localScale);
        Gizmos.matrix = rotationMatrix;

        if (_character != null)
            attackRange.center.x = (_character?.gameObject.transform.localScale.x > 0 ? attackRange.center.x : -attackRange.center.x);

        Vector3 point = WeaponAttackData.attackEffectPoint;

        Gizmos.DrawWireSphere( point, 0.1f);

        if(WeaponAttackData.projectile)
        {
            Gizmos.DrawWireSphere( WeaponAttackData.firePos, 0.1f);
        }

        switch (WeaponAttackData.attacKShape)
        {
            case Define.AttacKShape.Rectagle:
                Gizmos.DrawWireCube( attackRange.center, attackRange.size);
                break;
            case Define.AttacKShape.Circle:
                Gizmos.DrawWireSphere(_frontWeapon.transform.position + attackRange.center, attackRange.size.x);
                break;
            case Define.AttacKShape.Raycast:
                Gizmos.DrawRay(attackRange.center, (_character == null ? Vector3.right : _character.transform.localPosition.x> 0 ? Vector3.right : Vector3.left) * attackRange.size.x );
                break;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, WeaponAttackData.AttackDirection.normalized);

    }

    protected virtual void OnAttackKeyDown()
    {
        if (_character.IsLift) return;
        if (_character.IsAttacking) return;

        _character.IsAttacking = true;
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
        Client.Instance.SendCharacterInfo(_character);
    }
    protected virtual void OnAttackKeyUp()
    {
    }

    public virtual void Attack()
    {   
        if (WeaponAttackData.projectile == null)
        {
            Range attackRange = WeaponAttackData.attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? WeaponAttackData.attackRange.center.x : -WeaponAttackData.attackRange.center.x);

            int layerMask = LayerMask.GetMask(_enableAttackLayer);
            RaycastHit2D[] hits;
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

            int penetration = 0;

            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    Character character = hit.collider.GetComponentInParent<Character>();
                    if (character != null && character != _character)
                    {
                        Camera.main.GetComponent<CameraMove>().ShakeCamera(WeaponAttackData.power, 0.4f);

                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            Vector3 attackDirection = WeaponAttackData.AttackDirection;
                            attackDirection.x = _character.transform.localScale.x > 0 ? attackDirection.x : -attackDirection.x;
                            character.Damage(WeaponAttackData.damage, attackDirection, WeaponAttackData.power, WeaponAttackData.stagger);

                            Vector3 point = hit.point;

                            GameObject hitEffect = Manager.Data.GetEffect(WeaponAttackData.hitEffectName);
                            if (hitEffect)
                            {
                                GameObject g = Instantiate(hitEffect);
                                g.transform.position = point;
                            }
                        }
                        penetration++;
                        if (penetration > WeaponAttackData.penetrationPower) break;
                    }
                }

            }
        }
        else
        {
            _frontfirePoint.transform.localPosition = WeaponAttackData.firePos;

            Projectile projectile = Instantiate(WeaponAttackData.projectile);
            projectile.transform.position = _frontfirePoint.transform.position;
            projectile.Fire(transform.localScale.x, _frontfirePoint.transform.eulerAngles,
                gameObject.tag == Define.CharacterTag.Player.ToString() ? Define.CharacterTag.Enemy : Define.CharacterTag.Player);
        }
    }

    protected virtual void OnAttackEnd()
    {
        Client.Instance.SendCharacterInfo(_character);
    }
}

