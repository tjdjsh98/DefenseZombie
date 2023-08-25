using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    Character _character;
    [SerializeField] Range _attackRange;

    private void Awake()
    {
        _character = GetComponentInParent<Character>();

        _character.TurnHandler += OnTurn;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + _attackRange.center, transform.position + _attackRange.center + _attackRange.size * transform.parent.localScale.x);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Fire();
    }

    public void Fire()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + _attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, _attackRange.size.x);

        if(hit.collider != null)
        {
            Character character = hit.collider.GetComponentInParent<Character>();
            if (character != null)
                character.Damage(1, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, 20);
        }
    }

    void OnTurn(float direction)
    {
        if (direction > 0)
            _attackRange.center.x = Mathf.Abs(_attackRange.center.x);
        else
            _attackRange.center.x = -Mathf.Abs(_attackRange.center.x);
    }
}

[System.Serializable]
public struct Range
{
    public Vector3 center;
    public Vector3 size;
}
