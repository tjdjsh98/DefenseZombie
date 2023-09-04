using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Character _character;
    protected AnimatorHandler _animatorHandler;

    [SerializeField] protected Range _attackRange;
    [SerializeField] protected int _damage;
    [SerializeField] protected float _power;
    [SerializeField] protected float _stagger;
    [SerializeField] protected int _penetrationPower = 0;

    [SerializeField] protected string _enableAttackLayer = "Character";
    
    protected bool _isPress;

    protected virtual void Awake()
    {
        _character = GetComponentInParent<Character>();
        _animatorHandler = GetComponentInParent<AnimatorHandler>();
        _animatorHandler.AttackHandler += Attack;
        _animatorHandler.AttackEndHandler += OnAttackEnd;
    }

    protected virtual void OnDrawGizmosSelected()
    {
    
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _isPress = true;
            _character.IsAttacking = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _isPress = false;
        }
    }


    public virtual void Attack()
    {
        Range attackRange = _attackRange;
        attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attackRange.center.x : -_attackRange.center.x);

        int layerMask = LayerMask.GetMask(_enableAttackLayer);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);

        int penetration = 0;

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                Character character = hit.collider.GetComponentInParent<Character>();
                if (character != null && character != _character)
                {
                    character.Damage(_damage, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, _power, _stagger);
                    penetration++;
                    if (penetration > _penetrationPower) break;
                }
            }

        }
    }

    protected virtual void OnAttackEnd()
    {
        if (!_isPress)
            _character.IsAttacking = false;
    }
}

[System.Serializable]
public struct Range
{
    public Vector3 center;
    public Vector3 size;
}
