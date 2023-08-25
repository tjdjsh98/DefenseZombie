using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Character _character;

    [SerializeField]Range _searchRange;
    [SerializeField]Range _attackRange;

    Character _target;

    private void Awake()
    {
        _character = GetComponent<Character>();

        _character.TurnHandler += OnTurn;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + _searchRange.center, _searchRange.size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + _attackRange.center, _attackRange.size);
    }

    private void Update()
    {
        AI();
    }

    void AI()
    {
        if (_character.CharacterState == CharacterState.Idle)
        {
            if (_target == null)
            {
                Search();
            }
            else
            {
                _character.SetCharacterDirection(_target.transform.position - transform.position);
            }
        }
        else
        {
            _character.SetCharacterDirection(Vector2.zero);
        }
    }


    void Search()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + _searchRange.center, _searchRange.size, 0, Vector2.zero);
        if (hits.Length <= 0) return;

        foreach(RaycastHit2D hit in hits)
        {
            Character character = hit.collider.gameObject.GetComponentInParent<Character>();
            if (character != null)
            {
                _target = character;
                break;
            }
        }
    }

    void OnTurn(float direction)
    {
        _attackRange.center.x = Mathf.Abs(_attackRange.center.x) * direction > 0 ? 1 : -1; 
        _searchRange.center.x = Mathf.Abs(_searchRange.center.x) * direction > 0 ? 1 : -1; 
    }
}
