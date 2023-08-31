using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    Character _character;
    AnimatorHandler _animatorHandler;

    [SerializeField] Range _attackRange;
    [SerializeField] int _damage;
    [SerializeField] float _power;
    [SerializeField] float _stagger;
    [SerializeField] int _penetrationPower = 0;
    
    [SerializeField] string _enableAttackLayer = "Character";

    bool _isPress;

    private void Awake()
    {
        _character = GetComponentInParent<Character>();
        _animatorHandler = GetComponentInParent<AnimatorHandler>();
        _animatorHandler.AttackHandler += Fire;
        _animatorHandler.AttackEndHandler += OnAttackEnd;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (_character == null)
        {
            Gizmos.DrawLine(transform.position + _attackRange.center, transform.position + _attackRange.center + _attackRange.size * transform.parent.localScale.x);
        }
        else
        {
            Range attackRange = _attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attackRange.center.x : -_attackRange.center.x);
            Gizmos.DrawLine(transform.position + attackRange.center, transform.position + attackRange.center + attackRange.size * transform.parent.localScale.x);
        }
    }

    private void Update()
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


    public void Fire()
    {
        Range attackRange = _attackRange;
        attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attackRange.center.x : -_attackRange.center.x);

        int layerMask = LayerMask.GetMask(_enableAttackLayer);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x,layerMask);

        int penetration = 0;

        if(hits.Length > 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                Character character = hit.collider.GetComponentInParent<Character>();
                if (character != null)
                {
                    character.Damage(_damage, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, _power,_stagger);
                    penetration++;
                    if(penetration > _penetrationPower) break;
                }
            }
        }
    }

    void OnAttackEnd()
    {
        if(!_isPress)
            _character.IsAttacking = false;
    }
}

[System.Serializable]
public struct Range
{
    public Vector3 center;
    public Vector3 size;
}
