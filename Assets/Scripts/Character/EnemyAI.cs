using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    ZombieCharacter _character;

    [SerializeField]Range _searchRange;
    [SerializeField]Range _attackRange;

    Range AttackRange
    {
        get
        {
            Range range = _attackRange;
            range.center.x = _attackRange.center.x * transform.lossyScale.x;
            range.size.x = _attackRange.size.x * transform.lossyScale.x;

            return range;
        }
    }
    Range SearchRange
    {
        get
        {
            Range range = _searchRange;
            range.center.x = _searchRange.center.x * transform.lossyScale.x;

            return range;
        }
    }

    private void Awake()
    {
        _character = GetComponent<ZombieCharacter>();
        _character.Speed = Random.Range(_character.Speed-3, _character.Speed+2);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + SearchRange.center, SearchRange.size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + AttackRange.center, AttackRange.size);
    }

    private void Update()
    {
        AI();
    }

    void AI()
    {
        if (_character.Target != null && !_character.Target.gameObject.activeSelf) _character.Target = null;
        if (_character.IsAttacking) return;

        if (_character.CharacterState == CharacterState.Idle)
        {
            if (_character.Target == null)
            {
                Search();
            }
            else
            {
                if (Util.GetGameObjectByPhysics<PlayerCharacter>(transform.position, AttackRange, LayerMask.GetMask("Character")) == _character.Target)
                {
                    _character.SetCharacterDirection(Vector2.zero);
                    _character.IsAttacking = true;
                }
                else
                {
                    _character.IsAttacking = false;
                    _character.SetCharacterDirection(_character.Target.transform.position - transform.position);
                }
            }
        }
        else
        {
            _character.SetCharacterDirection(Vector2.zero);
        }
    }


    void Search()
    {
        int layerMask = LayerMask.GetMask("Character");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + SearchRange.center, SearchRange.size, 0, Vector2.zero,0,layerMask);
        if (hits.Length <= 0) return;

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Character character = hit.collider.gameObject.GetComponentInParent<Character>();
                if (character != null)
                {
                    _character.Target = character;
                    break;
                }
            }
        }
    }
}
