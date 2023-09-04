using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hammer : Weapon
{
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (_character == null)
        {
            Gizmos.DrawWireCube(transform.position + _attackRange.center, _attackRange.size);
        }
        else
        {
            Range attackRange = _attackRange;
            attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attackRange.center.x : -_attackRange.center.x);
            Gizmos.DrawWireCube(transform.position + attackRange.center, attackRange.size);
        }
    }
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _character.CharacterState = CharacterState.Attack;
            _isPress = true;
            _character.IsAttacking = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _isPress = false;
        }
    }

    public override void Attack()
    {
        Range attackRange = _attackRange;
        attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attackRange.center.x : -_attackRange.center.x);

        int layerMask = LayerMask.GetMask(_enableAttackLayer);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + attackRange.center,  attackRange.size,0,Vector2.zero,0,layerMask);

        int penetration = 0;

        Camera.main.GetComponent<CameraMove>().ShakeCamera(_power, 0.5f);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == this.transform.parent.gameObject) continue;
                Character character = hit.collider.GetComponentInParent<Character>();
                if (character != null )
                {
                    character.Damage(_damage, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, _power, _stagger);
                    penetration++;
                    if (penetration > _penetrationPower) break;
                }
            }
        }
    }

    protected override void OnAttackEnd()
    {
        if (!_isPress)
        {
            _character.IsAttacking = false;
            if(_character.CharacterState == CharacterState.Attack)
            {
                _character.CharacterState = CharacterState.Idle;
            }
        }

    }
}
