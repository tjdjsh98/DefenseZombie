using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected Character _character;
    protected AnimatorHandler _animatorHandler;

    [SerializeField] protected int _attackType;
    [SerializeField] protected Attack[] _attacks;

    [SerializeField] protected string _enableAttackLayer = "Character";
    
    protected bool _isPress;

    protected virtual void Awake()
    {
        _character = GetComponentInParent<Character>();
        _animatorHandler = GetComponentInParent<AnimatorHandler>();
        
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (_attackType >= _attacks.Length) return;

        Gizmos.color = Color.red;

        Range attackRange = _attacks[_attackType].attackRange;
        if(_character != null)
            attackRange.center.x = (_character?.gameObject.transform.localScale.x > 0 ? attackRange.center.x : -attackRange.center.x);

        switch (_attacks[_attackType].attacKShape)
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
        Gizmos.DrawRay(transform.position, _attacks[_attackType].AttackDirection.normalized);

    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _isPress = true;
            _character.IsAttacking = true;
            _character.AttackType = _attackType;
            _animatorHandler.AttackHandler = Attack;
            _animatorHandler.AttackEndHandler = OnAttackEnd;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _isPress = false;
        }
    }


    public virtual void Attack()
    {
        if (_attacks.Length <= _attackType) return;

        Range attackRange = _attacks[_attackType].attackRange;
        attackRange.center.x = (_character.gameObject.transform.localScale.x > 0 ? _attacks[_attackType].attackRange.center.x : -_attacks[_attackType].attackRange.center.x);

        int layerMask = LayerMask.GetMask(_enableAttackLayer);
        RaycastHit2D[] hits;
        switch (_attacks[_attackType].attacKShape)
        {
            case Define.AttacKShape.Rectagle:
                hits = Physics2D.BoxCastAll(transform.position + attackRange.center,  attackRange.size,0,Vector2.zero,0,layerMask);
                break;
            case Define.AttacKShape.Raycast:
                hits = Physics2D.RaycastAll(transform.position + attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);
                break;
            default:
                hits = Physics2D.BoxCastAll(transform.position + attackRange.center,  attackRange.size,0,Vector2.zero,0,layerMask);
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
                    Camera.main.GetComponent<CameraMove>().ShakeCamera(_attacks[_attackType].power, 0.4f);
                    Vector3 attackDirection = _attacks[_attackType].AttackDirection;
                    attackDirection.x = _character.transform.localScale.x >0 ? attackDirection.x : -attackDirection.x;
                    character.Damage(_attacks[_attackType].damage, attackDirection, _attacks[_attackType].power, _attacks[_attackType].stagger);
                    Vector3 point = hit.point;
                    if (_attacks[_attackType].effect)
                    {
                        GameObject g = Instantiate(_attacks[_attackType].effect);
                        g.transform.position = point;
                    }

                    penetration++;
                    if (penetration > _attacks[_attackType].penetrationPower) break;
                }
            }

        }
    }

    protected virtual void OnAttackEnd()
    {
        if (!_isPress)
        {
            _character.IsAttacking = false;
            _animatorHandler.AttackHandler = null;
            _animatorHandler.AttackEndHandler = null;
        }
    }
}

