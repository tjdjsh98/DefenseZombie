using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCharacter : Character
{
    AnimatorHandler _animatorHandler;
    [SerializeField] Range _attackRange;
    [SerializeField] Vector3 _injectPosition;
    [SerializeField] public Vector3 InjectPosition
    {
        get
        {
            Vector3 temp = _injectPosition;
            temp.x = transform.localScale.x > 0 ? temp.x : -temp.x;
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
        Gizmos.DrawWireCube(transform.position + _attackRange.center, _attackRange.size);
        Gizmos.DrawWireSphere(transform.position + InjectPosition, 0.05f);
    }

    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        SetAnimatorBool("Attack", IsAttacking);
    }

    void OnAttack()
    {
        if (_projectile == null)
        {
            PlayerCharacter character = Util.GetGameObjectByPhysics<PlayerCharacter>(transform.position, _attackRange, LayerMask.GetMask("Character"));

            character?.Damage(1, Vector2.right * transform.localScale.x, 10, 0.1f);
        }
        else
        {
            ParabolaProjectile projectile = Instantiate(_projectile);
            projectile.transform.position = transform.position + InjectPosition;
            projectile.StartAttack(Target.transform.position, 5, 2);
        }
    }

    void OnAttackEnd()
    {
        IsAttacking = false;
    }

}