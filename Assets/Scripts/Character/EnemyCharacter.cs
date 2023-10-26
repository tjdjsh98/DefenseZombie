using UnityEngine;

public class EnemyCharacter : Character
{
    AnimatorHandler _animatorHandler;
    [SerializeField] AttackData _attack;
    [SerializeField] Vector3 _injectPosition;
    Vector3 InjectPosition
    {
        get
        {
            Vector3 temp = _injectPosition;
            temp.x = transform.localScale.x > 0 ? temp.x : -temp.x;
            return temp;
        }
    }
    Range AttackRange
    {
        get
        {
            Range temp = _attack.attackRange;
            temp.center.x = transform.localScale.x > 0 ? temp.center.x : -temp.center.x;

            return temp;
        }
    }
    [SerializeField] ParabolaProjectile _projectile;

    public Character Target { set; get; }

    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _animatorHandler.AttackHandler += OnAttack;
        _animatorHandler.AttackEndHandler += OnAttackEnd;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + AttackRange.center, AttackRange.size);
        Gizmos.DrawWireSphere(transform.position + InjectPosition, 0.05f);
    }

    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        if(CharacterState == CharacterState.Idle)
        {
            if(_currentSpeed != 0)
            {
                SetAnimatorBool("Walk", true);
            }
            else
            {
                SetAnimatorBool("Walk", false);
            }
        }
        if(CharacterState == CharacterState.Damage && !IsStagger) 
        {
            IsStagger = true;
            SetAnimatorTrigger("Hit");
            SetAnimatorBool("Stagger", IsStagger);

        }
        if (CharacterState != CharacterState.Damage && IsStagger)
        {
            IsStagger = false;
            SetAnimatorBool("Stagger", IsStagger);

        }
        SetAnimatorBool("Attack", IsAttacking);
    }

    void OnAttack()
    {
        if (_projectile == null)
        {

            RaycastHit2D[] hits;
            Util.GetHItsByPhysics(transform, _attack, Define.PlayerLayerMask | Define.BuildingLayerMask, out hits);

            foreach(var hit in hits)
            {
                Character character = hit.collider.gameObject.GetComponent<Character>();
                Building building = hit.collider.gameObject.GetComponent<Building>();
                if (character != null)
                {
                    character?.Damage(_attack.damage, Vector2.right * transform.localScale.x, _attack.power, _attack.stagger);
                    
                }
                else if (building != null)
                {
                    building?.Damage(1);
                }

                if (character != null || building != null)
                {
                    Vector3 point = hit.point;

                    GameObject hitEffect = Manager.Data.GetEffect(_attack.hitEffectName);
                    if (hitEffect)
                    {
                        GameObject g = Instantiate(hitEffect);
                        g.transform.position = point;
                    }
                }

            }
        }
        else
        {
            if (Target != null)
            {
                ParabolaProjectile projectile = Instantiate(_projectile);
                projectile.transform.position = transform.position + InjectPosition;
                projectile.StartAttack(Target.transform.position, 5, 2);
            }
        }
    }

    void OnAttackEnd()
    {
        IsAttacking = false;
        CharacterState = CharacterState.Idle;
    }

}