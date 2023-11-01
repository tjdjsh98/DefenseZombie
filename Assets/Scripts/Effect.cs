using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Effect : MonoBehaviour
{
    AnimatorHandler _animatorHanlder;

    [SerializeField] EffectName _effectName;
    public EffectName EffectName => _effectName;

    [SerializeField] Range _attackRange;

    private void Awake()
    {
        _animatorHanlder= GetComponent<AnimatorHandler>();
        _animatorHanlder.AttackHandler += OnAttack;
    }


    private void OnDrawGizmosSelected()
    {
        Util.DrawRangeGizmo(gameObject, _attackRange,Color.red);
    }

    void OnAttack()
    {
        RaycastHit2D[] hits;
        Util.GetHItsByPhysics(transform, _attackRange, CharacterLayerMask | BuildingLayerMask, out hits);

        foreach(var hit in hits)
        {
            if((1 << (hit.collider.gameObject.layer) & Define.CharacterLayerMask) != 0)
            {
                Character character = hit.collider.gameObject.GetComponent<Character>();

                character.Damage(10, character.transform.position - transform.position, 20, 3);
            }
            else if ((1 << (hit.collider.gameObject.layer) & Define.BuildingLayerMask) != 0)
            {
                Building building = hit.collider.gameObject.GetComponent<Building>();

                building.Damage(10);
            }
        }
    }
}
