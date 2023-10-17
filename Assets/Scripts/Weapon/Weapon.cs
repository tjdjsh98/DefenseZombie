using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]protected MiniPlayerCharacter _character;
    protected PlayerController _playerController;
    protected HelperAI _helperAi;
    protected AnimatorHandler _animatorHandler;

    [SerializeField] protected int _attackType;
    [SerializeField] protected Attack _defaultAttack;

    [SerializeField] GameObject _weapon;
    [SerializeField] GameObject _firePoint;
    protected Attack _attack
    {
        get 
        {
            if(_character == null)
                return _defaultAttack;

            return _character.Attack;
        }
    }

    [SerializeField] protected string _enableAttackLayer = "Character";
    
    public bool Controllable => _playerController != null && _character.CharacterId == Client.Instance.ClientId;
    protected virtual void Awake()
    {
        _character = GetComponent<MiniPlayerCharacter>();
        _playerController = GetComponent<PlayerController>();
        _helperAi = GetComponent<HelperAI>();
        _animatorHandler = GetComponent<AnimatorHandler>();

        if(_playerController != null)
        {
            _playerController.AttackKeyDown += OnAttackKeyDown;
            _playerController.AttackKeyUp += OnAttackKeyUp;
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

        Range attackRange = _attack.attackRange;


        if(_character != null)
            attackRange.center.x = (_character?.gameObject.transform.localScale.x > 0 ? attackRange.center.x : -attackRange.center.x);

        Vector3 point = _attack.attackEffectPoint;

        Gizmos.DrawWireSphere(transform.position + point, 0.1f);

        if(_attack.projectile)
        {
            Gizmos.DrawWireSphere(transform.position + _attack.firePos, 0.1f);
        }

        switch (_attack.attacKShape)
        {
            case Define.AttacKShape.Rectagle:
                Gizmos.DrawWireCube(transform.position + attackRange.center, attackRange.size);
                break;
            case Define.AttacKShape.Circle:
                Gizmos.DrawWireSphere(transform.position + attackRange.center, attackRange.size.x);
                break;
            case Define.AttacKShape.Raycast:
                Gizmos.DrawRay(transform.position + attackRange.center, (_character == null ? Vector3.right : _character.transform.localPosition.x> 0 ? Vector3.right : Vector3.left) * attackRange.size.x );
                break;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, _attack.AttackDirection.normalized);

    }

    protected virtual void OnAttackKeyDown()
    {
        _character.IsAttacking = true;
        _character.AttackType = _attackType;
        GameObject attackEffect = Manager.Data.GetEffect(_attack.attackEffectName);

        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect);

            Vector3 point = _attack.attackEffectPoint;
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
        if (_attack.projectile == null)
        {
            Range attackRange = _attack.attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attack.attackRange.center.x : -_attack.attackRange.center.x);

            int layerMask = LayerMask.GetMask(_enableAttackLayer);
            RaycastHit2D[] hits;
            switch (_attack.attacKShape)
            {
                case Define.AttacKShape.Rectagle:
                    hits = Physics2D.BoxCastAll(transform.position + attackRange.center, attackRange.size, 0, Vector2.zero, 0, layerMask);
                    break;
                case Define.AttacKShape.Raycast:
                    hits = Physics2D.RaycastAll(transform.position + attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);
                    break;
                default:
                    hits = Physics2D.BoxCastAll(transform.position + attackRange.center, attackRange.size, 0, Vector2.zero, 0, layerMask);
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
                        Camera.main.GetComponent<CameraMove>().ShakeCamera(_attack.power, 0.4f);

                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            Vector3 attackDirection = _attack.AttackDirection;
                            attackDirection.x = _character.transform.localScale.x > 0 ? attackDirection.x : -attackDirection.x;
                            character.Damage(_attack.damage, attackDirection, _attack.power, _attack.stagger);

                            Vector3 point = hit.point;

                            GameObject hitEffect = Manager.Data.GetEffect(_attack.hitEffectName);
                            if (hitEffect)
                            {
                                GameObject g = Instantiate(hitEffect);
                                g.transform.position = point;
                            }
                        }
                        penetration++;
                        if (penetration > _attack.penetrationPower) break;
                    }
                }

            }
        }
        else
        {
            _firePoint.transform.localPosition = _attack.firePos;

            Projectile projectile = Instantiate(_attack.projectile);
            projectile.transform.position = _firePoint.transform.position;
            projectile.Fire(transform.localScale.x, _firePoint.transform.eulerAngles);
        }
    }

    protected virtual void OnAttackEnd()
    {
        Client.Instance.SendCharacterInfo(_character);
    }
}

