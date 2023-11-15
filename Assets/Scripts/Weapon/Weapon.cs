using UnityEngine;
using static Define;

public class Weapon : MonoBehaviour, ICharacterOption
{
    [SerializeField] protected Character _character;
    
    protected AnimatorHandler _animatorHandler;


    [SerializeField] protected int _attackType;
    [SerializeField] protected AttackData _defaultAttack;
    public virtual AttackData AttackData => _defaultAttack;
   
    public bool IsDone { get; set; }

    protected float _attackTime;

    public bool IsEnableAttack => _attackTime >= _defaultAttack.attackDelay;

    public Vector3 TargetPosition { set; get; }


    public virtual void Init()
    {
        _character = GetComponentInParent<Character>();
        _animatorHandler = GetComponentInParent<AnimatorHandler>();


        RegisterControl();

        IsDone = true;
    }

    public virtual void Remove()
    {
        UnregisterControl();
    }

    protected virtual void RegisterControl()
    {
      

        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
        {
            _animatorHandler.AttackHandler += Attack;
        }
        _animatorHandler.AttackEndHandler += OnAttackEnd;
    }

    protected virtual void UnregisterControl()
    {
        _animatorHandler.AttackEndHandler -= OnAttackEnd;
        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
        {
            _animatorHandler.AttackHandler -= Attack;
        }
       
    }

    public void Update()
    {
        if (!IsDone) return;
        if (!_character.IsAttacking)
            _attackTime += Time.deltaTime;
    }

    protected virtual void OnDrawGizmos()
    {
        if (_character == null) return;
        Gizmos.color = Color.red;

        AttackData attackData = _defaultAttack;

        if (_character != null)
            attackData.attackRange.center.x = (_character?.gameObject.transform.localScale.x > 0 ? attackData.attackRange.center.x : -attackData.attackRange.center.x);

        Vector3 point = attackData.attackEffectPoint;

        Gizmos.DrawWireSphere(point, 0.1f);

        if (attackData.projectile)
        {
            Gizmos.DrawWireSphere(attackData.firePos, 0.1f);
        }

      
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
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, attackData.AttackDirection.normalized);

    }

    protected virtual void OnAttackKeyDown()
    {
        if (Manager.Building.IsDrawing) return;
        if (_character.IsAttacking) return;
        if (_defaultAttack.attackDelay > _attackTime) return;

        _attackTime = 0;
        _character.IsAttacking = true;
        _character.IsEnableMoveWhileAttack = true;
        _character.AttackType = _attackType;

        GameObject attackEffect = Manager.Data.GetEffect(_defaultAttack.attackEffectName);

        if (attackEffect != null)
        {
            GameObject effect = Instantiate(attackEffect);

            Vector3 point = _defaultAttack.attackEffectPoint;
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
        // 특수 공격
        if (_defaultAttack.actualAttackEffectName != Define.EffectName.None)
        {
            int layerMask = gameObject.tag == "Enemy" ? Define.PlayerLayerMask : Define.EnemyLayerMask;
            Character character = Util.GetGameObjectByPhysics<Character>(transform.position, _defaultAttack.attackRange, layerMask);

            if (character != null)
            {
                GameObject effect = null;
                Manager.Effect.GenerateEffect(_defaultAttack.actualAttackEffectName, character.transform.position, ref effect);
            }
        }
        // 근접 공격
        else if (_defaultAttack.projectile == null)
        {
            Range attackRange = _defaultAttack.attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _defaultAttack.attackRange.center.x : -_defaultAttack.attackRange.center.x);

            int layerMask = 0;
            if (_character.tag.Equals(Define.CharacterTag.Player.ToString()))
            {
                layerMask = Define.PlayerLayerMask | Define.BuildingLayerMask;
            }
            else if (_character.tag.Equals(Define.CharacterTag.Enemy.ToString()))
            {
                layerMask = Define.EnemyLayerMask | Define.BuildingLayerMask;
            }

            RaycastHit2D[] hits = null;
          if (_character != null)
            {
                switch (_defaultAttack.attacKShape)
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
                        Camera.main.GetComponent<CameraMove>().ShakeCamera(_defaultAttack.power, 0.4f);

                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            Vector3 attackDirection = _defaultAttack.AttackDirection;
                            attackDirection.x = _character.transform.localScale.x > 0 ? attackDirection.x : -attackDirection.x;
                            character.Damage(_defaultAttack.damage, attackDirection, _defaultAttack.power, _defaultAttack.stagger);

                            Vector3 point = hit.point;

                            GameObject effect = null;
                            Manager.Effect.GenerateEffect(_defaultAttack.hitEffectName, point, ref effect);
                        }
                        penetration++;
                        if (penetration > _defaultAttack.penetrationPower) break;
                    }
                    else if (building != null && (_character.tag.Equals(CharacterTag.Player.ToString()) && building.tag.Equals(CharacterTag.Enemy.ToString())
                        || _character.tag.Equals(CharacterTag.Enemy.ToString()) && !building.tag.Equals(CharacterTag.Enemy.ToString())))
                    {
                        if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
                        {
                            building.Damage(_defaultAttack.damage);

                            Vector3 point = hit.point;

                            GameObject effect = null;
                            Manager.Effect.GenerateEffect(_defaultAttack.hitEffectName, point, ref effect);
                        }
                        penetration++;
                        if (penetration > _defaultAttack.penetrationPower) break;
                    }
                }

            }
        }
        // 원거리 공격
        else
        {
            if(_character != null)
            {
                int layerMask = gameObject.tag == "Enemy" ? Define.PlayerLayerMask : Define.EnemyLayerMask;
                Character character = Util.GetGameObjectByPhysics<Character>(transform.position, _defaultAttack.attackRange, layerMask);
                Building building = Util.GetGameObjectByPhysics<Building>(transform.position, _defaultAttack.attackRange, layerMask);

                Vector3 firePoint = transform.position;
                firePoint.x += transform.localScale.x * _defaultAttack.firePos.x;

                Vector3 direction = Vector3.zero;
                if (character != null)
                    direction = character.transform.position - firePoint;
                else if (building != null)
                    direction = building.transform.position - firePoint;
                else
                    direction = TargetPosition - firePoint;

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

                Projectile projectile = null;
                Manager.Projectile.SetPacketDetail(direction, tag1, tag2,_defaultAttack.damage);
                Manager.Projectile.GenerateProjectile(_defaultAttack.projectile.ProjectileName, firePoint, ref projectile);

                projectile?.Fire(direction, tag1, tag2, _defaultAttack.damage);

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

