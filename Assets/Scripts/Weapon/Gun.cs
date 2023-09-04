using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    protected override void OnDrawGizmosSelected()
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

    public override void Attack()
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
}