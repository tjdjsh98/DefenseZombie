using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D.IK;

public class ZombieCharacter : Character
{
    AnimatorHandler _animatorHandler;
    [SerializeField] Range _attackRange;
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
            Range temp = _attackRange;
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

        SetAnimatorBool("Attack", IsAttacking);
    }

    void OnAttack()
    {
        if (_projectile == null)
        {
            PlayerCharacter character = Util.GetGameObjectByPhysics<PlayerCharacter>(transform.position, AttackRange, LayerMask.GetMask("Character"));
            Building building = Util.GetGameObjectByPhysics<Building>(transform.position, AttackRange, LayerMask.GetMask("Character"));

            if (character != null)
            {
                character?.Damage(1, Vector2.right * transform.localScale.x, 10, 0.1f);
            }
            else if(building != null)
            {
                building?.Damage(1);
            }
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
        CharacterState = CharacterState.Idle;
    }

}